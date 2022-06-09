using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using System.IO;
using System.Net;

namespace LabExer5
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        //readonly string IP_ADDRESS = "192.168.1.130"; //mark
        readonly string IP_ADDRESS = "192.168.18.4"; //charmaine

        EditText usernameET, passwordET;
        Button loginBTN;
        HttpWebResponse response;
        HttpWebRequest request;
        string res = "";
        string uname = "";
        string pword = "";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.login_layout);

            usernameET = FindViewById<EditText>(Resource.Id.usernameET);
            passwordET = FindViewById<EditText>(Resource.Id.passwordET);
            loginBTN = FindViewById<Button>(Resource.Id.loginBTN);

            loginBTN.Click += this.Login;
        }

        public void Login(object sender, EventArgs e)
        {
            uname = usernameET.Text;
            pword = passwordET.Text;
            request = (HttpWebRequest)WebRequest.Create("http://" + IP_ADDRESS + "/IT140P/REST/admin_login.php?uname=" + uname + " &pword=" + pword);
            response = (HttpWebResponse)request.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream());
            res = reader.ReadToEnd();
            Toast.MakeText(this, res, ToastLength.Long).Show();

            if (res.Contains("OK!"))
            {
                Intent i = new Intent(this, typeof(HomeActivity));
                StartActivity(i);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}