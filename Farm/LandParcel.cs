using FarmTycoon.Data;
using FarmTycoon.Utils;

namespace FarmTycoon.Farm
{
    public class LandParcel
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public float SizeInHectares { get; private set; }
        public Soil Soil { get; private set; } = new Soil();
        public Crop PlantedCrop { get; private set; }
        public bool IsOwned { get; set; } = false;
        public float PurchasePrice { get; private set; }

        public bool HasCrop => PlantedCrop != null && !PlantedCrop.IsWithered;

        public LandParcel(string id, LandData data)
        {
            Id = id;
            Name = data.Name;
            SizeInHectares = data.SizeInHectares;
            PurchasePrice = data.PurchasePrice;

            Soil.Data.Fertility = data.BaseFertility;
            Soil.Data.Moisture = data.WaterAvailability;
        }

        public void PlantCrop(CropData cropData)
        {
            if (PlantedCrop != null && !PlantedCrop.IsWithered)
                return;

            PlantedCrop = new Crop(cropData);
            Core.EventSystem.Instance.Publish(new LandParcelChangedEvent(Id, "CropPlanted"));
        }

        public float Harvest()
        {
            if (PlantedCrop?.CurrentStage == GrowthStage.Harvestable)
            {
                float yield = PlantedCrop.CalculateYield(SizeInHectares);
                PlantedCrop = null;
                Core.EventSystem.Instance.Publish(new LandParcelChangedEvent(Id, "Harvested"));
                return yield;
            }
            return 0f;
        }

        public void UpdateDaily(WeatherType weather, Season season)
        {
            Soil.UpdateDaily(weather);
            PlantedCrop?.UpdateDaily(Soil, weather, season);
        }
    }
}
