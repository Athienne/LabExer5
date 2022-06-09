using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;

namespace LabExer5
{
    [Activity(Label = "HomeActivity")]
    public class HomeActivity : Activity
    {
        Button btnAdd, btnSearch, btnLogout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.home_layout);

            btnAdd = FindViewById<Button>(Resource.Id.buttonAdd);
            btnSearch = FindViewById<Button>(Resource.Id.buttonSearch);
            btnLogout = FindViewById<Button>(Resource.Id.buttonLogout);

            btnAdd.Click += this.GoToAdd;
            btnSearch.Click += this.GoToSearch;
            btnLogout.Click += this.Logout;
        }

        public void GoToAdd(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(AddActivity));
            StartActivity(i);
        }

        public void GoToSearch(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(SearchActivity));
            StartActivity(i);
        }

        public void Logout(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(MainActivity));
            StartActivity(i);
        }
    }
}