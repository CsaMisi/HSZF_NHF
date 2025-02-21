using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NVRTMG_HSZF_2024251.Application
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using NVRTMG_HSZF_2024251.Model;
    using NVRTMG_HSZF_2024251.Presistence.MsSql;

    public static class Statistics
    {
        public static void GenerateStatistics(string optionalPath = null)
        {
            using var context = DbContextFactory.CreateDbContext();

            var statisticsData = new StringBuilder();

            foreach (var region in context.Regions.Include(r => r.BusServices).ToList())
            {
                statisticsData.AppendLine($"Region: {region.RegionName} ({region.RegionNumber})");
                var busesWithSmallDelays = region.BusServices.Count(b => b.DelayAmount < 10);
                statisticsData.AppendLine($" - Buses with delay < 10 minutes: {busesWithSmallDelays}");
                if (region.BusServices.Any())
                {
                    var averageDelay = region.BusServices.Average(b => b.DelayAmount);
                    var leastDelayed = region.BusServices.OrderBy(b => b.DelayAmount).First();
                    var mostDelayed = region.BusServices.OrderByDescending(b => b.DelayAmount).First();

                    statisticsData.AppendLine($" - Average delay: {averageDelay:F2} minutes");
                    statisticsData.AppendLine($" - Least delayed bus: {leastDelayed.BusNumber} ({leastDelayed.DelayAmount} minutes)");
                    statisticsData.AppendLine($" - Most delayed bus: {mostDelayed.BusNumber} ({mostDelayed.DelayAmount} minutes)");
                }
                else
                {
                    statisticsData.AppendLine(" - No bus services available in this region.");
                }
                var significantDelays = region.BusServices.Where(b => b.DelayAmount > 5);
                if (significantDelays.Any())
                {
                    var mostFrequentDestination = significantDelays
                        .GroupBy(b => b.To)
                        .OrderByDescending(g => g.Count())
                        .First().Key;

                    statisticsData.AppendLine($" - Most common destination with significant delays (>5 minutes): {mostFrequentDestination}");
                }
                else
                {
                    statisticsData.AppendLine(" - No significant delays recorded.");
                }

                statisticsData.AppendLine();
            }

            SaveStatisticsToFile(statisticsData.ToString(), optionalPath);
           
        }

        private static void SaveStatisticsToFile(string content, string optionalPath)
        {
            string filePath;

            if(string.IsNullOrWhiteSpace(optionalPath))
            {
                var baseDirectory = AppContext.BaseDirectory;
                var solutionDirectory = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\..\"));
                Directory.CreateDirectory(solutionDirectory);
                filePath = Path.Combine(solutionDirectory, $"Statistics_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            }
            else
            {
                filePath = optionalPath;
            }

            try
            {
                File.WriteAllText(filePath, content);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Statistics saved successfully to: {filePath}");
                Console.ResetColor();
                Console.ReadKey();
                MenuFunctions.MainMenu.ShowMenu();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error saving statistics: {ex.Message}");
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }

}
