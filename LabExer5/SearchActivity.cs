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
    [Activity(Label = "SearchActivity")]
    public class SearchActivity : Activity
    {
        //readonly string IP_ADDRESS = "192.168.1.130"; //mark
        readonly string IP_ADDRESS = "192.168.18.4"; //charmaine

        EditText searchName;
        Button btnSearch, btnHome;
        HttpWebResponse nextResponse;
        HttpWebRequest nextRequest;
        string nameToBeSearched = "", res = "";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.search_layout);

            searchName = FindViewById<EditText>(Resource.Id.searchName);
            btnSearch = FindViewById<Button>(Resource.Id.buttonSearch);
            btnHome = FindViewById<Button>(Resource.Id.buttonHome);

            btnHome.Click += this.BackHome;
            btnSearch.Click += this.SearchRecord;
        }

        public void BackHome(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(HomeActivity));
            StartActivity(i);
        }

        public void SearchRecord(object sender, EventArgs e)
        {
            nameToBeSearched = searchName.Text;
            if (nameToBeSearched == "") { nameToBeSearched = "N/A"; }

            try
            {
                nextRequest = (HttpWebRequest)WebRequest.Create("http://" + IP_ADDRESS + "/IT140P/REST/search_record.php?name=" + nameToBeSearched);
                nextResponse = (HttpWebResponse)nextRequest.GetResponse();
                res = nextResponse.ProtocolVersion.ToString();
                StreamReader reader = new StreamReader(nextResponse.GetResponseStream());
                var result = reader.ReadToEnd();

                using JsonDocument doc = JsonDocument.Parse(result);
                JsonElement root = doc.RootElement;

                var searchedElement = root[0];
                int searchedID = Convert.ToInt32(searchedElement.GetProperty("student_ID").ToString());
                string searchedName = searchedElement.GetProperty("name").ToString();
                string searchedSchool = searchedElement.GetProperty("school").ToString();
                string searchedCountry = searchedElement.GetProperty("country").ToString();
                int searchedGender = Convert.ToInt32(searchedElement.GetProperty("gender").ToString());

                Toast.MakeText(this, searchedGender.ToString(), ToastLength.Long).Show();

                Intent i = new Intent(this, typeof(ViewActivity));
                i.PutExtra("Name", searchedName);
                i.PutExtra("School", searchedSchool);
                i.PutExtra("Country", searchedCountry);
                i.PutExtra("Gender", searchedGender);
                i.PutExtra("ID", searchedID);
                StartActivity(i);
            }
            catch (Exception)
            {
                Toast.MakeText(this, "Data not found!", ToastLength.Long).Show();
            }
        }
    }
}