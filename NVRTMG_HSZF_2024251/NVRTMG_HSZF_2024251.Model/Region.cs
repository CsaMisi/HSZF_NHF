namespace NVRTMG_HSZF_2024251.Model
{
    public class Region
    {
        public int Id { get; set; }
        public string RegionName { get; set; }
        public string RegionNumber { get; set; }

        public ICollection<BusService> BusServices { get; set; } = new List<BusService>();
    }
}

