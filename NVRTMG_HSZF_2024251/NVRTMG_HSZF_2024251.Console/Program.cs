using Microsoft.EntityFrameworkCore;
using NVRTMG_HSZF_202425.Console;
using NVRTMG_HSZF_2024251.Application.Exceptions;
using NVRTMG_HSZF_2024251.Console;
using NVRTMG_HSZF_2024251.Presistence.MsSql;
using System;
using System.Runtime.CompilerServices;

namespace NVRTMG_HSZF_2024251
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string conString = "Server=(localdb)\\MSSQLLocalDB;Database=BusServices;Trusted_Connection=True;";
            var options = new DbContextOptionsBuilder<BusServicesContext>()
                            .UseSqlServer(conString)
                            .Options;
            using var context = new BusServicesContext(options);
            await context.Database.MigrateAsync();
            MenuFunctions.mainMenu.ShowMenu();
            
        }
    }
}
