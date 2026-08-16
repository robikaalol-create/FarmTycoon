using System;
using System.Collections.Generic;
using System.Linq;
using FarmTycoon.Core;
using FarmTycoon.Data;
using FarmTycoon.Utils;

namespace FarmTycoon.Animals
{
    public class AnimalManager
    {
        public List<Animal> AllAnimals { get; private set; } = new();
        public List<AnimalHousing> Housings { get; private set; } = new();
        public Dictionary<string, float> FeedStock { get; private set; } = new();

        private int _nextAnimalId = 1;
        private Random _random = new Random();

        public void Initialize()
        {
            Console.WriteLine("AnimalManager inicializálva");
        }

        public Animal PurchaseAnimal(string animalDataId, string housingId = null)
        {
            var data = DataManager.Instance.GetAnimal(animalDataId);
            if (data == null) return null;

            if (!GameManager.Instance.TrySpendMoney(data.PurchaseCost))
                return null;

            var gender = _random.NextDouble() < 0.5 ? AnimalGender.Male : AnimalGender.Female;
            var animal = new Animal($"anm_{_nextAnimalId++}", data, gender);

            if (housingId != null)
            {
                var housing = Housings.FirstOrDefault(h => h.BuildingId == housingId);
                if (housing != null && housing.HasSpace)
                {
                    housing.AddAnimal(animal);
                }
                else
                {
                    GameManager.Instance.AddMoney(data.PurchaseCost);
                    return null;
                }
            }

            AllAnimals.Add(animal);
            return animal;
        }

        public bool SellAnimal(string instanceId)
        {
            var animal = AllAnimals.FirstOrDefault(a => a.InstanceId == instanceId);
            if (animal == null || !animal.IsAlive) return false;

            float value = animal.CalculateValue();
            GameManager.Instance.AddMoney(value);

            foreach (var housing in Housings)
            {
                if (housing.Animals.Contains(animal))
                {
                    housing.RemoveAnimal(animal);
                    break;
                }
            }

            AllAnimals.Remove(animal);
            return true;
        }

        public void AddFeed(string feedType, float amount, float costPerUnit)
        {
            float totalCost = amount * costPerUnit;
            if (!GameManager.Instance.TrySpendMoney(totalCost))
                return;

            if (!FeedStock.ContainsKey(feedType))
                FeedStock[feedType] = 0f;
            FeedStock[feedType] += amount;
        }

        public void UpdateDaily(WeatherType weather)
        {
            FeedAllAnimals();
            WaterAllAnimals();

            foreach (var animal in AllAnimals.Where(a => a.IsAlive))
            {
                var housing = Housings.FirstOrDefault(h => h.Animals.Contains(animal));
                float housingBonus = housing != null ? housing.Cleanliness * 0.2f : 0f;

                animal.UpdateDaily(weather, 1.0f + housingBonus);

                if (housing != null)
                {
                    animal.Needs.Cleanliness = housing.Cleanliness;
                    animal.Needs.Safety = 0.7f + housing.TemperatureControl * 0.3f;
                }
            }

            foreach (var housing in Housings)
            {
                housing.UpdateDaily();
            }

            HandleBreeding();
        }

        private void FeedAllAnimals()
        {
            foreach (var animal in AllAnimals.Where(a => a.IsAlive))
            {
                var data = DataManager.Instance.GetAnimal(animal.AnimalDataId);
                if (data == null) continue;

                string feedType = data.FeedType;
                float needed = data.FeedAmountPerDay;

                if (FeedStock.ContainsKey(feedType) && FeedStock[feedType] >= needed)
                {
                    FeedStock[feedType] -= needed;
                    animal.Feed(0.5f, 1.0f);
                }
                else
                {
                    animal.Needs.Hunger -= 0.3f;
                }
            }
        }

        private void WaterAllAnimals()
        {
            foreach (var animal in AllAnimals.Where(a => a.IsAlive))
            {
                animal.Water(0.6f);
            }
        }

        private void HandleBreeding()
        {
            var females = AllAnimals.Where(a => a.IsAlive && a.CanBreed()).ToList();
            var males = AllAnimals.Where(a => a.IsAlive && a.Gender == AnimalGender.Male 
                && a.LifeStage == AnimalLifeStage.Adult 
                && a.Health.Status == AnimalHealthStatus.Healthy).ToList();

            foreach (var female in females)
            {
                if (males.Count == 0) break;
                var male = males[_random.Next(males.Count)];
                female.Impregnate(male.Genetics);
            }

            foreach (var animal in AllAnimals.Where(a => a.IsAlive && a.Health.IsPregnant))
            {
                int gestationDays = 280;
                var baby = animal.AdvancePregnancy(gestationDays);
                if (baby != null)
                {
                    AllAnimals.Add(baby);
                    var motherHousing = Housings.FirstOrDefault(h => h.Animals.Contains(animal));
                    motherHousing?.AddAnimal(baby);
                    Console.WriteLine($"Új {baby.Name} született! ({baby.InstanceId})");
                }
            }
        }

        public Dictionary<AnimalProductType, float> GetDailyProduction()
        {
            var production = new Dictionary<AnimalProductType, float>();

            foreach (var animal in AllAnimals.Where(a => a.IsAlive))
            {
                var data = DataManager.Instance.GetAnimal(animal.AnimalDataId);
                if (data == null || animal.DailyProductAmount <= 0) continue;

                AnimalProductType productType = data.Product switch
                {
                    "Tej" => AnimalProductType.Milk,
                    "Tojás" => AnimalProductType.Eggs,
                    "Hús" => AnimalProductType.Meat,
                    "Gyapjú" => AnimalProductType.Wool,
                    _ => AnimalProductType.Manure
                };

                if (!production.ContainsKey(productType))
                    production[productType] = 0f;
                production[productType] += animal.DailyProductAmount;
            }

            float manureAmount = AllAnimals.Count(a => a.IsAlive) * 0.5f;
            if (manureAmount > 0)
            {
                if (!production.ContainsKey(AnimalProductType.Manure))
                    production[AnimalProductType.Manure] = 0f;
                production[AnimalProductType.Manure] += manureAmount;
            }

            return production;
        }

        public (int total, int healthy, int sick, int pregnant, float avgWellbeing) GetHerdSummary()
        {
            var alive = AllAnimals.Where(a => a.IsAlive).ToList();
            int total = alive.Count;
            int healthy = alive.Count(a => a.Health.Status == AnimalHealthStatus.Healthy);
            int sick = alive.Count(a => a.Health.Status >= AnimalHealthStatus.Sick);
            int pregnant = alive.Count(a => a.Health.IsPregnant);
            float avgWellbeing = alive.Count > 0 ? alive.Average(a => a.Needs.CalculateWellbeing()) : 0f;

            return (total, healthy, sick, pregnant, avgWellbeing);
        }
    }
}
