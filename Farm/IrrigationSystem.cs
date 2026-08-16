namespace FarmTycoon.Farm
{
    public class IrrigationSystem
    {
        public enum IrrigationType
        {
            Manual, Well, Tank, Canal, Sprinkler, Drip, Smart
        }

        public IrrigationType Type { get; private set; }
        public float Efficiency { get; private set; }
        public float WaterConsumption { get; private set; }
        public float DailyCost { get; private set; }

        public IrrigationSystem(IrrigationType type)
        {
            Type = type;
            (Efficiency, WaterConsumption, DailyCost) = type switch
            {
                IrrigationType.Manual => (0.3f, 1.0f, 0f),
                IrrigationType.Well => (0.5f, 0.8f, 2f),
                IrrigationType.Tank => (0.6f, 0.7f, 1f),
                IrrigationType.Canal => (0.7f, 0.6f, 0.5f),
                IrrigationType.Sprinkler => (0.75f, 0.5f, 5f),
                IrrigationType.Drip => (0.9f, 0.3f, 8f),
                IrrigationType.Smart => (0.95f, 0.2f, 15f),
                _ => (0.3f, 1.0f, 0f)
            };
        }

        public void Irrigate(Soil soil)
        {
            soil.Data.Moisture += Efficiency * 0.3f;
            if (soil.Data.Moisture > 1f) soil.Data.Moisture = 1f;
        }
    }
}
