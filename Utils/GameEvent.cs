using System;
using System.Collections.Generic;

namespace FarmTycoon.Utils
{
    public abstract class GameEvent
    {
        public DateTime Timestamp { get; } = DateTime.Now;
    }

    public class TimeAdvancedEvent : GameEvent
    {
        public int Day { get; }
        public int Hour { get; }
        public Season Season { get; }

        public TimeAdvancedEvent(int day, int hour, Season season)
        {
            Day = day;
            Hour = hour;
            Season = season;
        }
    }

    public class CropGrowthStageChangedEvent : GameEvent
    {
        public string CropId { get; }
        public GrowthStage OldStage { get; }
        public GrowthStage NewStage { get; }

        public CropGrowthStageChangedEvent(string cropId, GrowthStage oldStage, GrowthStage newStage)
        {
            CropId = cropId;
            OldStage = oldStage;
            NewStage = newStage;
        }
    }

    public class LandParcelChangedEvent : GameEvent
    {
        public string LandId { get; }
        public string ChangeType { get; }

        public LandParcelChangedEvent(string landId, string changeType)
        {
            LandId = landId;
            ChangeType = changeType;
        }
    }

    public enum Season
    {
        Spring,
        Summer,
        Autumn,
        Winter
    }

    public enum GrowthStage
    {
        Seed,
        Germination,
        Seedling,
        Growing,
        Maturing,
        Harvestable,
        Withered
    }

    public enum SoilType
    {
        Sandy,
        Loamy,
        Clay,
        Silty,
        Peaty,
        Chalky
    }

    public enum WeatherType
    {
        Sunny,
        Rainy,
        Stormy,
        Drought,
        Foggy,
        Frosty,
        Hail,
        Heatwave
    }
}
