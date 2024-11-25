using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NVRTMG_HSZF_2024251.Model
{
    public  class BusService
    {
        public int Id { get; set; }
        public int RegionId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int BusNumber { get; set; }
        public int DelayAmount { get; set; }
        public string BusType { get; set; }

        public Region Region { get; set; }
    }
}
