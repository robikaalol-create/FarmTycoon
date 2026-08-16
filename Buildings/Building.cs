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
                       
