using System;
using System.Collections.Generic;
using System.Linq;
using FarmTycoon.Core;
using FarmTycoon.Data;

namespace FarmTycoon.Machines
{
    public class MachineManager
    {
        public List<Machine> Machines { get; private set; } = new();
        public Dictionary<string, List<Machine>> Fleets { get; private set; } = new();
        public float FuelPrice { get; private set; } = 1.5f;

        private int _nextMachineId = 1;

        public void Initialize()
        {
            Console.WriteLine("MachineManager inicializálva");
        }

        public Machine PurchaseMachine(string machineDataId, MachineType type, FuelType fuel, bool isUsed = false)
        {
            var data = DataManager.Instance.GetMachine(machineDataId);
            if (data == null) return null;

            float price = isUsed ? data.PurchaseCost * 0.6f : data.PurchaseCost;
            if (!GameManager.Instance.TrySpendMoney(price))
                return null;

            var machine = new Machine($"mch_{_nextMachineId++}", data, type, fuel);

            if (isUsed)
            {
                machine.GetType().GetProperty("ConditionValue")?.SetValue(machine, 60f + new Random().Next(30));
            }

            Machines.Add(machine);
            return machine;
        }

        public bool SellMachine(string instanceId)
        {
            var machine = Machines.FirstOrDefault(m => m.InstanceId == instanceId);
            if (machine == null) return false;

            GameManager.Instance.AddMoney(machine.CalculateValue());
            Machines.Remove(machine);
            return true;
        }

        public bool MaintainMachine(string instanceId)
        {
            var machine = Machines.FirstOrDefault(m => m.InstanceId == instanceId);
            if (machine == null) return false;

            float cost = machine.MaintenanceCost;
            if (!GameManager.Instance.TrySpendMoney(cost))
                return false;

            machine.Maintain();
            return true;
        }

        public bool RepairMachine(string instanceId)
        {
            var machine = Machines.FirstOrDefault(m => m.InstanceId == instanceId);
            if (machine == null || !machine.IsBroken) return false;

            float repairCost = machine.CalculateValue() * 0.3f;
            if (!GameManager.Instance.TrySpendMoney(repairCost))
                return false;

            machine.Repair();
            return true;
        }

        public bool RefuelMachine(string instanceId, float liters)
        {
            var machine = Machines.FirstOrDefault(m => m.InstanceId == instanceId);
            if (machine == null) return false;

            float cost = liters * FuelPrice;
            if (!GameManager.Instance.TrySpendMoney(cost))
                return false;

            machine.Refuel(liters, FuelPrice);
            return true;
        }

        public bool InstallGPS(string instanceId)
        {
            var machine = Machines.FirstOrDefault(m => m.InstanceId == instanceId);
            if (machine == null || machine.HasGPS) return false;

            if (!GameManager.Instance.TrySpendMoney(5000f))
                return false;

            machine.InstallGPS();
            return true;
        }

        public void CreateFleet(string fleetName, List<string> machineIds)
        {
            var fleet = Machines.Where(m => machineIds.Contains(m.InstanceId)).ToList();
            Fleets[fleetName] = fleet;
        }

        public void AssignFleetTask(string fleetName, MachineTaskType task, string landId)
        {
            if (!Fleets.ContainsKey(fleetName)) return;

            foreach (var machine in Fleets[fleetName])
            {
                if (!machine.IsBroken && machine.FuelLevel > 10f)
                {
                    machine.AssignTask(task, landId);
                }
            }
        }

        public void UpdateDaily()
        {
            foreach (var machine in Machines)
            {
                if (machine.CurrentTask != MachineTaskType.Idle)
                {
                    machine.DoWork(8f, 10f);
                }
                else
                {
                    machine.UpdateDaily(0.5f);
                }
            }
        }

        public (int total, int operational, int broken, float avgCondition, float totalValue) GetFleetSummary()
        {
            int total = Machines.Count;
            int operational = Machines.Count(m => !m.IsBroken);
            int broken = Machines.Count(m => m.IsBroken);
            float avgCondition = Machines.Count > 0 ? Machines.Average(m => m.ConditionValue) : 0f;
            float totalValue = Machines.Sum(m => m.CalculateValue());

            return (total, operational, broken, avgCondition, totalValue);
        }

        public List<Machine> GetMachinesByType(MachineType type)
        {
            return Machines.Where(m => m.Type == type).ToList();
        }
    }
}
