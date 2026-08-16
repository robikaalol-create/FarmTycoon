using System;

namespace FarmTycoon.Animals
{
    public class AnimalHealth
    {
        public float HealthValue { get; private set; } = 100f;
        public AnimalHealthStatus Status { get; private set; } = AnimalHealthStatus.Healthy;
        public bool IsPregnant { get; set; } = false;
        public int PregnancyDays { get; set; } = 0;
        public int DaysSinceLastVet { get; set; } = 0;

        private Random _random = new Random();

        public void UpdateDaily(float wellbeing, float diseaseResistance)
        {
            DaysSinceLastVet++;

            if (wellbeing < 0.5f)
                HealthValue -= (0.5f - wellbeing) * 5f;

            float infectionChance = (1f - diseaseResistance) * 0.02f;
            if (wellbeing < 0.3f)
                infectionChance *= 2f;

            if (_random.NextDouble() < infectionChance && Status == AnimalHealthStatus.Healthy)
            {
                Status = AnimalHealthStatus.Sick;
                HealthValue -= 10f;
            }

            if (Status == AnimalHealthStatus.Sick && wellbeing > 0.7f)
            {
                HealthValue += 2f;
                if (HealthValue >= 90f)
                    Status = AnimalHealthStatus.Healthy;
            }

            HealthValue = Clamp(HealthValue, 0f, 100f);
            UpdateStatus();
        }

        public void Treat(float effectiveness)
        {
            HealthValue += effectiveness;
            DaysSinceLastVet = 0;
            if (Status == AnimalHealthStatus.Sick || Status == AnimalHealthStatus.Injured)
            {
                Status = AnimalHealthStatus.Healthy;
            }
            HealthValue = Clamp(HealthValue, 0f, 100f);
        }

        public void Quarantine()
        {
            if (Status == AnimalHealthStatus.Sick)
                Status = AnimalHealthStatus.Quarantined;
        }

        private void UpdateStatus()
        {
            if (Status == AnimalHealthStatus.Quarantined && HealthValue >= 80f)
                Status = AnimalHealthStatus.Healthy;
            else if (HealthValue < 20f)
                Status = AnimalHealthStatus.Critical;
            else if (HealthValue < 50f && Status == AnimalHealthStatus.Healthy)
                Status = AnimalHealthStatus.Stressed;
        }

        private float Clamp(float value, float min, float max) => value < min ? min : (value > max ? max : value);
    }
}
