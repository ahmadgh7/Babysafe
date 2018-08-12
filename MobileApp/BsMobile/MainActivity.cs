using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Util;
using Firebase;
using Firebase.Iid;

namespace BsMobile
{
    [Activity(Label = "@string/app_name")]
    public class MainActivity : Activity
    {
        private const string Tag = "MainActivity";

        public static string DeviceId { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // Initialize FireBase
            FirebaseApp.InitializeApp(Application.Context);

            // Get updated InstanceID token.
            UpdateToken().Wait();
        }

        private static async Task UpdateToken()
        {
            var token = FirebaseInstanceId.Instance.Token;
            Log.Debug(Tag, "Token: " + token);
            await MyFirebaseInsatnceIdService.SendRegistrationToServer(DeviceId, token);
        }
    }
}

