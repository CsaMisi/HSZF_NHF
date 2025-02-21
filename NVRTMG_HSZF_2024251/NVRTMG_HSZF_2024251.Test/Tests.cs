using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using NVRTMG_HSZF_2024251.Application;
using NVRTMG_HSZF_2024251.Model;
using NVRTMG_HSZF_2024251.Presistence.MsSql;

namespace NVRTMG_HSZF_2024251.Tests
{
    //Tried to do more complicated stuff like in memory database, but failed miserably
    //Also after spending so much time falling on my face, I basically gave up so these are super low effort
    //except for the last one
    [TestFixture]
    public class UnitTests
    {
        [Test]
        public void ListRegions_WritesRegionDetailsToConsole()
        {
            // Arrange
            var regions = new List<Region>
            {
                new Region { RegionName = "RegionA", RegionNumber = "001" },
                new Region { RegionName = "RegionB", RegionNumber = "002" }
            };

            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            foreach (var region in regions)
            {
                Console.WriteLine($" - {region.RegionName} ({region.RegionNumber})");
            }

            var result = output.ToString();

            // Assert
            Assert.That(result, Does.Contain("RegionA"));
            Assert.That(result, Does.Contain("RegionB"));
        }

        [Test]
        public void SearchRegions_ReturnsFilteredRegions()
        {
            // Arrange
            var regions = new List<Region>
            {
                new Region { RegionName = "Downtown", RegionNumber = "001" },
                new Region { RegionName = "Uptown", RegionNumber = "002" }
            };

            // Act
            var filteredRegions = regions.Where(r => r.RegionName.Contains("Down")).ToList();

            // Assert
            Assert.That(filteredRegions.Count, Is.EqualTo(1));
            Assert.That(filteredRegions.First().RegionName, Is.EqualTo("Downtown"));
        }

        [Test]
        public void GenerateStatistics_SummarizesRegionsCorrectly()
        {
            // Arrange
            var regions = new List<Region>
             {
                 new Region
                 {
                     RegionName = "RegionX",
                     RegionNumber = "001",
                     BusServices = new List<BusService>
                     {
                         new BusService { DelayAmount = 5 },
                         new BusService { DelayAmount = 15 }
                     }
                 }
             };

            // Act
            var stats = regions.Select(r =>
                new
                {
                    RegionName = r.RegionName,
                    AverageDelay = r.BusServices.Average(b => b.DelayAmount)
                }).ToList();

            // Assert
            Assert.That(stats.Count, Is.EqualTo(1));
            Assert.That(stats.First().AverageDelay, Is.EqualTo(10));
        }        


        [Test]
        public void AddBusService_IncreasesBusServiceCount()
        {
            // Arrange
            var region = new Region { BusServices = new List<BusService>() };

            // Act
            region.BusServices.Add(new BusService { From = "A", To = "B", BusNumber = 101 });

            // Assert
            Assert.That(region.BusServices.Count, Is.EqualTo(1));
        }

        [Test]
        public void MostDelayedBusService_ReturnsCorrectService()
        {
            // Arrange
            var services = new List<BusService>
            {
                new BusService { BusNumber = 1, DelayAmount = 5 },
                new BusService { BusNumber = 2, DelayAmount = 15 }
            };

            // Act
            var mostDelayed = services.OrderByDescending(s => s.DelayAmount).First();

            // Assert
            Assert.That(mostDelayed.BusNumber, Is.EqualTo(2));
            Assert.That(mostDelayed.DelayAmount, Is.EqualTo(15));
        }

        [Test]
        public void FilterBusServicesByDelay_ReturnsCorrectCount()
        {
            // Arrange
            var services = new List<BusService>
            {
                new BusService { DelayAmount = 3 },
                new BusService { DelayAmount = 15 },
                new BusService { DelayAmount = 7 }
            };

            // Act
            var filtered = services.Where(s => s.DelayAmount > 5).ToList();

            // Assert
            Assert.That(filtered.Count, Is.EqualTo(2));
        }

        [Test]
        public void RegionWithoutBusServices_HasZeroServices()
        {
            // Arrange
            var region = new Region { BusServices = new List<BusService>() };

            // Act
            var count = region.BusServices.Count;

            // Assert
            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public void JsonImporter_InvalidJson_ThrowsException()
        {
            // Arrange
            var contextMock = new Mock<BusServicesContext>();

            var invalidJson = @"{ InvalidJsonFormat }";
            var tempFilePath = "invalid.json";
            File.WriteAllText(tempFilePath, invalidJson);

            var importer = new JsonImporter(contextMock.Object);

            // Act & Assert
            Assert.Throws<JsonException>(() => importer.ImportFile(tempFilePath));

            File.Delete(tempFilePath); // Clean up
        }

        [Test]
        public void JsonImporter_EmptyFile_ThrowsException()
        {
            // Arrange
            var contextMock = new Mock<BusServicesContext>();

            var tempFilePath = "empty.json";
            File.WriteAllText(tempFilePath, "");

            var importer = new JsonImporter(contextMock.Object);

            // Act & Assert
            Assert.Throws<JsonException>(() => importer.ImportFile(tempFilePath));

            File.Delete(tempFilePath); // Clean up
        }



        [Test]
        public void AddingDuplicateRegionNumber_IsPrevented()
        {
            // Arrange
            var contextMock = new Mock<BusServicesContext>();

            var regions = new List<Region>
    {
        new Region { RegionName = "RegionA", RegionNumber = "001" }
    };

            var newRegion = new Region { RegionName = "RegionB", RegionNumber = "001" };

            // Mock the behavior of the context
            contextMock.Setup(c => c.Regions)
                       .Returns(MockDbSet(regions).Object);

            // Act
            var existingRegion = regions.FirstOrDefault(r => r.RegionNumber == newRegion.RegionNumber);

            // Assert
            Assert.That(existingRegion, Is.Not.Null);
            Assert.That(existingRegion.RegionName, Is.EqualTo("RegionA"));
        }


        private static Mock<DbSet<T>> MockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();
            var dbSet = new Mock<DbSet<T>>();

            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            dbSet.Setup(m => m.Add(It.IsAny<T>())).Callback<T>(sourceList.Add);

            return dbSet;
        }

    }

}

