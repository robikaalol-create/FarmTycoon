using System.Collections.Generic;

namespace FarmTycoon.Animals
{
    public class AnimalHousing
    {
        public string BuildingId { get; private set; }
        public string HousingType { get; private set; }
        public int Capacity { get; private set; }
        public List<Animal> Animals { get; private set; } = new();
        public float Cleanliness { get; private set; } = 1.0f;
        public float TemperatureControl { get; private set; } = 0.5f;

        public int Occupancy => Animals.Count;
        public bool HasSpace => Occupancy < Capacity;
        public bool IsFull => Occupancy >= Capacity;

        public AnimalHousing(string buildingId, string housingType, int capacity)
        {
            BuildingId = buildingId;
            HousingType = housingType;
            Capacity = capacity;
        }

        public bool AddAnimal(Animal animal)
        {
            if (!HasSpace) return false;
            Animals.Add(animal);
            return true;
        }

        public bool RemoveAnimal(Animal animal)
        {
            return Animals.Remove(animal);
        }

        public void UpdateDaily()
        {
            Cleanliness -= 0.05f * Occupancy / Capacity;
            if (Cleanliness < 0f) Cleanliness = 0f;
        }

        public void Clean(float amount)
        {
            Cleanliness += amount;
            if (Cleanliness > 1f) Cleanliness = 1f;
        }

        public void SetTemperatureControl(float level)
        {
            TemperatureControl = level;
        }
    }
}
