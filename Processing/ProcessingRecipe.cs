using System.Collections.Generic;

namespace FarmTycoon.Processing
{
    public class ProcessingRecipe
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string OutputProductId { get; set; }
        public float OutputAmount { get; set; }
        public ProductTier OutputTier { get; set; }
        public Dictionary<string, float> Inputs { get; set; } = new();
        public float ProductionTimeHours { get; set; } = 1f;
        public float EnergyPerUnit { get; set; } = 1f;
        public float BaseQuality { get; set; } = 0.5f;
        public Dictionary<string, float> ByProducts { get; set; } = new();
        public int RequiredTechLevel { get; set; } = 0;
        public float WorkerSkillRequirement { get; set; } = 0.3f;
    }
}
