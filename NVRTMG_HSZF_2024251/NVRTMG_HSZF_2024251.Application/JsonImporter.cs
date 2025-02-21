using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using NVRTMG_HSZF_2024251.Model;
using NVRTMG_HSZF_2024251.Presistence.MsSql;

namespace NVRTMG_HSZF_2024251.Application;

public class BusRegionDto
{
    [JsonPropertyName("BusRegions")]
    public List<RegionDto> BusRegions { get; set; } = new();
}

public class RegionDto
{
    [JsonPropertyName("RegionName")]
    public string RegionName { get; set; } = string.Empty;

    [JsonPropertyName("RegionNumber")]
    public string RegionNumber { get; set; } = string.Empty;

    [JsonPropertyName("Services")]
    public List<ServiceDto> Services { get; set; } = new();
}

public class ServiceDto
{
    [JsonPropertyName("From")]
    public string From { get; set; } = string.Empty;

    [JsonPropertyName("To")]
    public string To { get; set; } = string.Empty;

    [JsonPropertyName("BusNumber")]
    public int BusNumber { get; set; }

    [JsonPropertyName("DelayAmount")]
    public int DelayAmount { get; set; }

    [JsonPropertyName("BusType")]
    public string BusType { get; set; } = string.Empty;
}

//gave up on multithreading, for some reason it refused to work and i ran out of time
public class JsonImporter
{
    private readonly BusServicesContext _context;
    private int AddedDataCounter { get; set; }

    public bool AddedAnything => AddedDataCounter > 0;

    public JsonImporter(BusServicesContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        AddedDataCounter = 0;
    }

    public void ImportFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("File path cannot be null or empty.", nameof(path));

        if (!File.Exists(path))
            throw new FileNotFoundException("File not found.", path);

        var jsonString = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<BusRegionDto>(jsonString);

        if (data?.BusRegions == null || !data.BusRegions.Any())
            throw new InvalidOperationException("No regions found in the JSON file.");

        foreach (var regionDto in data.BusRegions)
        {
            if (string.IsNullOrWhiteSpace(regionDto.RegionNumber))
                continue;

            var existingRegion = _context.Regions
                .Include(r => r.BusServices)
                .FirstOrDefault(r => r.RegionNumber == regionDto.RegionNumber);

            if (existingRegion == null)
            {
                var newRegion = new Region
                {
                    RegionName = regionDto.RegionName,
                    RegionNumber = regionDto.RegionNumber,
                    BusServices = regionDto.Services.Select(s => new BusService
                    {
                        From = s.From,
                        To = s.To,
                        BusNumber = s.BusNumber,
                        DelayAmount = s.DelayAmount,
                        BusType = s.BusType,
                    }).ToList()
                };
                AddedDataCounter++;
                _context.Regions.Add(newRegion);
                Console.WriteLine($"New region {newRegion.RegionName} added");
            }
            else
            {
                foreach (var service in regionDto.Services)
                {
                    if (!existingRegion.BusServices.Any(bs => bs.BusNumber == service.BusNumber))
                    {
                        existingRegion.BusServices.Add(new BusService
                        {
                            From = service.From,
                            To = service.To,
                            BusNumber = service.BusNumber,
                            DelayAmount = service.DelayAmount,
                            BusType = service.BusType,
                        });
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"New service added to {regionDto.RegionName}");
                        AddedDataCounter++;
                    }
                }
            }
        }

        _context.SaveChanges();
    }
}
