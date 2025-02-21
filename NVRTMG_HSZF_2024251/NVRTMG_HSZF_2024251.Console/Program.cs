using Microsoft.EntityFrameworkCore;
using NVRTMG_HSZF_2024251.Application;
using NVRTMG_HSZF_2024251.Presistence.MsSql;

namespace NVRTMG_HSZF_2024251.Console;

internal class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            using (var context = DbContextFactory.CreateDbContext())
            {
                System.Console.WriteLine("Migrating database...");
                await context.Database.MigrateAsync();
            }

            MenuHandleFunctions.OnSmallestDelay += (busService, region) =>
            {
                System.Console.ForegroundColor = ConsoleColor.Cyan;
                System.Console.WriteLine($"New smallest delay detected! Service {busService.BusNumber} in region {region.RegionName} has the smallest delay of {busService.DelayAmount} minutes.");
                System.Console.ResetColor();
            };

            RunApplication();
        }
        catch (Exception ex)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"Fatal error: {ex.Message}");
            System.Console.ResetColor();
        }
    }

    private static void RunApplication()
    {
        MenuFunctions.MainMenu.ShowMenu();
    }
}

