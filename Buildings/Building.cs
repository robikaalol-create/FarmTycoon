using System;
using FarmTycoon.Core;
using FarmTycoon.Data;
using FarmTycoon.Utils;

namespace FarmTycoon.Buildings
{
    public class Building
    {
        public string InstanceId { get; private set; }
        public string BuildingDataId { get; private set; }
        public string Name { get; private set; }
        public BuildingCategory Category { get; private set; }
        public ConstructionPhase CurrentPhase { get; private set; } = ConstructionPhase.Foundation;
        public float PhaseProgress { get; private set; } = 0f;
        public BuildingCondition Condition { get; private set; } = BuildingCondition.New;
        public int CurrentLevel { get; private set; } = 1;
        public float CurrentCapacity { get; private set; }
        public float DailyMaintenanceCost { get; private set; }
        public float DailyEnergyRequirement { get; private set; }
        public bool IsCompleted => CurrentPhase == ConstructionPhase.Completed;
        public bool IsOperational => IsCompleted && Condition != BuildingCondition.Critical;

        private BuildingData _data;
        private float _conditionValue = 100f;

        public Building(string instanceId, BuildingData data)
        {
            InstanceId = instanceId;
            BuildingDataId = data.Id;
            Name = data.Name;
            Category = data.Category;
            _data = data;
            CurrentCapacity = data.BaseCapacity;
            DailyMaintenanceCost = data.DailyMaintenanceCost;
            DailyEnergyRequirement = data.DailyEnergyRequirement;
        }

        public void UpdateDaily()
        {
            if (!IsCompleted)
            {
                float phaseWorkPerDay = 1f / _data.ConstructionDays * 6f;
                PhaseProgress += phaseWorkPerDay;
                if (PhaseProgress >= 1f)
                {
                    PhaseProgress = 0f;
                    CurrentPhase = ConstructionPhaseHelper.GetNextPhase(CurrentPhase);
                    if (IsCompleted)
                        EventSystem.Instance.Publish(new BuildingConstructionCompletedEvent(InstanceId, Name));
                }
            }
            else
            {
                _conditionValue -= 0.1f;
                _conditionValue = Math.Max(0f, _conditionValue);
                UpdateConditionState();
            }
        }

        public bool Upgrade()
        {
            if (!IsCompleted || CurrentLevel >= _data.MaxLevel) return false;
            float upgradeCost = CalculateUpgradeCost();
            if (!GameManager.Instance.TrySpendMoney(upgradeCost)) return false;
            CurrentLevel++;
            CurrentCapacity = _data.BaseCapacity * MathF.Pow(_data.UpgradeCapacityMultiplier, CurrentLevel - 1);
            DailyMaintenanceCost *= 1.2f;
            EventSystem.Instance.Publish(new BuildingUpgradedEvent(InstanceId, CurrentLevel));
            return true;
        }

        public void Maintain()
        {
            _conditionValue = Math.Min(100f, _conditionValue + 30f);
            UpdateConditionState();
        }

        public float Demolish()
        {
            float recovered = _data.MaterialCost * 0.3f * (_conditionValue / 100f);
            EventSystem.Instance.Publish(new BuildingDemolishedEvent(InstanceId, recovered));
            return recovered;
        }

        public float CalculateUpgradeCost() => _data.MaterialCost * MathF.Pow(_data.UpgradeCostMultiplier, CurrentLevel);

        public float GetEffectiveCapacity()
        {
            float conditionMultiplier = Condition switch
            {
                BuildingCondition.New => 1.0f,
                BuildingCondition.Good => 0.95f,
                BuildingCondition.Fair => 0.85f,
                BuildingCondition.Poor => 0.7f,
                BuildingCondition.Worn => 0.6f,
                BuildingCondition.Critical => 0.5f,
                _ => 1.0f
            };
            return CurrentCapacity * conditionMultiplier;
        }

        private void UpdateConditionState()
        {
            Condition = _conditionValue switch
            {
                >= 90f => BuildingCondition.New,
                >= 70f => BuildingCondition.Good,
                >= 50f => BuildingCondition.Fair,
                >= 20f => BuildingCondition.Poor,
                > 0f => BuildingCondition.Worn,
                _ => BuildingCondition.Critical
            };
        }
    }
}
