using NVRTMG_HSZF_202425.Console;
using NVRTMG_HSZF_2024251.Application;
using NVRTMG_HSZF_2024251.Presistence.MsSql;
using System;

namespace NVRTMG_HSZF_2024251.Application;

public class MenuFunctions
{
    public static readonly MenuManager MainMenu = new(
        "NLB Manager",
        new[] { "Data Functions", "Statistics", "Search" },
        MainMenuAction,
        () => Environment.Exit(0)
    );
    public static readonly MenuManager DataMenu= new(
       "Data Functions",
       new[] { "Load File", "Region Functions" },
       DataFunctionsAction,
       MainMenu.ShowMenu
   );
    public static readonly MenuManager RegionMenu = new(
       "Region Functions",
       new[] { "Update region name","Add new services to region", "Add new region", "Delete bus service from region" },
       RegionFunctionsAction,
       DataMenu.ShowMenu
   );
    public static readonly MenuManager Search = new(
       "Search",
       new[] { "List all Regions", "Search Regions by property" },
       SearcAction,
       MainMenu.ShowMenu
   );

    private static void MainMenuAction(int selectedIndex)
    {
        switch (selectedIndex)
        {
            case 0:  
                DataMenu.ShowMenu();
                break;
            case 1:
                Console.Clear();
                Console.WriteLine("== Generate Statistics ==");
                Console.Write("File path(optional): ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                var statFilePath = MenuHandleFunctions.ReadInputWithEscape();
                Statistics.GenerateStatistics(statFilePath);
                Console.ResetColor();
                break;
            case 2:
                Search.ShowMenu();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(selectedIndex));
        }
    }

    private static void DataFunctionsAction(int selectedIndex)
    {
        switch (selectedIndex)
        {
            case 0:
                MenuHandleFunctions.HandleLoadFile();
                break;
            case 1:
                RegionMenu.ShowMenu();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(selectedIndex));
        }
    }

    private static void RegionFunctionsAction(int selectedIndex)
    {
        switch (selectedIndex)
        {
            case 0:
                MenuHandleFunctions.HandleModifyRegion();
                break;
            case 1:
                MenuHandleFunctions.HandleAddNewService();
                break;
            case 2:
                MenuHandleFunctions.HandleAddRegion();
                break;
            case 3:
                MenuHandleFunctions.HandleRemoveBusService();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(selectedIndex));
        }
    }

    private static void SearcAction(int selectedIndex)
    {
        using var context = DbContextFactory.CreateDbContext();
        switch (selectedIndex)
        {
            case 0:
                SearchRegions.ListRegions(context);
                break;
            case 1:
                SearchRegions.SearchRegionsByProperties(context);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(selectedIndex));
        }
    }

}
