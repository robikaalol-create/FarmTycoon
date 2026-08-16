using System;
using FarmTycoon.Utils;

namespace FarmTycoon.Core
{
    public class TimeManager
    {
        public int CurrentDay { get; private set; } = 1;
        public int CurrentHour { get; private set; } = 6;
        public Season CurrentSeason { get; private set; } = Season.Spring;
        public int CurrentYear { get; private set; } = 1;
        public WeatherType CurrentWeather { get; private set; } = WeatherType.Sunny;

        public const int HoursPerDay = 24;
        public const int DaysPerSeason = 30;
        public const int SeasonsPerYear = 4;

        public event Action<TimeAdvancedEvent> OnHourAdvanced;
        public event Action OnDayAdvanced;
        public event Action<Season> OnSeasonChanged;
        public event Action<WeatherType> OnWeatherChanged;

        private Random _random = new Random();

        public void AdvanceHour()
        {
            CurrentHour++;

            if (CurrentHour >= HoursPerDay)
            {
                CurrentHour = 0;
                AdvanceDay();
            }

            if (_random.NextDouble() < 0.05)
                ChangeWeather();

            OnHourAdvanced?.Invoke(new TimeAdvancedEvent(CurrentDay, CurrentHour, CurrentSeason));
        }

        private void AdvanceDay()
        {
            CurrentDay++;

            if (CurrentDay > DaysPerSeason)
            {
                CurrentDay = 1;
                AdvanceSeason();
            }

            OnDayAdvanced?.Invoke();
        }

        private void AdvanceSeason()
        {
            int currentSeasonIndex = (int)CurrentSeason;
            currentSeasonIndex++;

            if (currentSeasonIndex >= SeasonsPerYear)
            {
                currentSeasonIndex = 0;
                CurrentYear++;
            }

            CurrentSeason = (Season)currentSeasonIndex;
            OnSeasonChanged?.Invoke(CurrentSeason);
        }

        private void ChangeWeather()
        {
            WeatherType newWeather = GetRandomWeatherForSeason(CurrentSeason);
            if (newWeather != CurrentWeather)
            {
                CurrentWeather = newWeather;
                OnWeatherChanged?.Invoke(CurrentWeather);
            }
        }

        private WeatherType GetRandomWeatherForSeason(Season season)
        {
            var weatherOptions = season switch
            {
                Season.Spring => new[] { WeatherType.Sunny, WeatherType.Rainy, WeatherType.Foggy },
                Season.Summer => new[] { WeatherType.Sunny, WeatherType.Drought, WeatherType.Heatwave, WeatherType.Rainy },
                Season.Autumn => new[] { WeatherType.Sunny, WeatherType.Rainy, WeatherType.Foggy, WeatherType.Stormy },
                Season.Winter => new[] { WeatherType.Sunny, WeatherType.Frosty, WeatherType.Foggy, WeatherType.Hail },
                _ => new[] { WeatherType.Sunny }
            };

            return weatherOptions[_random.Next(weatherOptions.Length)];
        }

        public string GetDateString()
        {
            return $"{CurrentYear}. év, {CurrentSeason}, {CurrentDay}. nap, {CurrentHour:00}:00";
        }

        public void SetWeather(WeatherType weather)
        {
            CurrentWeather = weather;
            OnWeatherChanged?.Invoke(CurrentWeather);
        }
    }
}
