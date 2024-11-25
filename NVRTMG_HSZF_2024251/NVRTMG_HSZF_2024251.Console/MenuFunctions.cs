using NVRTMG_HSZF_202425.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NVRTMG_HSZF_2024251.Console
{
    public class MenuFunctions
    {
        public static string[] mainMenuItems = new string[] {"Data Functions"
            , "Statistics", "Search"};
        public static MenuManager mainMenu = new MenuManager(
            "NLB manager",
            mainMenuItems,
            MenuFunctions.MainMenuAction,
            delegate { return; }
            );
        public static void MainMenuAction(int selectedIndex)
        {
            switch (selectedIndex)
            {
                //DataFunctions
                case 0:
                    var dataFunctionsItems = new string[]
                    {
                        "Load File(drag and drop)",
                        "Load File(Path)",
                        "Region Functions"
                    };
                    var DataFunctionsMenu = new MenuManager
                        ("Data Functions"
                        , dataFunctionsItems
                        , MenuFunctions.DataFunctionsAction
                        , delegate { mainMenu.ShowMenu(); }
                        );
                    DataFunctionsMenu.ShowMenu();
                    break;
                //Statistics
                case 1:
                    break;
                //Search/List
                case 2:
                    break;
                default:
                    throw new IndexOutOfRangeException(nameof(selectedIndex));
            }
        }

        static void DataFunctionsAction(int selectedIndex)
        {
            switch (selectedIndex)
            {
                case 0:

                default:
                    throw new IndexOutOfRangeException(nameof(selectedIndex));
            }
        }
    }
}
