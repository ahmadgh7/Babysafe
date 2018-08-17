using System;
using System.Net.Http;
using System.Threading.Tasks;
using Android.App;
using Android.Util;
using Firebase.Iid;

namespace BsMobile
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseInsatnceIdService : FirebaseInstanceIdService
    {
        private const string Tag = "MyFirebaseInsatnceIdService";
        private const string Uri = "https://babysafewebapi.azurewebsites.net/api/UpdateToken";

        public override void OnTokenRefresh()
        {
            // Get updated InstanceID token.
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(Tag, "Refreshed token: " + refreshedToken);

            SendRegistrationToServer(MainActivity.DeviceId, refreshedToken).Wait();
        }

        public static async Task SendRegistrationToServer(string deviceId, string refreshedToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{Uri}?deviceId={deviceId}&token={refreshedToken}");
                    Log.Debug(Tag,
                        response.IsSuccessStatusCode
                            ? "Refreshed token successfully registered to the server"
                            : "Error registering refreshed token to the server");
                }
            }
            catch (Exception ex)
            {
                Log.Debug(Tag, "Error registering refreshed token to the server: " + ex);
            }
        }
    }
}