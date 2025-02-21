using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NVRTMG_HSZF_2024251.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using NVRTMG_HSZF_2024251.Model;
    using NVRTMG_HSZF_2024251.Presistence.MsSql;

    public static class SearchRegions
    {
        public static void ListRegions(BusServicesContext context)
        {
            Console.Clear();
            Console.ResetColor();
            var regions = context.Regions.Include(r => r.BusServices).ToList();
            if (!regions.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No regions found.");
                Console.ResetColor();
                return;
            }
            Console.WriteLine("Available Regions:");
            foreach (var region in regions)
            {
                Console.WriteLine($" - {region.RegionName} ({region.RegionNumber}), Bus Services: {region.BusServices.Count}");
            }
            Console.ResetColor();
            Console.ReadKey();
            MenuFunctions.MainMenu.ShowMenu();
        }

        public static void SearchRegionsByProperties(BusServicesContext context)
        {
            while (true)
            {
                Console.Clear();
                Console.ResetColor();
                Console.WriteLine("== Search Regions ==");
                Console.WriteLine("Leave fields empty if you don't want to filter by that property.");
                Console.WriteLine("Press ESC at any time to cancel and return to the main menu.");

                Console.Write("Region Name: ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                var regionName = MenuHandleFunctions.ReadInputWithEscape();
                if (regionName == null) return;
                Console.ResetColor();

                Console.Write("Region Number: ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                var regionNumber = MenuHandleFunctions.ReadInputWithEscape();
                if (regionNumber == null) return;
                Console.ResetColor();

                Console.Write("Minimum Bus Services: ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                var minBusServicesInput = MenuHandleFunctions.ReadInputWithEscape();
                if (minBusServicesInput == null) return; 
                Console.ResetColor();

                
                int? minBusServices = null;
                if (int.TryParse(minBusServicesInput, out var parsedMinBusServices))
                {
                    minBusServices = parsedMinBusServices;
                }
             
                var query = context.Regions.Include(r => r.BusServices).AsQueryable();

                if (!string.IsNullOrWhiteSpace(regionName))
                {
                    query = query.Where(r => EF.Functions.Like(r.RegionName, $"%{regionName}%"));
                }

                if (!string.IsNullOrWhiteSpace(regionNumber))
                {
                    query = query.Where(r => r.RegionNumber == regionNumber);
                }

                if (minBusServices.HasValue)
                {
                    query = query.Where(r => r.BusServices.Count >= minBusServices.Value);
                }

                var results = query.ToList();

                Console.Clear();
                if (!results.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("No regions matched the search criteria.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Found {results.Count} matching regions:");
                    foreach (var region in results)
                    {
                        Console.WriteLine($" - {region.RegionName} ({region.RegionNumber}), Bus Services: {region.BusServices.Count}");
                    }
                    Console.ResetColor();
                }

                Console.WriteLine("\nPress any key to perform another search or ESC to cancel.");
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine("\nSearch cancelled. Returning to main menu.");
                    return;
                }
            }
        }


    }

}
