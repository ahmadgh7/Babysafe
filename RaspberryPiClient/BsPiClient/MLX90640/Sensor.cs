using System;
using System.Diagnostics;

namespace BsPiClient.MLX90640
{
    internal class Sensor
    {
        private const string Mlx90640Path = "/home/pi/RPiMLX90640/MLX90640";
        private const string ReadingsPath = "/home/pi/babysafe/img";
        private const string Mlx90640Command = "sudo " + Mlx90640Path;
        private const int TimeOut = 120000;

        private Process _process;

        public void Init()
        {
            var escapedArgs = Mlx90640Command.Replace("\"", "\\\"");

            _process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
        }

        public Image GetImage()
        {
            try
            {
                Helper.CleanData(ReadingsPath);
                Capture();
                return Helper.GetLatestImage(ReadingsPath);
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
                Console.WriteLine("Call Init first!");
                return;
            }

            var output = "";

            _process.OutputDataReceived += (sender, args) => OnOutputDataReceived(ref output, args.Data);

            try
            {
                _process.Start();
                _process.BeginOutputReadLine();

                _process.WaitForExit();
                if (!_process.HasExited)
                {
                    Console.WriteLine("Timeout was reached");
                    return;
                }

                if (!output.Contains("Ready to start"))
                {
                    Console.WriteLine("Init was not successful");
                    return;
                }

                if (!output.Contains("Done capture!"))
                {
                    Console.WriteLine("Init was not successful");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Capture was not successful: " + ex);
                return;
            }

            Console.WriteLine("Capture was successful");
        }

        private static void OnOutputDataReceived(ref string output, string argsData)
        {
            //Console.WriteLine("Data recevied: " + argsData);
            output += argsData;
        }
    }
}
