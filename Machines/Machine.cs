using System;
using FarmTycoon.Data;

namespace FarmTycoon.Machines
{
    public class Machine
    {
        public string InstanceId { get; private set; }
        public string MachineDataId { get; private set; }
        public string Name { get; private set; }
        public MachineType Type { get; private set; }
        public FuelType FuelType { get; private set; }

        public float ConditionValue { get; private set; } = 100f;
        public MachineCondition Condition => GetConditionState();
        public int OperatingHours { get; private set; } = 0;
        public bool IsBroken { get; private set; } = false;

        public float FuelLevel { get; private set; } = 100f;
        public float FuelTankCapacity { get; private set; } = 100f;
        public float CurrentFuelLiters => FuelTankCapacity * (FuelLevel / 100f);

        public float Efficiency { get; private set; } = 1.0f;
        public float WorkSpeed { get; private set; } = 10f;
        public float WorkWidth { get; private set; } = 3f;

        public MachineTaskType CurrentTask { get; private set; } = MachineTaskType.Idle;
        public float TaskProgress { get; private set; } = 0f;
        public string AssignedLandId { get; private set; }

        public int HoursSinceLastMaintenance { get; private set; } = 0;
        public float MaintenanceCost { get; private set; } = 50f;

        public bool HasGPS { get; private set; } = false;
        public bool HasAutoSteer { get; private set; } = false;
        public bool IsAutonomous { get; private set; } = false;

        private MachineData _data;
        private Random _random = new Random();

        public Machine(string instanceId, MachineData data, MachineType type, FuelType fuel)
        {
            InstanceId = instanceId;
            MachineDataId = data.Id;
            Name = data.Name;
            Type = type;
            FuelType = fuel;
            _data = data;
            Efficiency = data.Efficiency;
            MaintenanceCost = data.MaintenanceCostPerDay * 5f;
        }

        public void UpdateDaily(float hoursWorked)
        {
            if (IsBroken) return;

            OperatingHours += (int)hoursWorked;
            HoursSinceLastMaintenance += (int)hoursWorked;

            float fuelConsumed = _data.FuelConsumptionPerHour * hoursWorked;
            FuelLevel -= (fuelConsumed / FuelTankCapacity) * 100f;
            if (FuelLevel < 0f) FuelLevel = 0f;

            float wearRate = 0.05f;
            if (HoursSinceLastMaintenance > 50) wearRate *= 2f;
            if (HoursSinceLastMaintenance > 100) wearRate *= 3f;

            ConditionValue -= wearRate * hoursWorked;
            ConditionValue = Math.Max(0f, ConditionValue);

            Efficiency = _data.Efficiency * (0.5f + ConditionValue / 200f);

            CheckBreakdown();
        }

        public float DoWork(float hours, float areaHectares)
        {
            if (IsBroken || FuelLevel <= 0f) return 0f;

            float fuelNeeded = _data.FuelConsumptionPerHour * hours;
            if (CurrentFuelLiters < fuelNeeded)
            {
                hours = CurrentFuelLiters / _data.FuelConsumptionPerHour;
            }

            UpdateDaily(hours);

            float workDone = areaHectares * Efficiency;
            return workDone;
        }

        public void Refuel(float liters, float costPerLiter)
        {
            float needed = FuelTankCapacity - CurrentFuelLiters;
            float toAdd = Math.Min(liters, needed);
            FuelLevel += (toAdd / FuelTankCapacity) * 100f;
        }

        public void Maintain()
        {
            ConditionValue = Math.Min(100f, ConditionValue + 30f);
            HoursSinceLastMaintenance = 0;
            IsBroken = false;
            Efficiency = _data.Efficiency;
        }

        public void Repair()
        {
            IsBroken = false;
            ConditionValue = Math.Max(ConditionValue, 20f);
        }

        public void InstallGPS()
        {
            HasGPS = true;
            WorkSpeed *= 1.1f;
        }

        public void EnableAutonomy()
        {
            if (HasGPS && HasAutoSteer)
                IsAutonomous = true;
        }

        public void AssignTask(MachineTaskType task, string landId = null)
        {
            CurrentTask = task;
            AssignedLandId = landId;
            TaskProgress = 0f;
        }

        public void CompleteTask()
        {
            CurrentTask = MachineTaskType.Idle;
            AssignedLandId = null;
            TaskProgress = 0f;
        }

        public float CalculateValue()
        {
            float baseValue = _data.PurchaseCost;
            float conditionMultiplier = ConditionValue / 100f;
            float ageMultiplier = Math.Max(0.3f, 1f - (OperatingHours / 10000f));
            return baseValue * conditionMultiplier * ageMultiplier;
        }

        private void CheckBreakdown()
        {
            if (IsBroken) return;

            float breakdownChance = 0f;
            if (ConditionValue < 20f) breakdownChance = 0.1f;
            else if (ConditionValue < 40f) breakdownChance = 0.05f;
            else if (ConditionValue < 60f) breakdownChance = 0.02f;

            if (HoursSinceLastMaintenance > 100) breakdownChance += 0.05f;

            if (_random.NextDouble() < breakdownChance)
            {
                IsBroken = true;
                CurrentTask = MachineTaskType.Idle;
            }
        }

        private MachineCondition GetConditionState()
        {
            return ConditionValue switch
            {
                >= 80f => MachineCondition.New,
                >= 60f => MachineCondition.Good,
                >= 40f => MachineCondition.Fair,
                >= 20f => MachineCondition.Poor,
                > 0f => MachineCondition.Worn,
                _ => MachineCondition.Critical
            };
        }
    }
}
