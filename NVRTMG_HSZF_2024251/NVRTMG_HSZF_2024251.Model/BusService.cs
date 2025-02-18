using System;
using System.Collections.Generic;

namespace NVRTMG_HSZF_2024251.Model;

public partial class BusService
{
    public int Id { get; set; }

    public int RegionId { get; set; }

    public string From { get; set; } = null!;

    public string To { get; set; } = null!;

    public int BusNumber { get; set; }

    public int DelayAmount { get; set; }

    public string BusType { get; set; } = null!;

    public virtual Region Region { get; set; } = null!;
}
