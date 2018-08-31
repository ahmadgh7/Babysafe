namespace BsPiClient.MLX90640
{
    using System;
    using System.Diagnostics;

    internal class Mlx90640Sensor
    {
        private const string Mlx90640Path = "/home/pi/RPiMLX90640/MLX90640";
        private const string ReadingsPath = "/home/pi/babysafe/img";
        private const string Mlx90640Command = "sudo " + Mlx90640Path;

        private Process _process;

        public void Init()
        {
            _process = ProcessHelper.Init(Mlx90640Command);
        }

        public Image GetImage()
        {
            try
            {
                Mlx90640Helper.CleanData(ReadingsPath);
                Capture();
                return Mlx90640Helper.GetLatestImage(ReadingsPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        private void Capture()
        {
            Console.WriteLine("Capturing image");

            if (_process == null)
            {
                Console.WriteLine("Call MLX90640 init first");
                throw new Exception("Call MLX90640 init first!");
            }

            var output = ProcessHelper.Run(_process, Mlx90640Command);
            if (!output.Contains("Ready to start"))
            {
                Console.WriteLine("MLX90640 init was not successful");
                throw new Exception("MLX90640 init was not successful");
            }

            if (!output.Contains("Done capture!"))
            {
                Console.WriteLine("Capture was not successful");
                throw new Exception("Capture was not successful");
            }
            
            Console.WriteLine("Capture was successful");
        }
    }
}
