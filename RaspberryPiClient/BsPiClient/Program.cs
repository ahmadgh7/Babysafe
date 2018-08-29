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

        private static async Task Main()
        {
            // Connect to the IoT hub using the MQTT protocol
            Console.WriteLine("Creating IoT hub");
            _deviceClient = DeviceClient.CreateFromConnectionString(ConnectionString, TransportType.Mqtt);

            await Loop();
        }

        private static async Task Loop()
        {
            Console.WriteLine("Entering loop...");

            while (true)
            {
                Console.WriteLine("Reading car RPM");

                var carRpm = CarDataProvider.GetRpm();
                if (carRpm >= 1)
                {
                    await SendMessageToCloud(carRpm);
                    await Task.Delay(5000);
                }
                else
                {
                    Console.WriteLine("Car is off");
                    Console.WriteLine("Getting presence");
                    var presence = PresenceProvider.GetPresence();
                    if (presence)
                    {
                        await Task.Delay(5000);
                        Console.WriteLine("Verifying presence still there");
                        presence = PresenceProvider.GetPresence();
                        if (presence)
                        {
                            await SendMessageToCloud(carRpm, presence: true);
                            return;
                        }
                    }

                    await SendMessageToCloud(carRpm, presence: false);
                    return;
                }
            }
        }

        private static async Task SendMessageToCloud(int carRpm, bool? presence = null)
        {
            try
            {
                // Create JSON message
                var telemetryDataPoint = new
                {
                    carStatus = carRpm < 1 ? "off" : "on",
                    carRpm,
                    baby = presence == true ? "yes" : "no"
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                // Add a custom application property to the message.
                // An IoT hub can filter on these properties without access to the message body.
                message.Properties.Add("temperatureAlert", carRpm < 1 && presence == true ? "true" : "false");

                // Send the tlemetry message
                await _deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to send message to cloud: " + ex);
            }
        }
    }
}
