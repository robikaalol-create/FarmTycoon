using System.Collections.Generic;
using FarmTycoon.Core;
using FarmTycoon.Data;
using FarmTycoon.Utils;

namespace FarmTycoon.Farm
{
    public class FarmManager
    {
        public List<LandParcel> LandParcels { get; private set; } = new();
        public List<IrrigationSystem> IrrigationSystems { get; private set; } = new();

        public void Initialize()
        {
            var starterLand = new LandData
            {
                Id = "starter",
                Name = "Kezdő föld",
                SizeInHectares = 2.0f,
                SoilType = SoilType.Loamy,
                BaseFertility = 0.6f,
                WaterAvailability = 0.5f,
                PurchasePrice = 0f,
                Region = "Kezdő régió"
            };

            var parcel = new LandParcel("land_001", starterLand);
            parcel.IsOwned = true;
            LandParcels.Add(parcel);

            IrrigationSystems.Add(new IrrigationSystem(IrrigationSystem.IrrigationType.Manual));

            System.Console.WriteLine($"Farm inicializálva: {LandParcels.Count} földterület");
        }

        public void Update(int hour, int day, Season season, WeatherType weather)
        {
            if (hour == 6)
            {
                foreach (var parcel in LandParcels)
                {
                    if (parcel.IsOwned)
                    {
                        parcel.UpdateDaily(weather, season);
                    }
                }

                foreach (var system in IrrigationSystems)
                {
                    if (system.Type >= IrrigationSystem.IrrigationType.Sprinkler)
                    {
                        foreach (var parcel in LandParcels)
                        {
                            if (parcel.IsOwned && parcel.Soil.Data.Moisture < 0.4f)
                            {
                                system.Irrigate(parcel.Soil);
                            }
                        }
                    }
                }
            }
        }

        public bool PurchaseLand(string landId, LandData template)
        {
            if (!GameManager.Instance.TrySpendMoney(template.PurchasePrice))
                return false;

            var parcel = new LandParcel(landId, template);
            parcel.IsOwned = true;
            LandParcels.Add(parcel);

            EventSystem.Instance.Publish(new LandParcelChangedEvent(landId, "Purchased"));
            return true;
        }

        public void PlantCropOnParcel(string parcelId, string cropId)
        {
            var parcel = LandParcels.Find(l => l.Id == parcelId);
            var cropData = DataManager.Instance.GetCrop(cropId);

            if (parcel != null && cropData != null && parcel.IsOwned)
            {
                parcel.PlantCrop(cropData);
            }
        }

        public float HarvestParcel(string parcelId)
        {
            var parcel = LandParcels.Find(l => l.Id == parcelId);
            return parcel?.Harvest() ?? 0f;
        }

        public void ApplyFertilizer(string parcelId, string fertilizerId)
        {
            var parcel = LandParcels.Find(l => l.Id == parcelId);
            var fertilizer = DataManager.Instance.GetFertilizer(fertilizerId);

            if (parcel != null && fertilizer != null && parcel.IsOwned)
            {
                if (GameManager.Instance.TrySpendMoney(fertilizer.Cost * parcel.SizeInHectares))
                {
                    parcel.Soil.ApplyFertilizer(fertilizer);
                }
            }
        }

        public void TillParcel(string parcelId)
        {
            var parcel = LandParcels.Find(l => l.Id == parcelId);
            parcel?.Soil.Till();
        }
    }
}
