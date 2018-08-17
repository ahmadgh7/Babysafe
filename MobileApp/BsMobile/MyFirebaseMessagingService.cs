using Android.App;
using Android.Util;
using Firebase.Messaging;

namespace BsMobile
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        private const string Tag = "MyFirebaseMessagingService";

        public override void OnMessageReceived(RemoteMessage message)
        {
            // TODO(developer): Handle FCM messages here.
            // Not getting messages here? See why this may be: https://goo.gl/39bRNJ
            Log.Debug(Tag, "From: " + message.From);

            // Check if message contains a notification payload.
            if (message.GetNotification() != null)
            {
                Log.Debug(Tag, "Message Notification Body: " + message.GetNotification().Body);
            }
        }
    }
}