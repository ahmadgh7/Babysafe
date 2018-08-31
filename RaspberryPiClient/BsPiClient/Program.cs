using BsPiClient.OBDII;

namespace BsPiClient
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Providers;
    using Microsoft.Azure.Devices.Client;
    using Newtonsoft.Json;

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

                var carStatus = CarDataProvider.GetCarStatus();
                switch (carStatus)
                {
                    case CarSatus.On:
                        Console.WriteLine("Car is on");
                        await SendMessageToCloud(CarSatus.On);
                        await Task.Delay(5000);
                        break;

                    case CarSatus.Off:
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
                                await SendMessageToCloud(CarSatus.Off, presence: true);
                                return;
                            }
                        }

                        await SendMessageToCloud(CarSatus.Off, presence: false);
                        return;
                }
            }
        }

        private static async Task SendMessageToCloud(CarSatus carStatus, bool? presence = null)
        {
            try
            {
                // Create JSON message
                var telemetryDataPoint = new
                {
                    carStatus = carStatus.ToString(),
                    presence
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                // Add a custom application property to the message.
                // An IoT hub can filter on these properties without access to the message body.
                message.Properties.Add("temperatureAlert", carStatus == CarSatus.Off && presence == true ? "true" : "false");

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
