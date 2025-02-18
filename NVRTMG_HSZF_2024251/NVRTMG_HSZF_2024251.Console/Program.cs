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
            string conString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=persondb;Integrated Security=True;MultipleActiveResultSets=true";
            try
            {
                var options = new DbContextOptionsBuilder<AppDbContext>()
                                .UseSqlServer(conString)
                                .Options;
                using var context = new AppDbContext(options);

                await context.Database.MigrateAsync();
            }
            catch (Exception)
            {
                throw new ConnectionStringException(nameof(conString) + " is not a valid connection String");
            }
            MenuFunctions.mainMenu.ShowMenu();



        }
    }
}
