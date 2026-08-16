using System;
using System.Collections.Generic;
using System.Linq;
using FarmTycoon.Buildings;
using FarmTycoon.Core;
using FarmTycoon.Data;

namespace FarmTycoon.Processing
{
    public class ProcessingManager
    {
        public List<ProcessingFacility> Facilities { get; private set; } = new();
        public Dictionary<string, ProcessingRecipe> Recipes { get; private set; } = new();
        public Dictionary<string, float> GlobalInventory { get; private set; } = new();

        public float TotalStorageCapacity { get; private set; } = 0f;
        public float TotalStorageUsed => GlobalInventory.Values.Sum();

        public void Initialize()
        {
            LoadDefaultRecipes();
            Console.WriteLine($"ProcessingManager inicializálva: {Recipes.Count} recept");
        }

        public ProcessingFacility CreateFacility(string buildingId, string name, BuildingCategory category, float storageCapacity)
        {
            var facility = new ProcessingFacility(buildingId, name, category, storageCapacity);
            Facilities.Add(facility);
            TotalStorageCapacity += storageCapacity;
            return facility;
        }

        public void RegisterRecipe(ProcessingRecipe recipe)
        {
            Recipes[recipe.Id] = recipe;
        }

        public ProcessingRecipe GetRecipe(string id)
        {
            Recipes.TryGetValue(id, out var recipe);
            return recipe;
        }

        public void UpdateDaily(float averageWorkerSkill)
        {
            foreach (var facility in Facilities)
            {
                facility.UpdateDaily(averageWorkerSkill);

                foreach (var output in facility.OutputStorage)
                {
                    AddToGlobalInventory(output.Key, output.Value);
                }
                facility.OutputStorage.Clear();
            }
        }

        public void AddToGlobalInventory(string productId, float amount)
        {
            if (!GlobalInventory.ContainsKey(productId))
                GlobalInventory[productId] = 0f;
            GlobalInventory[productId] += amount;
        }

        public bool RemoveFromGlobalInventory(string productId, float amount)
        {
            if (!GlobalInventory.ContainsKey(productId) || GlobalInventory[productId] < amount)
                return false;
            GlobalInventory[productId] -= amount;
            if (GlobalInventory[productId] <= 0)
                GlobalInventory.Remove(productId);
            return true;
        }

        public float GetInventoryValue(Dictionary<string, float> prices)
        {
            float total = 0f;
            foreach (var item in GlobalInventory)
            {
                if (prices.ContainsKey(item.Key))
                    total += item.Value * prices[item.Key];
            }
            return total;
        }

        public (int facilities, int lines, float inventoryValue, float dailyOutput) GetProductionSummary()
        {
            int facilities = Facilities.Count;
            int lines = Facilities.Sum(f => f.ProductionLines.Count);
            float inventoryValue = GlobalInventory.Values.Sum();
            float dailyOutput = Facilities.Sum(f => f.OutputStorage.Values.Sum());
            return (facilities, lines, inventoryValue, dailyOutput);
        }

        private void LoadDefaultRecipes()
        {
            RegisterRecipe(new ProcessingRecipe
            {
                Id = "wheat_to_flour",
                Name = "Búza őrlése",
                OutputProductId = "flour",
                OutputAmount = 80f,
                OutputTier = ProductTier.Intermediate,
                Inputs = new Dictionary<string, float> { { "wheat", 100f } },
                ProductionTimeHours = 2f,
                EnergyPerUnit = 5f,
                ByProducts = new Dictionary<string, float> { { "bran", 15f } }
            });

            RegisterRecipe(new ProcessingRecipe
            {
                Id = "flour_to_bread",
                Name = "Kenyér sütése",
                OutputProductId = "bread",
                OutputAmount = 120f,
                OutputTier = ProductTier.Finished,
                Inputs = new Dictionary<string, float> { { "flour", 100f }, { "water", 30f } },
                ProductionTimeHours = 4f,
                EnergyPerUnit = 8f
            });

            RegisterRecipe(new ProcessingRecipe
            {
                Id = "milk_to_cheese",
                Name = "Sajt készítése",
                OutputProductId = "cheese",
                OutputAmount = 15f,
                OutputTier = ProductTier.Finished,
                Inputs = new Dictionary<string, float> { { "milk", 100f } },
                ProductionTimeHours = 12f,
                EnergyPerUnit = 10f,
                ByProducts = new Dictionary<string, float> { { "whey", 80f } }
            });

            RegisterRecipe(new ProcessingRecipe
            {
                Id = "sunflower_to_oil",
                Name = "Olaj préselése",
                OutputProductId = "oil",
                OutputAmount = 35f,
                OutputTier = ProductTier.Intermediate,
                Inputs = new Dictionary<string, float> { { "sunflower", 100f } },
                ProductionTimeHours = 3f,
                EnergyPerUnit = 6f,
                ByProducts = new Dictionary<string, float> { { "oil_cake", 60f } }
            });

            RegisterRecipe(new ProcessingRecipe
            {
                Id = "feed_production",
                Name = "Takarmány gyártása",
                OutputProductId = "feed",
                OutputAmount = 90f,
                OutputTier = ProductTier.Intermediate,
                Inputs = new Dictionary<string, float> { { "corn", 50f }, { "wheat", 30f }, { "bran", 20f } },
                ProductionTimeHours = 2f,
                EnergyPerUnit = 4f
            });

            RegisterRecipe(new ProcessingRecipe
            {
                Id = "wool_to_yarn",
                Name = "Gyapjú feldolgozása",
                OutputProductId = "yarn",
                OutputAmount = 85f,
                OutputTier = ProductTier.Intermediate,
                Inputs = new Dictionary<string, float> { { "wool", 100f } },
                ProductionTimeHours = 5f,
                EnergyPerUnit = 7f
            });

            RegisterRecipe(new ProcessingRecipe
            {
                Id = "premium_cheese",
                Name = "Prémium sajt érlelése",
                OutputProductId = "premium_cheese",
                OutputAmount = 12f,
                OutputTier = ProductTier.Premium,
                Inputs = new Dictionary<string, float> { { "milk", 100f } },
                ProductionTimeHours = 48f,
                EnergyPerUnit = 15f,
                RequiredTechLevel = 2,
                WorkerSkillRequirement = 0.7f
            });
        }
    }
}
