using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmTycoon.Processing
{
    public class ProductionLine
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public ProcessingRecipe CurrentRecipe { get; private set; }
        public float Progress { get; private set; } = 0f;
        public bool IsRunning { get; private set; } = false;
        public float Efficiency { get; set; } = 1.0f;
        public float DailyCapacity { get; private set; } = 100f;
        public float AllocatedPercentage { get; set; } = 100f;

        public ProductionLine(string id, string name, float dailyCapacity)
        {
            Id = id;
            Name = name;
            DailyCapacity = dailyCapacity;
        }

        public void SetRecipe(ProcessingRecipe recipe)
        {
            CurrentRecipe = recipe;
            Progress = 0f;
            IsRunning = false;
        }

        public void UpdateDaily(float facilityEfficiency, float workerSkill)
        {
            if (CurrentRecipe == null || !IsRunning) return;

            float effectiveCapacity = DailyCapacity * (AllocatedPercentage / 100f) * facilityEfficiency * Efficiency * (0.5f + workerSkill * 0.5f);
            float batches = effectiveCapacity / CurrentRecipe.OutputAmount;
            Progress += batches;
        }

        public bool CanProduceBatch(Dictionary<string, float> availableInputs)
        {
            if (CurrentRecipe == null) return false;
            return CurrentRecipe.Inputs.All(input => 
                availableInputs.ContainsKey(input.Key) && availableInputs[input.Key] >= input.Value);
        }

        public (float output, Dictionary<string, float> consumed, Dictionary<string, float> byproducts) ProduceBatch()
        {
            if (CurrentRecipe == null) return (0f, new(), new());

            float qualityMultiplier = Efficiency * (0.8f + (float)new Random().NextDouble() * 0.2f);
            float actualOutput = CurrentRecipe.OutputAmount * qualityMultiplier;

            var consumed = new Dictionary<string, float>(CurrentRecipe.Inputs);
            var byproducts = new Dictionary<string, float>(CurrentRecipe.ByProducts);

            return (actualOutput, consumed, byproducts);
        }

        public void Start() => IsRunning = true;
        public void Stop() => IsRunning = false;
    }
}
