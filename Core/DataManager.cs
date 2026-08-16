using System;
using System.Collections.Generic;
using FarmTycoon.Data;
using FarmTycoon.Utils;

namespace FarmTycoon.Core
{
    public class DataManager
    {
        private static DataManager _instance;
        public static DataManager Instance => _instance ??= new DataManager();

        private readonly Dictionary<string, CropData> _crops = new();
        private readonly Dictionary<string, AnimalData> _animals = new();
        private readonly Dictionary<string, LandData> _landTemplates = new();
        private readonly Dictionary<string, FertilizerData> _fertilizers = new();
        private readonly Dictionary<string, MachineData> _machines = new();

        private DataManager() { }

        public void RegisterCrop(CropData crop)
        {
            if (_crops.ContainsKey(crop.Id))
                throw new ArgumentException($"Növény már létezik: {crop.Id}");
            _crops[crop.Id] = crop;
        }

        public CropData GetCrop(string id) => _crops.TryGetValue(id, out var crop) ? crop : null;
        public IReadOnlyDictionary<string, CropData> GetAllCrops() => _crops;

        public void RegisterAnimal(AnimalData animal)
        {
            if (_animals.ContainsKey(animal.Id))
                throw new ArgumentException($"Állat már létezik: {animal.Id}");
            _animals[animal.Id] = animal;
        }

        public AnimalData GetAnimal(string id) => _animals.TryGetValue(id, out var animal) ? animal : null;
        public IReadOnlyDictionary<string, AnimalData> GetAllAnimals() => _animals;

        public void RegisterLandTemplate(LandData land)
        {
            if (_landTemplates.ContainsKey(land.Id))
                throw new ArgumentException($"Földterület már létezik: {land.Id}");
            _landTemplates[land.Id] = land;
        }

        public LandData GetLandTemplate(string id) => _landTemplates.TryGetValue(id, out var land) ? land : null;

        public void RegisterFertilizer(FertilizerData fertilizer)
        {
            if (_fertilizers.ContainsKey(fertilizer.Id))
                throw new ArgumentException($"Műtrágya már létezik: {fertilizer.Id}");
            _fertilizers[fertilizer.Id] = fertilizer;
        }

        public FertilizerData GetFertilizer(string id) => _fertilizers.TryGetValue(id, out var fertilizer) ? fertilizer : null;

        public void RegisterMachine(MachineData machine)
        {
            if (_machines.ContainsKey(machine.Id))
                throw new ArgumentException($"Gép már létezik: {machine.Id}");
            _machines[machine.Id] = machine;
        }

        public MachineData GetMachine(string id) => _machines.TryGetValue(id, out var machine) ? machine : null;

        public void LoadDefaultData()
        {
            RegisterCrop(new CropData { Id = "wheat", Name = "Búza", Season = Season.Spring, BaseProduct = "Búza", ProcessedProduct = "Liszt", GrowthDays = 10, WaterRequirement = 0.5f, NutrientRequirement = 0.4f, BaseYieldPerHectare = 8.0f, BasePrice = 200f, SeedCost = 50f });
            RegisterCrop(new CropData { Id = "corn", Name = "Kukorica", Season = Season.Summer, BaseProduct = "Kukorica", ProcessedProduct = "Takarmány", GrowthDays = 12, WaterRequirement = 0.7f, NutrientRequirement = 0.6f, BaseYieldPerHectare = 10.0f, BasePrice = 180f, SeedCost = 60f });
            RegisterCrop(new CropData { Id = "barley", Name = "Árpa", Season = Season.Spring, BaseProduct = "Árpa", ProcessedProduct = "Maláta", GrowthDays = 9, WaterRequirement = 0.4f, NutrientRequirement = 0.3f, BaseYieldPerHectare = 7.0f, BasePrice = 170f, SeedCost = 45f });
            RegisterCrop(new CropData { Id = "sunflower", Name = "Napraforgó", Season = Season.Summer, BaseProduct = "Mag", ProcessedProduct = "Olaj", GrowthDays = 11, WaterRequirement = 0.5f, NutrientRequirement = 0.5f, BaseYieldPerHectare = 3.0f, BasePrice = 400f, SeedCost = 120f });
            RegisterCrop(new CropData { Id = "potato", Name = "Burgonya", Season = Season.Spring, BaseProduct = "Burgonya", ProcessedProduct = "Hasábburgonya", GrowthDays = 8, WaterRequirement = 0.6f, NutrientRequirement = 0.5f, BaseYieldPerHectare = 25.0f, BasePrice = 150f, SeedCost = 80f });
            RegisterCrop(new CropData { Id = "tomato", Name = "Paradicsom", Season = Season.Summer, BaseProduct = "Paradicsom", ProcessedProduct = "Paradicsomszósz", GrowthDays = 7, WaterRequirement = 0.7f, NutrientRequirement = 0.6f, BaseYieldPerHectare = 35.0f, BasePrice = 120f, SeedCost = 40f });

            RegisterAnimal(new AnimalData { Id = "chicken", Name = "Tyúk", Product = "Tojás", FeedType = "Gabona", FeedAmountPerDay = 0.1f, ProductPerDay = 1.0f, BasePrice = 10f, PurchaseCost = 50f });
            RegisterAnimal(new AnimalData { Id = "cow", Name = "Tehén", Product = "Tej", FeedType = "Széna", FeedAmountPerDay = 15.0f, ProductPerDay = 25.0f, BasePrice = 0.5f, PurchaseCost = 800f });
            RegisterAnimal(new AnimalData { Id = "pig", Name = "Sertés", Product = "Hús", FeedType = "Takarmány", FeedAmountPerDay = 3.0f, ProductPerDay = 0f, BasePrice = 300f, PurchaseCost = 200f });
            RegisterAnimal(new AnimalData { Id = "sheep", Name = "Juh", Product = "Gyapjú", FeedType = "Fű", FeedAmountPerDay = 2.0f, ProductPerDay = 0.2f, BasePrice = 15f, PurchaseCost = 150f });

            RegisterFertilizer(new FertilizerData { Id = "organic", Name = "Szerves trágya", NitrogenBoost = 0.2f, PhosphorusBoost = 0.1f, PotassiumBoost = 0.1f, Cost = 30f, NegativeEffect = 0f });
            RegisterFertilizer(new FertilizerData { Id = "compost", Name = "Komposzt", NitrogenBoost = 0.15f, PhosphorusBoost = 0.15f, PotassiumBoost = 0.15f, Cost = 40f, NegativeEffect = 0f });
            RegisterFertilizer(new FertilizerData { Id = "nitrogen", Name = "Nitrogén alapú műtrágya", NitrogenBoost = 0.5f, PhosphorusBoost = 0.05f, PotassiumBoost = 0.05f, Cost = 60f, NegativeEffect = 0.1f });
            RegisterFertilizer(new FertilizerData { Id = "complex", Name = "Komplex műtrágya", NitrogenBoost = 0.3f, PhosphorusBoost = 0.3f, PotassiumBoost = 0.3f, Cost = 80f, NegativeEffect = 0.05f });

            RegisterMachine(new MachineData { Id = "tractor_small", Name = "Kis traktor", Type = "Tractor", PurchaseCost = 15000f, MaintenanceCostPerDay = 15f, FuelConsumptionPerHour = 8f, Efficiency = 0.8f, WorkSpeed = 15f, WorkWidth = 2.5f, FuelTankCapacity = 80f });
            RegisterMachine(new MachineData { Id = "tractor_large", Name = "Nagy traktor", Type = "Tractor", PurchaseCost = 45000f, MaintenanceCostPerDay = 40f, FuelConsumptionPerHour = 15f, Efficiency = 1.2f, WorkSpeed = 25f, WorkWidth = 4f, FuelTankCapacity = 200f });
            RegisterMachine(new MachineData { Id = "harvester", Name = "Kombájn", Type = "Harvester", PurchaseCost = 120000f, MaintenanceCostPerDay = 100f, FuelConsumptionPerHour = 25f, Efficiency = 1.0f, WorkSpeed = 8f, WorkWidth = 6f, FuelTankCapacity = 300f });
            RegisterMachine(new MachineData { Id = "seeder", Name = "Vetőgép", Type = "Seeder", PurchaseCost = 25000f, MaintenanceCostPerDay = 20f, FuelConsumptionPerHour = 5f, Efficiency = 1.0f, WorkSpeed = 12f, WorkWidth = 3f, FuelTankCapacity = 50f });
            RegisterMachine(new MachineData { Id = "sprayer", Name = "Permetező", Type = "Sprayer", PurchaseCost = 30000f, MaintenanceCostPerDay = 25f, FuelConsumptionPerHour = 6f, Efficiency = 1.0f, WorkSpeed = 10f, WorkWidth = 12f, FuelTankCapacity = 100f });
            RegisterMachine(new MachineData { Id = "trailer", Name = "Pótkocsi", Type = "Trailer", PurchaseCost = 8000f, MaintenanceCostPerDay = 5f, FuelConsumptionPerHour = 0f, Efficiency = 1.0f, WorkSpeed = 20f, WorkWidth = 2.5f, FuelTankCapacity = 0f });
            RegisterMachine(new MachineData { Id = "truck_light", Name = "Kisteherautó", Type = "LightTruck", PurchaseCost = 35000f, MaintenanceCostPerDay = 30f, FuelConsumptionPerHour = 12f, Efficiency = 1.0f, WorkSpeed = 60f, WorkWidth = 2f, FuelTankCapacity = 80f });
        }
    }
}
