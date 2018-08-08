using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace BsPiClient
{
    public sealed partial class MainPage
    {
        private static DeviceClient _deviceClient;
        //private static DeviceClient _featherDeviceClient;

        public MainPage()
        {
            InitializeComponent();

            try
            {
                //deviceClient = DeviceClient.Create(iotHubUri, AuthenticationMethodFactory.CreateAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey), TransportType.Http1);
                _deviceClient = DeviceClient.CreateFromConnectionString(connectionString);
                //_featherDeviceClient = DeviceClient.CreateFromConnectionString("HostName=Babysafe.azure-devices.net;DeviceId=Feather;SharedAccessKey=RgkH/brtmbZa3mBAZlmfoL25T9E4I8LFQxwDExW3i8s=");

                SendDeviceToCloudMessagesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async void SendDeviceToCloudMessagesAsync()
        {
            //var weatherDataprovider = await WeatherDataProvider.Create();

            // Use this if you don't have a real sensor:
            var weatherDataprovider = await SimulatedWeatherDataProvider.Create();

            while (true)
            {
                var currentHumidity = weatherDataprovider.GetHumidity();
                var currentTemperature = weatherDataprovider.GetTemperature();
                //var temperatureAlert = currentTemperature > 30;


                var telemetryDataPoint = new
                {
                    time = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                    deviceId,
                    currentHumidity,
                    currentTemperature,
                    temperatureAlert = true
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                await _deviceClient.SendEventAsync(message);
                //await _featherDeviceClient.SendEventAsync(message);

                Debug.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                listView.Items?.Insert(0, messageString);

                await Task.Delay(1000);
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
