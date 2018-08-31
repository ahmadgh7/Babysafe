namespace BsPiClient.OBDII
{
    using System;
    using System.Diagnostics;

    internal class ObdSensor
    {
        private const string ObdPath = "/home/pi/pi-bluetooth-obd/obd_reader.py";
        private const string ObdCommand = "sudo python3 " + ObdPath;

        private Process _process;

        public void Init()
        {
            _process = ProcessHelper.Init(ObdCommand);
        }

        public double GetRpm()
        {
            Console.WriteLine("Reading RPM");

            if (_process == null)
            {
                Console.WriteLine("Call OBDII init first");
                throw new Exception("Call OBDII init first");
            }

            var output = ProcessHelper.Run(_process, ObdCommand);
            if (!output.Contains("Car Connected"))
            {
                Console.WriteLine("OBDII init was not successful");
                throw new Exception("OBDII init was not successful");
            }

            if (!TryExtractRpm(output, out var rpm))
            {
                Console.WriteLine("Failed to extract RPM, maybe car is off...");
                return 0;
            }

            return rpm;
        }

        private static bool TryExtractRpm(string input, out double rpm)
        {
            var match = false;
            var inputLines = input.Split();
            foreach (var line in inputLines)
            {
                if (match)
                {
                    if (!double.TryParse(line, out rpm))
                        return false;

                    return true;
                }

                if (line.Contains("Result:"))
                    match = true;
            }

            rpm = 0;
            return false;
        }
    }
}
