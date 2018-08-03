using Android.App;
using Android.OS;
using Android.Util;
using Firebase;
using Firebase.Iid;

namespace BsMobile
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private string TAG = "MainActivity";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // Initialize FireBase
            FirebaseApp.InitializeApp(Application.Context);

            // Get updated InstanceID token.
            var token = FirebaseInstanceId.Instance.Token;
            Log.Debug(TAG, "Token: " + token);
        }
    }
}

