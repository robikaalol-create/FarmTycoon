using System;
using System.Collections.Generic;
using FarmTycoon.Utils;

namespace FarmTycoon.Farm
{
    public class WeatherForecast
    {
        public List<WeatherType> DailyForecasts { get; private set; } = new();
        public int ForecastAccuracy { get; private set; } = 3;
        public float AccuracyRate { get; private set; } = 0.7f;
        private Random _random = new Random();

        public void GenerateForecast(Season season, int daysAhead = 3)
        {
            DailyForecasts.Clear();
            ForecastAccuracy = daysAhead;
            for (int i = 0; i < daysAhead; i++)
                DailyForecasts.Add(GetRandomWeatherForSeason(season));
        }

        public bool IsForecastAccurate(int dayIndex, WeatherType actualWeather)
        {
            if (dayIndex >= DailyForecasts.Count) return false;
            return _random.NextDouble() < AccuracyRate
                ? DailyForecasts[dayIndex] == actualWeather
                : DailyForecasts[dayIndex] != actualWeather;
        }

        private WeatherType GetRandomWeatherForSeason(Season season)
        {
            var options = season switch
            {
                Season.Spring => new[] { WeatherType.Sunny, WeatherType.Rainy, WeatherType.Foggy },
                Season.Summer => new[] { WeatherType.Sunny, WeatherType.Drought, WeatherType.Heatwave, WeatherType.Rainy },
                Season.Autumn => new[] { WeatherType.Sunny, WeatherType.Rainy, WeatherType.Foggy, WeatherType.Stormy },
                Season.Winter => new[] { WeatherType.Sunny, WeatherType.Frosty, WeatherType.Foggy, WeatherType.Hail },
                _ => new[] { WeatherType.Sunny }
            };
            return options[_random.Next(options.Length)];
        }
    }
}
