using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NVRTMG_HSZF_2024251.Presistence.MsSql;
using System.Runtime.InteropServices;
using NVRTMG_HSZF_2024251.Model;

namespace NVRTMG_HSZF_2024251.Application
{
    public class BusRegionDto
    {
        public List<RegionDto> BusRegions { get; set; }
    }

    public class RegionDto
    {
        public string RegionName { get; set; }
        public string RegionNumber { get; set; }
        public List<ServiceDto> Services { get; set; }
    }

    public class ServiceDto
    {
        public string From { get; set; }
        public string To { get; set; }
        public int BusNumber { get; set; }
        public int DelayAmount { get; set; }
        public string BusType { get; set; }
    }
    public class JsonImporter
    {
        private readonly BusServicesContext context;

        public JsonImporter(BusServicesContext context)
        {
            this.context = context;
        }

        public async Task ImportFile(string path)
        {
            var jsonString = await File.ReadAllTextAsync(path);
            var data = JsonSerializer.Deserialize<BusRegionDto>(jsonString);

            foreach (var item in data.BusRegions)
            {
                var existingRegion = await context.Regions.Include(x => x.BusServices)
                    .FirstOrDefaultAsync(x => x.RegionNumber == item.RegionNumber);

                if(existingRegion is null)
                {
                    var newRegion = new Region
                    {
                        RegionName = item.RegionName,
                        RegionNumber = item.RegionNumber,
                        BusServices = item.Services.Select(x =>
                        new BusService
                        {
                            From = x.From,
                            To = x.To,
                            DelayAmount = x.DelayAmount,
                            BusType = x.BusType,
                        }).ToList()
                    };
                    context.Regions.Add(newRegion);
                }
                else
                {
                    foreach (var sercviceDto in item.Services)
                    {
                        if(!existingRegion.BusServices.Any(x=>
                        x.BusNumber == sercviceDto.BusNumber))
                        {
                            existingRegion.BusServices.Add(new BusService
                            {
                                From = sercviceDto.From,
                                To = sercviceDto.To,
                                BusNumber = sercviceDto.BusNumber,
                                DelayAmount= sercviceDto.DelayAmount,
                                BusType= sercviceDto.BusType
                            });
                        }
                    }
                }
                await context.SaveChangesAsync();
            }
        }
    }


}
