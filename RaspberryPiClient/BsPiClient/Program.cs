using System;
using System.Text;
using System.Threading.Tasks;
using BsPiClient.Providers;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace BsPiClient
{
    internal class Program
    {
        private const string ConnectionString = "HostName=Babysafe.azure-devices.net;DeviceId=Pi;SharedAccessKey=X+KC5AKvBZuEGIgWuwOZA8GvssxcqdEb7w0u584f8NY=";
        private static DeviceClient _deviceClient;

        private static void Main()
        {
            // Connect to the IoT hub using the MQTT protocol
            _deviceClient = DeviceClient.CreateFromConnectionString(ConnectionString, TransportType.Mqtt);
            Loop();
        }

        // Async method to send simulated telemetry
        private static async void Loop()
        {
            while (true)
            {
                var carRpm = CarDataProvider.GetRpm();
                if (carRpm < 1)
                {
                    var presence = PresenceProvider.GetPresence();
                    if (presence)
                    {
                        await Task.Delay(120000);
                        presence = PresenceProvider.GetPresence();
                        if (presence)
                        {
                            await SendDataToCloud(carRpm, presence: true);
                            continue;
                        }
                    }
                    await SendDataToCloud(carRpm, presence: false);
                }
                else
                {
                    await SendDataToCloud(carRpm);
                }
                await Task.Delay(60000);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private static async Task SendDataToCloud(int carRpm, bool? presence = null)
        {
            // Create JSON message
            var telemetryDataPoint = new
            {
                carRpm,
                presence
            };

            var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
            var message = new Message(Encoding.ASCII.GetBytes(messageString));

            // Add a custom application property to the message.
            // An IoT hub can filter on these properties without access to the message body.
            message.Properties.Add("temperatureAlert", carRpm < 1 && presence == true  ? "true" : "false");

            // Send the tlemetry message
            await _deviceClient.SendEventAsync(message);
            Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
        }
    }
}
