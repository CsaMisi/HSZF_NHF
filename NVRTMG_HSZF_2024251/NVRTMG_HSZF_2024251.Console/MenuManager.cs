using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NVRTMG_HSZF_202425.Console
{
    public class MenuManager
    {
        private List<string> _menuItems;
        private string _title;
        private Action<int> _menuAction;
        private Action _escapeAction;

        public MenuManager(string title, ICollection<string> menuItems, Action<int> menuAction, Action escapeAction)
        {
            _title = title;
            _menuItems = menuItems.ToList();
            _menuAction = menuAction;
            _escapeAction = escapeAction;
        }

        public void ShowMenu()
        {
            int selectedIndex = 0;
            while (true)
            {
                System.Console.Clear();
                System.Console.WriteLine($"== {_title} ==");

                for (int i = 0; i < _menuItems.Count; i++)
                {
                    if (i == selectedIndex)
                    {
                        System.Console.ForegroundColor = ConsoleColor.Cyan;
                        System.Console.WriteLine($"> {_menuItems[i]}");
                        System.Console.ResetColor();
                    }
                    else
                    {
                        System.Console.WriteLine($"  {_menuItems[i]}");
                    }
                }

                var key = System.Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = selectedIndex == 0 ? _menuItems.Count - 1 : selectedIndex - 1;
                        break;

                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % _menuItems.Count;
                        break;

                    case ConsoleKey.Enter:
                        _menuAction?.Invoke(selectedIndex);
                        return;
                    case ConsoleKey.Escape:
                        _escapeAction?.Invoke();
                        return;
                }
            }
        }
    }
}
