using Android.App;
using Android.Util;
using Firebase.Iid;

namespace BsMobile
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseInsatnceIdService : FirebaseInstanceIdService
    {
        private string TAG = "MyFirebaseInsatnceIdService";

        public override void OnTokenRefresh()
        {
            // Get updated InstanceID token.
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(TAG, "Refreshed token: " + refreshedToken);

            // If you want to send messages to this application instance or
            // manage this apps subscriptions on the server side, send the
            // Instance ID token to your app server.
            SendRegistrationToServer(refreshedToken);
        }

        private void SendRegistrationToServer(string refreshedToken)
        {
        }

    }
}