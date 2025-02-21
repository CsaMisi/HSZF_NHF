using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Net.NetworkInformation;
using NVRTMG_HSZF_202425.Console;
using NVRTMG_HSZF_2024251.Application;
using NVRTMG_HSZF_2024251.Presistence.MsSql;
using NVRTMG_HSZF_2024251.Model;
using Microsoft.EntityFrameworkCore;

namespace NVRTMG_HSZF_2024251.Application
{
    public static class MenuHandleFunctions
    {
        public static readonly BusServicesContext context = DbContextFactory.CreateDbContext();

        public delegate void SmallestDelayHandler(BusService busService, Region region);
        public static event SmallestDelayHandler OnSmallestDelay;

        //Read inputs, while allowing the user to get back to the main menu by pressing esc
        public static string ReadInputWithEscape()
        {
            var input = new StringBuilder();
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);

                    if (key.Key == ConsoleKey.Escape)
                    {
                        MenuFunctions.MainMenu.ShowMenu();
                        return null;
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        break;
                    }
                    else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                    {
                        input.Remove(input.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                    else
                    {
                        Console.Write(key.KeyChar);
                        input.Append(key.KeyChar);
                    }
                }
            }
            return input.ToString();
        }

        private static int? ReadIntInputWithEscape()
        {
            var input = ReadInputWithEscape();
            return input != null && int.TryParse(input, out var result) ? result : null;
        }

        // Helper to display regions and get selected region
        private static Region GetSelectedRegion(string actionMessage, BusServicesContext context)
        {
            var regions = context.Regions.Include(r => r.BusServices).ToList();
            if (!regions.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No regions available to modify.");
                Console.ResetColor();
                return null;
            }

            Console.Clear();
            Console.WriteLine(actionMessage);
            for (int i = 0; i < regions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {regions[i].RegionName} (Region Number: {regions[i].RegionNumber})");
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            var input = ReadIntInputWithEscape();
            Console.ResetColor();

            if (input == null || input < 1 || input > regions.Count)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid selection. Returning to main menu.");
                Console.ResetColor();
                return null;
            }

            return regions[input.Value - 1];
        }

        // Helper to display bus services and get selected service
        private static BusService GetSelectedBusService(Region region, string actionMessage)
        {
            if (!region.BusServices.Any())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("This region has no bus services to modify.");
                Console.ResetColor();
                return null;
            }

            Console.Clear();
            Console.WriteLine(actionMessage);
            for (int i = 0; i < region.BusServices.Count; i++)
            {
                var service = region.BusServices.ElementAt(i);
                Console.WriteLine($"{i + 1}. {service.BusNumber} - From: {service.From}, To: {service.To}");
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            var input = ReadIntInputWithEscape();
            Console.ResetColor();

            if (input == null || input < 1 || input > region.BusServices.Count)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid selection. Returning to main menu.");
                Console.ResetColor();
                return null;
            }

            return region.BusServices.ElementAt(input.Value - 1);
        }

        public static void HandleLoadFile()
        {
            Console.Clear();
            Console.Write("Enter file path (Press ESC to return to the main menu): ");
            Console.ForegroundColor = ConsoleColor.Cyan;

            var path = ReadInputWithEscape();
            Console.ResetColor();

            if (string.IsNullOrWhiteSpace(path))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid path!");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            try
            {
                var importer = new JsonImporter(context);
                importer.ImportFile(path);

                Console.ForegroundColor = importer.AddedAnything ? ConsoleColor.Green : ConsoleColor.Yellow;
                Console.WriteLine(importer.AddedAnything ? "File imported successfully!" : "Nothing was added to the database!");
                Console.ResetColor();
                Console.ReadKey();
                MenuFunctions.MainMenu.ShowMenu();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error during import: {ex.Message}");
                Console.ResetColor();
                Console.ReadKey();
                MenuFunctions.MainMenu.ShowMenu();
            }
        }

        public static void HandleAddRegion()
        {
            Console.Clear();
            Console.WriteLine("== Add New Region ==");
            Console.WriteLine("Press ESC at any time to return to the main menu.");

            Console.Write("Enter Region Name: ");
            var regionName = ReadInputWithEscape();
            if (string.IsNullOrWhiteSpace(regionName)) return;

            Console.Write("Enter Region Number: ");
            var regionNumber = ReadInputWithEscape();
            if (string.IsNullOrWhiteSpace(regionNumber)) return;

            if (context.Regions.Any(r => r.RegionNumber == regionNumber))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Region with number '{regionNumber}' already exists. Operation cancelled.");
                Console.ResetColor();
                Console.ReadKey();
                MenuFunctions.MainMenu.ShowMenu();
                return;
            }

            context.Regions.Add(new Region { RegionName = regionName, RegionNumber = regionNumber });
            context.SaveChanges();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Region '{regionName}' added successfully!");
            Console.ResetColor();
            Console.ReadKey();
            MenuFunctions.MainMenu.ShowMenu();
        }

        public static void HandleModifyRegion()
        {
            var region = GetSelectedRegion("Select a region to modify (Press ESC to return):", context);
            if (region == null) return;

            Console.Write("Enter new Region Name (Press ESC to return): ");
            var newName = ReadInputWithEscape();
            if (string.IsNullOrWhiteSpace(newName)) return;

            region.RegionName = newName;
            context.SaveChanges();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Region name updated to '{newName}' successfully!");
            Console.ResetColor();
            Console.ReadKey();
            MenuFunctions.MainMenu.ShowMenu(); 
        }

        public static void HandleAddNewService()
        {
            var region = GetSelectedRegion("Select a region to add a service (Press ESC to return):", context);
            if (region == null) return;

            Console.Write("From: ");
            var from = ReadInputWithEscape();
            Console.Write("To: ");
            var to = ReadInputWithEscape();
            Console.Write("Bus Number: ");
            var busNumber = ReadIntInputWithEscape();
            Console.Write("Delay Amount: ");
            var delayAmount = ReadIntInputWithEscape();
            Console.Write("Bus Type: ");
            var busType = ReadInputWithEscape();

            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to) || string.IsNullOrWhiteSpace(busType) || busNumber == null || delayAmount == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid input. Operation cancelled.");
                Console.ResetColor();
                return;
            }

            var newService = new BusService
            {
                From = from,
                To = to,
                BusNumber = busNumber.Value,
                DelayAmount = delayAmount.Value,
                BusType = busType
            };

            region.BusServices.Add(newService);
            context.SaveChanges();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Bus service added successfully!");
            Console.ResetColor();

            if (region.BusServices.Min(x => x.DelayAmount) > delayAmount)
                 OnSmallestDelay?.Invoke(newService, region);



            Console.ReadKey();
            MenuFunctions.MainMenu.ShowMenu();
        }

        public static void HandleRemoveBusService()
        {
            var region = GetSelectedRegion("Select a region to remove a service (Press ESC to return):", context);
            if (region == null) return;

            var service = GetSelectedBusService(region, "Select a bus service to remove (Press ESC to return):");
            if (service == null) return;

            context.BusServices.Remove(service);
            context.SaveChanges();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Bus service '{service.BusNumber}' removed successfully!");
            Console.ResetColor();
            Console.ReadKey();
            MenuFunctions.MainMenu.ShowMenu();
        }
    }
}
