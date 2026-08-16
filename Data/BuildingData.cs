using System.Collections.Generic;

namespace FarmTycoon.Data
{
    public class BuildingData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public BuildingCategory Category { get; set; }
        public float MaterialCost { get; set; }
        public float LaborCost { get; set; }
        public int ConstructionDays { get; set; }
        public float DailyMaintenanceCost { get; set; }
        public float DailyEnergyRequirement { get; set; }
        public float BaseCapacity { get; set; }
        public int MaxLevel { get; set; } = 1;
        public float UpgradeCostMultiplier { get; set; } = 1.5f;
        public float UpgradeCapacityMultiplier { get; set; } = 1.8f;
        public int RequiredTechLevel { get; set; } = 0;
        public float RequiredReputation { get; set; } = 0f;
        public List<string> Prerequisites { get; set; } = new();
        public float FootprintSize { get; set; } = 0.1f;
    }
}
