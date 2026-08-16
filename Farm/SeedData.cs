using FarmTycoon.Data;

namespace FarmTycoon.Farm
{
    public enum SeedQuality
    {
        Basic, Good, Premium, Professional, Special
    }

    public class SeedData
    {
        public string CropId { get; set; }
        public SeedQuality Quality { get; set; }
        public float CostMultiplier { get; set; }
        public float YieldBonus { get; set; }
        public float DiseaseResistance { get; set; }
        public float QualityBonus { get; set; }

        public SeedData(string cropId, SeedQuality quality)
        {
            CropId = cropId;
            Quality = quality;
            (CostMultiplier, YieldBonus, DiseaseResistance, QualityBonus) = quality switch
            {
                SeedQuality.Basic => (1.0f, 0.0f, 0.0f, 0.0f),
                SeedQuality.Good => (1.3f, 0.1f, 0.1f, 0.1f),
                SeedQuality.Premium => (1.8f, 0.2f, 0.2f, 0.2f),
                SeedQuality.Professional => (2.5f, 0.35f, 0.3f, 0.3f),
                SeedQuality.Special => (4.0f, 0.5f, 0.5f, 0.4f),
                _ => (1.0f, 0.0f, 0.0f, 0.0f)
            };
        }

        public float GetSeedCost(CropData cropData) => cropData.SeedCost * CostMultiplier;
    }
}
