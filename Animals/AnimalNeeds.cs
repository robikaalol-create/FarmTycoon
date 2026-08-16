using System;

namespace FarmTycoon.Animals
{
    public class AnimalNeeds
    {
        public float Hunger { get; set; } = 1.0f;
        public float Thirst { get; set; } = 1.0f;
        public float Rest { get; set; } = 1.0f;
        public float TemperatureComfort { get; set; } = 1.0f;
        public float Cleanliness { get; set; } = 1.0f;
        public float Safety { get; set; } = 1.0f;
        public float Space { get; set; } = 1.0f;
        public float Social { get; set; } = 1.0f;

        public float CalculateWellbeing()
        {
            return (Hunger + Thirst + Rest + TemperatureComfort + 
                    Cleanliness + Safety + Space + Social) / 8f;
        }

        public void DecayDaily()
        {
            Hunger -= 0.3f;
            Thirst -= 0.4f;
            Rest -= 0.2f;
            Cleanliness -= 0.15f;
            Social -= 0.1f;
            ClampAll();
        }

        public void Feed(float amount)
        {
            Hunger += amount;
            ClampAll();
        }

        public void Water(float amount)
        {
            Thirst += amount;
            ClampAll();
        }

        public void RestAnimal(float amount)
        {
            Rest += amount;
            ClampAll();
        }

        public void Clean(float amount)
        {
            Cleanliness += amount;
            ClampAll();
        }

        public void SetTemperatureComfort(float temp)
        {
            TemperatureComfort = temp;
            ClampAll();
        }

        private void ClampAll()
        {
            Hunger = Clamp(Hunger, 0f, 1f);
            Thirst = Clamp(Thirst, 0f, 1f);
            Rest = Clamp(Rest, 0f, 1f);
            TemperatureComfort = Clamp(TemperatureComfort, 0f, 1f);
            Cleanliness = Clamp(Cleanliness, 0f, 1f);
            Safety = Clamp(Safety, 0f, 1f);
            Space = Clamp(Space, 0f, 1f);
            Social = Clamp(Social, 0f, 1f);
        }

        private float Clamp(float value, float min, float max) => value < min ? min : (value > max ? max : value);
    }
}
