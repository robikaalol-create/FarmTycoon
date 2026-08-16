using System;
using System.Collections.Generic;
using System.Linq;
using FarmTycoon.Buildings;
using FarmTycoon.Core;
using FarmTycoon.Data;
using FarmTycoon.Utils;

namespace FarmTycoon.Processing
{
    public class ProcessingFacility
    {
        public string BuildingId { get; private set; }
        public string Name { get; private set; }
        public BuildingCategory Category { get; private set; }
        public List<ProductionLine> ProductionLines { get; private set; } = new();
        public Dictionary<string, float> InputStorage { get; private set; } = new();
        public Dictionary<string, float> OutputStorage { get; private set; } = new();
        public float StorageCapacity { get; private set; } = 1000f;
        public float CurrentStorageUsed => InputStorage.Values.Sum() + OutputStorage.Values.Sum();
        public bool IsStorageFull => CurrentStorageUsed >= StorageCapacity;

        public float EnergyConsumption { get; private set; } = 0f;
        public float DailyEnergyCost { get; private set; } = 0f;
        public float EnergyPrice { get; set; } = 0.15f;

        public int TechLevel { get; private set; } = 1;
        public float BaseEfficiency { get; private set; } = 1.0f;
        public float MaintenanceCost { get; private set; } = 50f;

        public bool IsOperational => !IsStorageFull;

        public ProcessingFacility(string buildingId, string name, BuildingCategory category, float storageCapacity)
        {
            BuildingId = buildingId;
            Name = name;
            Category = category;
            StorageCapacity = storageCapacity;
        }

        public void AddProductionLine(ProductionLine line)
        {
            ProductionLines.Add(line);
        }

        public void AddInput(string productId, float amount)
        {
            if (!InputStorage.ContainsKey(productId))
                InputStorage[productId] = 0f;
            InputStorage[productId] += amount;
        }

        public bool RemoveInput(string productId, float amount)
        {
            if (!InputStorage.ContainsKey(productId) || InputStorage[productId] < amount)
                return false;
            InputStorage[productId] -= amount;
            if (InputStorage[productId] <= 0)
                InputStorage.Remove(productId);
            return true;
        }

        public void AddOutput(string productId, float amount)
        {
            if (!OutputStorage.ContainsKey(productId))
                OutputStorage[productId] = 0f;
            OutputStorage[productId] += amount;
        }

        public float RemoveOutput(string productId, float amount)
        {
            if (!OutputStorage.ContainsKey(productId)) return 0f;
            float removed = Math.Min(amount, OutputStorage[productId]);
            OutputStorage[productId] -= removed;
            if (OutputStorage[productId] <= 0)
                OutputStorage.Remove(productId);
            return removed;
        }

        public void UpdateDaily(float workerSkill)
        {
            if (!IsOperational) return;

            float totalEnergy = 0f;
            foreach (var line in ProductionLines.Where(l => l.IsRunning && l.CurrentRecipe != null))
            {
                if (line.CanProduceBatch(InputStorage))
                {
                    var (output, consumed, byproducts) = line.ProduceBatch();
                    foreach (var input in consumed)
                    {
                        RemoveInput(input.Key, input.Value);
                    }

                    AddOutput(line.CurrentRecipe.OutputProductId, output);

                    foreach (var byproduct in byproducts)
                    {
                        AddOutput(byproduct.Key, byproduct.Value);
                    }

                    totalEnergy += line.CurrentRecipe.EnergyPerUnit * output;
                }
                else
                {
                    line.Stop();
                }

                line.UpdateDaily(BaseEfficiency, workerSkill);
            }

            EnergyConsumption = totalEnergy;
            DailyEnergyCost = totalEnergy * EnergyPrice;
            GameManager.Instance.TrySpendMoney(DailyEnergyCost + MaintenanceCost);
        }

        public void UpgradeTech()
        {
            if (GameManager.Instance.TrySpendMoney(5000f * TechLevel))
            {
                TechLevel++;
                BaseEfficiency += 0.1f;
                foreach (var line in ProductionLines)
                    line.Efficiency += 0.05f;
            }
        }

        public (float totalInput, float totalOutput, float capacityUsed) GetStorageSummary()
        {
            return (InputStorage.Values.Sum(), OutputStorage.Values.Sum(), CurrentStorageUsed / StorageCapacity);
        }
    }
}
