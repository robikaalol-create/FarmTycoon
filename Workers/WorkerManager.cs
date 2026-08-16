using System;
using System.Collections.Generic;
using System.Linq;
using FarmTycoon.Core;
using FarmTycoon.Farm;
using FarmTycoon.Machines;
using FarmTycoon.Utils;

namespace FarmTycoon.Workers
{
    public class WorkerManager
    {
        public List<Worker> Workers { get; private set; } = new();
        public List<WorkTask> TaskQueue { get; private set; } = new();
        public int AutomationLevel { get; private set; } = 0;

        private int _nextWorkerId = 1;
        private int _nextTaskId = 1;
        private Random _random = new Random();

        public void Initialize()
        {
            Console.WriteLine("WorkerManager inicializálva");
        }

        public Worker HireWorker(string name, WorkerType type, float offeredSalary)
        {
            float baseSalary = GetBaseSalary(type);
            if (offeredSalary < baseSalary * 0.8f)
            {
                Console.WriteLine($"{name} elutasította az ajánlatot");
                return null;
            }
            if (!GameManager.Instance.TrySpendMoney(baseSalary))
                return null;

            var worker = new Worker($"wrk_{_nextWorkerId++}", name, type, offeredSalary);
            Workers.Add(worker);
            Console.WriteLine($"{name} felvéve mint {type}, fizetés: {offeredSalary:C}");
            return worker;
        }

        public bool FireWorker(string workerId)
        {
            var worker = Workers.FirstOrDefault(w => w.Id == workerId);
            if (worker == null) return false;
            float severance = worker.CalculateSeverance();
            GameManager.Instance.TrySpendMoney(severance);
            Workers.Remove(worker);
            Console.WriteLine($"{worker.Name} elbocsátva, végkielégítés: {severance:C}");
            return true;
        }

        public WorkTask AddTask(string description, WorkerType requiredType,
            TaskPriority priority = TaskPriority.Normal, string landId = null, string machineId = null)
        {
            var task = new WorkTask($"tsk_{_nextTaskId++}", description, requiredType, landId, machineId);
            task.Priority = priority;
            TaskQueue.Add(task);
            return task;
        }

        public bool AssignTask(string taskId, string workerId = null)
        {
            var task = TaskQueue.FirstOrDefault(t => t.Id == taskId && !t.IsAssigned);
            if (task == null) return false;

            Worker worker;
            if (workerId != null)
                worker = Workers.FirstOrDefault(w => w.Id == workerId && w.IsAvailable);
            else
                worker = FindBestWorkerForTask(task);

            if (worker == null) return false;

            task.AssignedWorkerId = worker.Id;
            task.IsAssigned = true;
            worker.IsAvailable = false;
            return true;
        }

        public void ProcessDailyTasks()
        {
            if (AutomationLevel >= 2)
                AutoAssignTasks();

            foreach (var task in TaskQueue.Where(t => t.IsAssigned && !t.IsCompleted).ToList())
            {
                var worker = Workers.FirstOrDefault(w => w.Id == task.AssignedWorkerId);
                if (worker == null) continue;

                float performance = worker.GetPerformanceMultiplier();
                task.UpdateProgress(8f, performance);

                if (task.IsCompleted)
                {
                    worker.IsAvailable = true;
                    Console.WriteLine($"Feladat kész: {task.Description} ({worker.Name})");
                }
            }

            TaskQueue.RemoveAll(t => t.IsCompleted);
        }

        private void AutoAssignTasks()
        {
            var unassigned = TaskQueue.Where(t => !t.IsAssigned).OrderByDescending(t => (int)t.Priority).ToList();
            foreach (var task in unassigned)
            {
                var worker = FindBestWorkerForTask(task);
                if (worker != null)
                {
                    task.AssignedWorkerId = worker.Id;
                    task.IsAssigned = true;
                    worker.IsAvailable = false;
                }
            }
        }

        private Worker FindBestWorkerForTask(WorkTask task)
        {
            return Workers
                .Where(w => w.IsAvailable && w.Type == task.RequiredWorkerType)
                .OrderByDescending(w => w.GetPerformanceMultiplier())
                .FirstOrDefault();
        }

        public void PayDailySalaries()
        {
            float total = Workers.Sum(w => w.CurrentSalary / 30f);
            if (GameManager.Instance.TrySpendMoney(total))
            {
                foreach (var worker in Workers)
                    worker.UpdateDaily();
            }
            else
            {
                Console.WriteLine("NINCS ELÉG PÉNZ BÉREKRE! Morál csökken.");
                foreach (var worker in Workers)
                    worker.SetSalary(worker.CurrentSalary * 0.95f);
            }
        }

        public void RaiseSalary(string workerId, float percentage)
        {
            var worker = Workers.FirstOrDefault(w => w.Id == workerId);
            if (worker == null) return;
            worker.SetSalary(worker.CurrentSalary * (1f + percentage));
        }

        public List<string> GetAIRecommendations()
        {
            var recommendations = new List<string>();
            var game = GameManager.Instance;

            foreach (var parcel in game.FarmManager.LandParcels)
            {
                if (parcel.Soil.Data.Moisture < 0.2f)
                    recommendations.Add($"A {parcel.Name} nedvességtartalma kritikus. Öntözés javasolt.");
            }

            foreach (var machine in game.MachineManager.Machines)
            {
                if (machine.ConditionValue < 30f)
                    recommendations.Add($"A {machine.Name} állapota kritikus. Karbantartás szükséges.");
            }

            var overloaded = Workers.Where(w => w.IsOverworked).ToList();
            if (overloaded.Count > 0)
                recommendations.Add($"{overloaded.Count} munkás túlterhelt. További felvétel javasolt.");

            foreach (var parcel in game.FarmManager.LandParcels)
            {
                if (parcel.PlantedCrop?.CurrentStage == GrowthStage.Harvestable)
                    recommendations.Add($"A {parcel.Name} betakarítható!");
            }

            return recommendations;
        }

        public void UpgradeAutomation()
        {
            if (AutomationLevel < 4)
            {
                AutomationLevel++;
                Console.WriteLine($"Automatizálási szint: {AutomationLevel}");
            }
        }

        public (int total, float dailyCost, float avgMorale, int available) GetWorkforceSummary()
        {
            int total = Workers.Count;
            float dailyCost = Workers.Sum(w => w.CurrentSalary / 30f);
            float avgMorale = Workers.Count > 0 ? Workers.Average(w => w.Morale) : 0f;
            int available = Workers.Count(w => w.IsAvailable);
            return (total, dailyCost, avgMorale, available);
        }

        private float GetBaseSalary(WorkerType type)
        {
            return type switch
            {
                WorkerType.FarmWorker => 1500f,
                WorkerType.AnimalCaretaker => 1600f,
                WorkerType.MachineOperator => 2000f,
                WorkerType.TruckDriver => 1800f,
                WorkerType.Mechanic => 2200f,
                WorkerType.Agronomist => 2500f,
                WorkerType.Veterinarian => 2800f,
                WorkerType.LogisticsManager => 3500f,
                WorkerType.FarmManager => 4000f,
                WorkerType.AISpecialist => 5000f,
                _ => 1500f
            };
        }
    }
}
