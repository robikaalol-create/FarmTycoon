using FarmTycoon.Utils;

namespace FarmTycoon.Data
{
    public class CropData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Season Season { get; set; }
        public string BaseProduct { get; set; }
        public string ProcessedProduct { get; set; }
        public int GrowthDays { get; set; }
        public float WaterRequirement { get; set; }
        public float NutrientRequirement { get; set; }
        public float BaseYieldPerHectare { get; set; }
        public float BasePrice { get; set; }
        public float SeedCost { get; set; }
        public float OptimalTemperature { get; set; } = 20f;
        public float WeedSensitivity { get; set; } = 0.3f;
        public float PestSensitivity { get; set; } = 0.3f;
        public float DiseaseSensitivity { get; set; } = 0.2f;
    }
}
