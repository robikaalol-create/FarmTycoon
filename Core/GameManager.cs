using System;
using System.Linq;
using FarmTycoon.Animals;
using FarmTycoon.Buildings;
using FarmTycoon.Farm;
using FarmTycoon.Machines;
using FarmTycoon.Processing;
using FarmTycoon.Utils;
using FarmTycoon.Workers;

namespace FarmTycoon.Core
{
    public class GameManager
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance ??= new GameManager();

        public TimeManager TimeManager { get; private set; }
        public EventSystem EventSystem { get; private set; }
        public DataManager DataManager { get; private set; }
        public FarmManager FarmManager { get; private set; }
        public BuildingManager BuildingManager { get; private set; }
        public AnimalManager AnimalManager { get; private set; }
        public MachineManager MachineManager { get; private set; }
        public WorkerManager WorkerManager { get; private set; }
        public ProcessingManager ProcessingManager { get; private set; }

        public float PlayerMoney { get; private set; } = 10000f;
        public float Reputation { get; private set; } = 0f;

        public event Action<float> OnMoneyChanged;
        public event Action<float> OnReputationChanged;

        private GameManager() { }

        public void Initialize()
        {
            Console.WriteLine("=== Farm Tycoon - Játék inicializálása ===");

            EventSystem = EventSystem.Instance;
            TimeManager = new TimeManager();
            DataManager = DataManager.Instance;
            FarmManager = new FarmManager();
            BuildingManager = new BuildingManager();
            AnimalManager = new AnimalManager();

            DataManager.LoadDefaultData();

            FarmManager.Initialize();
            BuildingManager.Initialize();
            AnimalManager.Initialize();

            MachineManager = new MachineManager();
            MachineManager.Initialize();

            WorkerManager = new WorkerManager();
            WorkerManager.Initialize();

            ProcessingManager = new ProcessingManager();
            ProcessingManager.Initialize();

            SubscribeToEvents();

            Console.WriteLine($"Kezdő tőke: {PlayerMoney:C} | Kezdő hírnév: {Reputation:F1}");
            Console.WriteLine($"Játékidő: {TimeManager.GetDateString()}");
            Console.WriteLine("==========================================\n");
        }

        public void SimulateHour()
        {
            TimeManager.AdvanceHour();
            FarmManager.Update(TimeManager.CurrentHour, TimeManager.CurrentDay, TimeManager.CurrentSeason, TimeManager.CurrentWeather);

            if (TimeManager.CurrentHour == 6)
            {
                BuildingManager.UpdateDaily();
                AnimalManager.UpdateDaily(TimeManager.CurrentWeather);
                MachineManager.UpdateDaily();
                WorkerManager.PayDailySalaries();
                WorkerManager.ProcessDailyTasks();

                float avgSkill = WorkerManager.Workers.Count > 0
                    ? WorkerManager.Workers.Average(w => w.WorkSpeed)
                    : 0.5f;
                ProcessingManager.UpdateDaily(avgSkill);
            }
        }

        public void SimulateHours(int hours)
        {
            for (int i = 0; i < hours; i++)
                SimulateHour();
        }

        public void AddMoney(float amount)
        {
            PlayerMoney += amount;
            OnMoneyChanged?.Invoke(PlayerMoney);
        }

        public bool TrySpendMoney(float amount)
        {
            if (PlayerMoney < amount)
                return false;

            PlayerMoney -= amount;
            OnMoneyChanged?.Invoke(PlayerMoney);
            return true;
        }

        public void AddReputation(float amount)
        {
            Reputation += amount;
            OnReputationChanged?.Invoke(Reputation);
        }

        private void SubscribeToEvents()
        {
            TimeManager.OnHourAdvanced += (evt) => { };

            TimeManager.OnDayAdvanced += () =>
            {
                Console.WriteLine($"\n--- Új nap: {TimeManager.GetDateString()} ---");
            };

            TimeManager.OnSeasonChanged += (season) =>
            {
                Console.WriteLine($"\n>>> ÉVSZAKVÁLTÁS: {season} <<<");
            };

            TimeManager.OnWeatherChanged += (weather) =>
            {
                Console.WriteLine($"Időjárás változott: {weather}");
            };
        }
    }
}
