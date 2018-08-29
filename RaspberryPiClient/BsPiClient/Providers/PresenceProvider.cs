using System;
using BsPiClient.MLX90640;

namespace BsPiClient.Providers
{
    internal class PresenceProvider
    {
        private static Sensor _sensor;

        public static bool GetPresence()
        {
            try
            {
                if (_sensor == null)
                   _sensor = new Sensor();

                _sensor.Init();

                var image = _sensor.GetImage();
                return image.IsPresenceDetected();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public static bool GetPresenceMocked(bool presence = false)
        {
            return presence;
        }
    }
}