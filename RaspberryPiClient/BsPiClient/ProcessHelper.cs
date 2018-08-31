namespace BsPiClient
{
    using System;
    using System.Diagnostics;

    internal class ProcessHelper
    {
        public static Process Init(string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            return new Process()
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

        public static string Run(Process process, string processName)
        {
            var output = "";

            process.OutputDataReceived += (sender, args) => OnOutputDataReceived(ref output, args.Data);

            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to run process {0}: {1}",processName, ex);
                return null;
            }

            return output;
        }

        private static void OnOutputDataReceived(ref string output, string argsData)
        {            
            output += argsData;
        }
    }
}
