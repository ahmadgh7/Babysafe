using System;

namespace BsPiClient.Providers
{
    internal class WeatherProvider
    {
        private const double MinTemperature = 20;
        private const double MinHumidity = 60;
        private static readonly Random Rand = new Random();

        public static double GetTemperature()
        {
            return MinTemperature + Rand.NextDouble() * 15;
        }

        public static double GetHumidity()
        {
            return MinHumidity + Rand.NextDouble() * 20;
        }
    }
}
