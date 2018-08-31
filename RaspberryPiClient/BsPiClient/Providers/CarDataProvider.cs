namespace BsPiClient.Providers
{
    using System;
    using OBDII;

    internal class CarDataProvider
    {
        private static ObdSensor _sensor;

        public static CarSatus GetCarStatus()
        {
            try
            {
                if (_sensor == null)
                    _sensor = new ObdSensor();

                _sensor.Init();

                return _sensor.GetRpm() > 0 ? CarSatus.On : CarSatus.Off;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return CarSatus.Off;
            }
        }

        public static int GetRpmMocked()
        {
            return 0;
        }
    }
}