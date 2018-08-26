using System;
using System.Diagnostics;

namespace BsPiClient.MLX90640
{
    internal class Sensor
    {
        private const string Mlx90640Path = "/home/pi/RPiMLX90640/MLX90640";
        private const string ReadingsPath = "/home/pi/RPiMLX90640/img";

        public void Init()
        {
            try
            {
                var initOutput = ("sudo " + Mlx90640Path).Bash();
                Debug.WriteLine(initOutput.Contains("Init was successful") ? "Init was successful" : "Failed to Init");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to Init: " + ex);
            }
        }

        public static Image GetImage()
        {
            Helper.CleanData(ReadingsPath);
            Capture();
            return Helper.GetLatestImage(ReadingsPath);
        }

        private static void Capture()
        {
            var captureOutput = ("sudo " + Mlx90640Path).Bash();
            if (captureOutput.Contains("Done Printing! RGB!!"))
            {
                Debug.WriteLine("Capture was successful");
                return;
            }

            Debug.WriteLine("Failed to capture");
        }
    }
}
