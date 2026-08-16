using System;
using FarmTycoon.Data;
using FarmTycoon.Utils;

namespace FarmTycoon.Animals
{
    public class Animal
    {
        public string InstanceId { get; private set; }
        public string AnimalDataId { get; private set; }
        public string Name { get; private set; }
        public AnimalGender Gender { get; private set; }
        public AnimalLifeStage LifeStage { get; private set; } = AnimalLifeStage.Baby;
        public AnimalTemperament Temperament { get; private set; }
        public int AgeInDays { get; private set; } = 0;
        public bool IsAlive { get; private set; } = true;

        public AnimalGenetics Genetics { get; private set; } = new AnimalGenetics();
        public AnimalNeeds Needs { get; private set; } = new AnimalNeeds();
        public AnimalHealth Health { get; private set; } = new AnimalHealth();

        public float DailyProductAmount { get; private set; } = 0f;
        public float TotalProduced { get; private set; } = 0f;

        private AnimalData _data;
        private Random _random = new Random();

        public Animal(string instanceId, AnimalData data, AnimalGender gender)
        {
            InstanceId = instanceId;
            AnimalDataId = data.Id;
            Name = data.Name;
            Gender = gender;
            _data = data;
            Temperament = (AnimalTemperament)_random.Next(Enum.GetValues(typeof(AnimalTemperament)).Length);
        }

        public void UpdateDaily(WeatherType weather, float feedQuality = 1.0f)
        {
            if (!IsAlive) return;

            AgeInDays++;
            UpdateLifeStage();

            Needs.DecayDaily();

            Needs.TemperatureComfort = weather switch
            {
                WeatherType.Sunny => 1.0f,
                WeatherType.Rainy => 0.7f,
                WeatherType.Stormy => 0.4f,
                WeatherType.Drought => 0.3f,
                WeatherType.Frosty => 0.2f,
                WeatherType.Hail => 0.1f,
                WeatherType.Heatwave => 0.3f,
                _ => 0.8f
            };

            float wellbeing = Needs.CalculateWellbeing();
            Health.UpdateDaily(wellbeing, Genetics.DiseaseResistance);

            if (LifeStage == AnimalLifeStage.Adult && Health.Status <= AnimalHealthStatus.Stressed)
            {
                float productionMultiplier = wellbeing * Genetics.ProductivityGene * feedQuality;
                if (Health.Status == AnimalHealthStatus.Sick)
                    productionMultiplier *= 0.3f;

                DailyProductAmount = _data.ProductPerDay * productionMultiplier;
                TotalProduced += DailyProductAmount;
            }
            else
            {
                DailyProductAmount = 0f;
            }

            if (Health.HealthValue <= 0f)
            {
                IsAlive = false;
            }
        }

        public void Feed(float amount, float quality = 1.0f)
        {
            Needs.Feed(amount * quality);
        }

        public void Water(float amount)
        {
            Needs.Water(amount);
        }

        public void Clean(float amount)
        {
            Needs.Clean(amount);
        }

        public void Treat(float effectiveness)
        {
            Health.Treat(effectiveness);
        }

        public bool CanBreed()
        {
            return Gender == AnimalGender.Female
                && LifeStage == AnimalLifeStage.Adult
                && Health.Status == AnimalHealthStatus.Healthy
                && !Health.IsPregnant
                && Needs.CalculateWellbeing() > 0.6f;
        }

        public bool Impregnate(AnimalGenetics fatherGenetics)
        {
            if (!CanBreed()) return false;

            float chance = Genetics.Fertility * 0.5f;
            if (_random.NextDouble() < chance)
            {
                Health.IsPregnant = true;
                Health.PregnancyDays = 0;
                return true;
            }
            return false;
        }

        public Animal AdvancePregnancy(int gestationDays)
        {
            if (!Health.IsPregnant) return null;

            Health.PregnancyDays++;
            if (Health.PregnancyDays >= gestationDays)
            {
                Health.IsPregnant = false;
                Health.PregnancyDays = 0;
                var babyGender = _random.NextDouble() < 0.5 ? AnimalGender.Male : AnimalGender.Female;
                var baby = new Animal($"{InstanceId}_baby_{_random.Next(1000)}", _data, babyGender);
                baby.Genetics = AnimalGenetics.Breed(Genetics, Genetics);
                return baby;
            }
            return null;
        }

        public float CalculateValue()
        {
            if (!IsAlive) return 0f;

            float baseValue = _data.PurchaseCost;
            float ageMultiplier = LifeStage switch
            {
                AnimalLifeStage.Baby => 0.5f,
                AnimalLifeStage.Young => 0.8f,
                AnimalLifeStage.Adult => 1.2f,
                AnimalLifeStage.Senior => 0.6f,
                _ => 1.0f
            };

            float healthMultiplier = Health.HealthValue / 100f;
            float geneticMultiplier = 0.8f + Genetics.ProductivityGene * 0.4f;

            return baseValue * ageMultiplier * healthMultiplier * geneticMultiplier;
        }

        private void UpdateLifeStage()
        {
            int adultAge = (int)(365f / Genetics.GrowthRate);
            int seniorAge = (int)(adultAge * 2.5f);

            if (AgeInDays < 30)
                LifeStage = AnimalLifeStage.Baby;
            else if (AgeInDays < adultAge)
                LifeStage = AnimalLifeStage.Young;
            else if (AgeInDays < seniorAge)
                LifeStage = AnimalLifeStage.Adult;
            else
                LifeStage = AnimalLifeStage.Senior;
        }
    }
}
