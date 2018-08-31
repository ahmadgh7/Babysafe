namespace BsPiClient.Providers
{
    using System;
    using MLX90640;

    internal class PresenceProvider
    {
        private static Mlx90640Sensor _sensor;

        public static bool GetPresence()
        {
            try
            {
                if (_sensor == null)
                   _sensor = new Mlx90640Sensor();

                _sensor.Init();

                var image = _sensor.GetImage();
                return image.IsPresenceDetected();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return true;
            }
        }

        public static bool GetPresenceMocked(bool presence = false)
        {
            return presence;
        }
    }
}