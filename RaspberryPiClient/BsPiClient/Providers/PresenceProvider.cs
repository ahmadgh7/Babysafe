using BsPiClient.MLX90640;

namespace BsPiClient.Providers
{
    internal class PresenceProvider
    {

        public static bool GetPresence()
        {
            var image = Sensor.GetImage();
            return image.IsPresenceDetected();
        }
    }
}