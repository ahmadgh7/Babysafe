using System;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Widget;

namespace BsMobile
{
    [Activity(Theme = "@style/MyTheme.Login", MainLauncher = true, Icon = "@mipmap/ic_launcher")]
    public class LoginActivity : Activity
    {
        private EditText _email;
        private EditText _password;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Login);

            //Get email & password values from edit text
            _email = FindViewById<EditText>(Resource.Id.txtEmail);
            _password = FindViewById<EditText>(Resource.Id.txtPassword);

            //Trigger click event of Login Button
            var button = FindViewById<FloatingActionButton>(Resource.Id.btnLogin);
            button.Click += DoLogin;
        }

        public void DoLogin(object sender, EventArgs e)
        {
            if (_email.Text == _password.Text)
            {
                Toast.MakeText(this, "Login successfully done!", ToastLength.Long).Show();
                MainActivity.DeviceId = _email.Text;
                StartActivity(typeof(MainActivity));
            }
            else
            {
                Toast.MakeText(this, "Wrong credentials!", ToastLength.Long).Show();
            }
        }
    }
}