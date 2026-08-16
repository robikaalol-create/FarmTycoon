namespace FarmTycoon.Buildings
{
    public enum InfrastructureType
    {
        RoadDirt, RoadConcrete, RoadAsphalt, Fence, Bridge,
        WaterPipe, PowerLine, StreetLight, Canal
    }

    public class Infrastructure
    {
        public string InstanceId { get; private set; }
        public InfrastructureType Type { get; private set; }
        public float ConstructionCost { get; private set; }
        public float DailyMaintenanceCost { get; private set; }
        public float SpeedMultiplier { get; private set; } = 1.0f;
        public float FuelMultiplier { get; private set; } = 1.0f;

        public Infrastructure(string instanceId, InfrastructureType type)
        {
            InstanceId = instanceId;
            Type = type;
            (ConstructionCost, DailyMaintenanceCost, SpeedMultiplier, FuelMultiplier) = type switch
            {
                InfrastructureType.RoadDirt => (50f, 0f, 0.6f, 1.3f),
                InfrastructureType.RoadConcrete => (200f, 1f, 0.85f, 1.1f),
                InfrastructureType.RoadAsphalt => (400f, 2f, 1.0f, 1.0f),
                InfrastructureType.Fence => (30f, 0f, 1.0f, 1.0f),
                InfrastructureType.Bridge => (1000f, 5f, 0.9f, 1.05f),
                InfrastructureType.WaterPipe => (150f, 0.5f, 1.0f, 1.0f),
                InfrastructureType.PowerLine => (300f, 1f, 1.0f, 1.0f),
                InfrastructureType.StreetLight => (100f, 0.5f, 1.0f, 1.0f),
                InfrastructureType.Canal => (500f, 2f, 1.0f, 1.0f),
                _ => (100f, 0f, 1.0f, 1.0f)
            };
        }
    }
}
