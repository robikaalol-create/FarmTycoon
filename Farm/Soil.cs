using FarmTycoon.Data;
using FarmTycoon.Utils;

namespace FarmTycoon.Farm
{
    public class Soil
    {
        public SoilData Data { get; private set; } = new SoilData();

        public void UpdateDaily(WeatherType weather)
        {
            Data.Moisture += weather switch
            {
                WeatherType.Rainy => 0.3f,
                WeatherType.Stormy => 0.4f,
                WeatherType.Drought => -0.2f,
                WeatherType.Heatwave => -0.15f,
                _ => -0.05f
            };

            Data.Moisture = Clamp(Data.Moisture, 0f, 1f);
            Data.WeedLevel += 0.02f;
            Data.WeedLevel = Clamp(Data.WeedLevel, 0f, 1f);
        }

        public void ApplyFertilizer(FertilizerData fertilizer)
        {
            Data.Nitrogen += fertilizer.NitrogenBoost;
            Data.Phosphorus += fertilizer.PhosphorusBoost;
            Data.Potassium += fertilizer.PotassiumBoost;
            Data.PH -= fertilizer.NegativeEffect * 0.5f;

            Data.Nitrogen = Clamp(Data.Nitrogen, 0f, 1f);
            Data.Phosphorus = Clamp(Data.Phosphorus, 0f, 1f);
            Data.Potassium = Clamp(Data.Potassium, 0f, 1f);
            Data.PH = Clamp(Data.PH, 4f, 9f);
        }

        public void Till()
        {
            Data.Compaction = 0f;
            Data.WeedLevel *= 0.3f;
        }

        public float CalculateEffectiveFertility()
        {
            float nutrients = (Data.Nitrogen + Data.Phosphorus + Data.Potassium) / 3f;
            float penalty = Data.Compaction * 0.3f + Data.WeedLevel * 0.2f;
            return Clamp(Data.Fertility * 0.3f + nutrients * 0.7f - penalty, 0f, 1f);
        }

        private float Clamp(float value, float min, float max) => value < min ? min : (value > max ? max : value);
    }
}
