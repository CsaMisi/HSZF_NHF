using System;
using System.Collections.Generic;

namespace NVRTMG_HSZF_2024251.Model;

public partial class Region
{
    public int Id { get; set; }

    public string RegionName { get; set; } = null!;

    public string RegionNumber { get; set; } = null!;

    public virtual ICollection<BusService> BusServices { get; set; } = new List<BusService>();
}
