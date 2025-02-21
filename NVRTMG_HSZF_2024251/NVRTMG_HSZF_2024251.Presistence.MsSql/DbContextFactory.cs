using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NVRTMG_HSZF_2024251.Presistence.MsSql
{
    public class DbContextFactory
    {
        private static string _connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=BusServices;Trusted_Connection=True;";

        public static BusServicesContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<BusServicesContext>()
                .UseSqlServer(_connectionString)
                .Options;

            return new BusServicesContext(options);
        }
    }
}
