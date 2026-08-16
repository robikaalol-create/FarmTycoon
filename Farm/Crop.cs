using FarmTycoon.Data;
using FarmTycoon.Utils;

namespace FarmTycoon.Farm
{
    public class Crop
    {
        public string CropDataId { get; private set; }
        public GrowthStage CurrentStage { get; private set; } = GrowthStage.Seed;
        public float GrowthProgress { get; private set; } = 0f;
        public float Health { get; private set; } = 1.0f;
        public float YieldMultiplier { get; private set; } = 1.0f;
        public bool IsWithered => CurrentStage == GrowthStage.Withered;

        private CropData _data;
        private int _daysInCurrentStage = 0;

        public Crop(CropData data)
        {
            _data = data;
            CropDataId = data.Id;
        }

        public void UpdateDaily(Soil soil, WeatherType weather, Season season)
        {
            if (CurrentStage == GrowthStage.Harvestable || CurrentStage == GrowthStage.Withered)
                return;

            float growthRate = 1.0f;

            float waterDiff = soil.Data.Moisture - _data.WaterRequirement;
            growthRate *= 1f - System.MathF.Abs(waterDiff) * 0.5f;

            float nutrientScore = soil.CalculateEffectiveFertility();
            growthRate *= 0.5f + nutrientScore * 0.5f;

            growthRate *= weather switch
            {
                WeatherType.Sunny => 1.1f,
                WeatherType.Rainy => 1.0f,
                WeatherType.Stormy => 0.7f,
                WeatherType.Drought => 0.4f,
                WeatherType.Frosty => 0.2f,
                WeatherType.Hail => 0.3f,
                WeatherType.Heatwave => 0.6f,
                _ => 0.9f
            };

            if (season != _data.Season)
                growthRate *= 0.3f;

            Health -= soil.Data.WeedLevel * _data.WeedSensitivity * 0.1f;
            Health = Clamp(Health, 0f, 1f);

            if (Health <= 0f)
            {
                CurrentStage = GrowthStage.Withered;
                return;
            }

            GrowthProgress += growthRate / _data.GrowthDays;
            _daysInCurrentStage++;

            if (GrowthProgress >= GetStageThreshold())
            {
                AdvanceStage();
            }

            YieldMultiplier = Health * (0.7f + nutrientScore * 0.3f);
        }

        private void AdvanceStage()
        {
            var oldStage = CurrentStage;
            CurrentStage = CurrentStage switch
            {
                GrowthStage.Seed => GrowthStage.Germination,
                GrowthStage.Germination => GrowthStage.Seedling,
                GrowthStage.Seedling => GrowthStage.Growing,
                GrowthStage.Growing => GrowthStage.Maturing,
                GrowthStage.Maturing => GrowthStage.Harvestable,
                _ => CurrentStage
            };

            GrowthProgress = 0f;
            _daysInCurrentStage = 0;

            if (oldStage != CurrentStage)
            {
                Core.EventSystem.Instance.Publish(new CropGrowthStageChangedEvent(CropDataId, oldStage, CurrentStage));
            }
        }

        private float GetStageThreshold()
        {
            return CurrentStage switch
            {
                GrowthStage.Seed => 0.15f,
                GrowthStage.Germination => 0.25f,
                GrowthStage.Seedling => 0.25f,
                GrowthStage.Growing => 0.2f,
                GrowthStage.Maturing => 0.15f,
                _ => 1f
            };
        }

        public float CalculateYield(float hectares)
        {
            if (CurrentStage != GrowthStage.Harvestable) return 0f;
            return _data.BaseYieldPerHectare * hectares * YieldMultiplier;
        }

        private float Clamp(float value, float min, float max) => value < min ? min : (value > max ? max : value);
    }
}
