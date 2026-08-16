using FarmTycoon.Utils;

namespace FarmTycoon.Data
{
    public class LandData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public float SizeInHectares { get; set; }
        public SoilType SoilType { get; set; }
        public float BaseFertility { get; set; } = 0.5f;
        public float Slope { get; set; } = 0f;
        public float WaterAvailability { get; set; } = 0.5f;
        public float PurchasePrice { get; set; }
        public string Region { get; set; }
    }
}
