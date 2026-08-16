namespace FarmTycoon.Data
{
    public class MachineData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public float PurchaseCost { get; set; }
        public float MaintenanceCostPerDay { get; set; }
        public float FuelConsumptionPerHour { get; set; }
        public float Efficiency { get; set; } = 1.0f;
        public float WorkSpeed { get; set; } = 10f;
        public float WorkWidth { get; set; } = 3f;
        public float FuelTankCapacity { get; set; } = 100f;
        public int RequiredTechLevel { get; set; } = 0;
    }
}
