using System;
using System.Collections.Generic;
using System.Linq;
using FarmTycoon.Core;
using FarmTycoon.Data;
using FarmTycoon.Utils;

namespace FarmTycoon.Buildings
{
    public class BuildingManager
    {
        public List<Building> Buildings { get; private set; } = new();
        public List<Infrastructure> InfrastructureItems { get; private set; } = new();
        public Dictionary<string, BuildingData> AvailableBuildings { get; private set; } = new();

        private int _nextBuildingId = 1;
        private int _nextInfrastructureId = 1;

        public void Initialize()
        {
            LoadDefaultBuildings();
            Console.WriteLine($"BuildingManager inicializálva: {AvailableBuildings.Count} épülettípus");
        }

        public Building StartConstruction(string buildingDataId)
        {
            if (!AvailableBuildings.TryGetValue(buildingDataId, out var data)) return null;
            float totalCost = data.MaterialCost + data.LaborCost;
            if (!GameManager.Instance.TrySpendMoney(totalCost)) return null;

            string instanceId = $"bld_{_nextBuildingId++}";
            var building = new Building(instanceId, data);
            Buildings.Add(building);
            EventSystem.Instance.Publish(new BuildingConstructionStartedEvent(instanceId, data.Name, totalCost));
            return building;
        }

        public bool UpgradeBuilding(string instanceId)
        {
            var building = Buildings.FirstOrDefault(b => b.InstanceId == instanceId);
            return building?.Upgrade() ?? false;
        }

        public bool DemolishBuilding(string instanceId)
        {
            var building = Buildings.FirstOrDefault(b => b.InstanceId == instanceId);
            if (building == null) return false;
            float recovered = building.Demolish();
            GameManager.Instance.AddMoney(recovered);
            Buildings.Remove(building);
            return true;
        }

        public Infrastructure PlaceInfrastructure(InfrastructureType type)
        {
            var infra = new Infrastructure($"inf_{_nextInfrastructureId++}", type);
            if (!GameManager.Instance.TrySpendMoney(infra.ConstructionCost)) return null;
            InfrastructureItems.Add(infra);
            EventSystem.Instance.Publish(new InfrastructurePlacedEvent(infra.InstanceId, type.ToString()));
            return infra;
        }

        public void UpdateDaily()
        {
            float totalMaintenance = 0f;
            foreach (var building in Buildings)
            {
                building.UpdateDaily();
                if (building.IsOperational)
                    totalMaintenance += building.DailyMaintenanceCost;
            }
            GameManager.Instance.TrySpendMoney(totalMaintenance);
            foreach (var infra in InfrastructureItems)
                GameManager.Instance.TrySpendMoney(infra.DailyMaintenanceCost);
        }

        public List<Building> GetBuildingsByCategory(BuildingCategory category) =>
            Buildings.Where(b => b.Category == category).ToList();

        public float GetTotalStorageCapacity() =>
            Buildings.Where(b => b.Category == BuildingCategory.Storage && b.IsOperational)
                     .Sum(b => b.GetEffectiveCapacity());

        private void LoadDefaultBuildings()
        {
            RegisterBuilding(new BuildingData
            {
                Id = "warehouse_small", Name = "Kis raktár", Category = BuildingCategory.Storage,
                MaterialCost = 2000f, LaborCost = 1000f, ConstructionDays = 3,
                DailyMaintenanceCost = 10f, DailyEnergyRequirement = 5f,
                BaseCapacity = 1000f, MaxLevel = 5, FootprintSize = 0.2f
            });
            RegisterBuilding(new BuildingData
            {
                Id = "silo", Name = "Siló", Category = BuildingCategory.Storage,
                MaterialCost = 3000f, LaborCost = 1500f, ConstructionDays = 5,
                DailyMaintenanceCost = 15f, DailyEnergyRequirement = 2f,
                BaseCapacity = 5000f, MaxLevel = 3, FootprintSize = 0.15f
            });
            RegisterBuilding(new BuildingData
            {
                Id = "barn", Name = "Istálló", Category = BuildingCategory.AnimalHousing,
                MaterialCost = 5000f, LaborCost = 2500f, ConstructionDays = 7,
                DailyMaintenanceCost = 25f, DailyEnergyRequirement = 10f,
                BaseCapacity = 20f, MaxLevel = 3, FootprintSize = 0.3f
            });
            RegisterBuilding(new BuildingData
            {
                Id = "mill", Name = "Malom", Category = BuildingCategory.Production,
                MaterialCost = 8000f, LaborCost = 4000f, ConstructionDays = 10,
                DailyMaintenanceCost = 40f, DailyEnergyRequirement = 30f,
                BaseCapacity = 100f, MaxLevel = 4, RequiredTechLevel = 1, FootprintSize = 0.4f
            });
            RegisterBuilding(new BuildingData
            {
                Id = "loading_dock", Name = "Rakodóállomás", Category = BuildingCategory.Logistics,
                MaterialCost = 6000f, LaborCost = 3000f, ConstructionDays = 8,
                DailyMaintenanceCost = 20f, DailyEnergyRequirement = 15f,
                BaseCapacity = 500f, MaxLevel = 3, FootprintSize = 0.25f
            });
            RegisterBuilding(new BuildingData
            {
                Id = "research_center", Name = "Kutatóközpont", Category = BuildingCategory.Special,
                MaterialCost = 15000f, LaborCost = 10000f, ConstructionDays = 15,
                DailyMaintenanceCost = 100f, DailyEnergyRequirement = 50f,
                BaseCapacity = 1f, MaxLevel = 5, RequiredTechLevel = 2,
                RequiredReputation = 50f, FootprintSize = 0.3f
            });
        }

        private void RegisterBuilding(BuildingData data) => AvailableBuildings[data.Id] = data;
    }
}
