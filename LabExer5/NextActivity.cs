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
    [Activity(Label = "NextActivity")]
    public class NextActivity : Activity
    {
        //readonly string IP_ADDRESS = "192.168.1.130"; //mark
        readonly string IP_ADDRESS = "192.168.18.4"; //charmaine

        EditText editName, editSchool, searchName;
        Button btnAdd, btnSearch, btnUpdate, btnHome, btnDelete;
        RadioGroup gender;
        AutoCompleteTextView autoCompleteCountry;
        HttpWebResponse nextResponse;
        HttpWebRequest nextRequest;
        string name = "", school = "", country = "", selectedGender = "", nameToBeSearched = "", res = "", str = "", login_name = "";
        int record_ID = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.crud_layout);
            login_name = Intent.GetStringExtra(login_name);

            editName = FindViewById<EditText>(Resource.Id.editName);
            editSchool = FindViewById<EditText>(Resource.Id.editSchool);
            searchName = FindViewById<EditText>(Resource.Id.searchName);
            btnAdd = FindViewById<Button>(Resource.Id.buttonAdd);
            btnSearch = FindViewById<Button>(Resource.Id.buttonSearch);
            btnUpdate = FindViewById<Button>(Resource.Id.buttonUpdate);
            btnHome = FindViewById<Button>(Resource.Id.buttonHome);
            btnDelete = FindViewById<Button>(Resource.Id.buttonDelete);

            gender = FindViewById<RadioGroup>(Resource.Id.radioGroup1);
            gender.CheckedChange += myRadioGroup_CheckedChange;

            autoCompleteCountry = FindViewById<AutoCompleteTextView>(Resource.Id.autoCompleteTextViewCountry);
            var country = new string[] { "Cambodia", "Indonesia", "Philippines" };
            ArrayAdapter adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, country);
            autoCompleteCountry.Adapter = adapter;

            btnAdd.Click += this.AddRecord;
            btnHome.Click += this.BackHome;
            btnSearch.Click += this.SearchRecord;
            btnUpdate.Click += this.UpdateRecord;
            btnDelete.Click += this.DeleteRecord;
        }

        public void myRadioGroup_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            int checkedItemId = gender.CheckedRadioButtonId;
            RadioButton checkedRadioButton = FindViewById<RadioButton>(checkedItemId);
            selectedGender = checkedItemId.ToString();
            gender.Check(checkedItemId);
        }

        public void AddRecord(object sender, EventArgs e)
        {
            if (editName.Text == "" ||
                editSchool.Text == "" ||
                autoCompleteCountry.Text == "")
            {
                Toast.MakeText(this, "FAILED: Fill out the missing fields and try again.", ToastLength.Long).Show();
            }
            else
            {
                name = editName.Text;
                school = editSchool.Text;
                country = autoCompleteCountry.Text;

                try
                {
                    nextRequest = (HttpWebRequest)WebRequest.Create("http://" + IP_ADDRESS + "/IT140P/REST/search_record.php?name=" + name);
                    nextResponse = (HttpWebResponse)nextRequest.GetResponse();
                    res = nextResponse.ProtocolVersion.ToString();
                    StreamReader reader = new StreamReader(nextResponse.GetResponseStream());
                    var result = reader.ReadToEnd();

                    using JsonDocument doc = JsonDocument.Parse(result);
                    JsonElement root = doc.RootElement;
                    var searchedElement = root[0];
                    string searchedName = searchedElement.GetProperty("name").ToString();
                    Toast.MakeText(this, "Student Name '" + name + "' already exists!", ToastLength.Long).Show();
                }
                catch (Exception)
                {
                    nextRequest = (HttpWebRequest)WebRequest.Create("http://" + IP_ADDRESS + "/IT140P/REST/add_record.php?name=" + name + " &school=" + school + " &country=" + country + " &gender=" + selectedGender);
                    nextResponse = (HttpWebResponse)nextRequest.GetResponse();
                    StreamReader reader = new StreamReader(nextResponse.GetResponseStream());
                    res = reader.ReadToEnd();
                    Toast.MakeText(this, res, ToastLength.Long).Show();

                    editName.Text = "";
                    editSchool.Text = "";
                    autoCompleteCountry.Text = "";
                    gender.Check(0);
                }             
            }
        }

        public void BackHome(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(MainActivity));
            StartActivity(i);
        }

        public void SearchRecord(object sender, EventArgs e)
        {
            nameToBeSearched = searchName.Text;
            if (nameToBeSearched == "") { nameToBeSearched = "N/A"; }

            nextRequest = (HttpWebRequest)WebRequest.Create("http://" + IP_ADDRESS + "/IT140P/REST/search_record.php?name=" + nameToBeSearched);
            nextResponse = (HttpWebResponse)nextRequest.GetResponse();
            res = nextResponse.ProtocolVersion.ToString();
            StreamReader reader = new StreamReader(nextResponse.GetResponseStream());
            var result = reader.ReadToEnd();

            using JsonDocument doc = JsonDocument.Parse(result);
            JsonElement root = doc.RootElement;

            try
            {
                var searchedElement = root[0];
                int searchedID = Convert.ToInt32(searchedElement.GetProperty("student_ID").ToString());
                string searchedName = searchedElement.GetProperty("name").ToString();
                string searchedSchool = searchedElement.GetProperty("school").ToString();
                string searchedCountry = searchedElement.GetProperty("country").ToString();
                int searchedGender = Convert.ToInt32(searchedElement.GetProperty("gender").ToString());

                editName.Text = searchedName;
                editSchool.Text = searchedSchool;
                autoCompleteCountry.Text = searchedCountry;
                gender.Check(searchedGender);
                record_ID = searchedID;

                Toast.MakeText(this, searchedGender.ToString(), ToastLength.Long).Show();
            }
            catch(Exception)
            {
                Toast.MakeText(this, "Data not found!", ToastLength.Long).Show();
            }
        }

        // LOGIC: Before the user can update a record, the user must search a student record first.
        public void UpdateRecord(object sender, EventArgs e)
        {
            if (editName.Text == "" ||
                editSchool.Text == "" ||
                autoCompleteCountry.Text == "")
            {
                Toast.MakeText(this, "FAILED: Search an existing record or fill the missing fields and try again.", ToastLength.Long).Show();
            }
            else
            {
                try
                {
                    name = editName.Text;
                    school = editSchool.Text;
                    country = autoCompleteCountry.Text;

                    nextRequest = (HttpWebRequest)WebRequest.Create("http://" + IP_ADDRESS + "/IT140P/REST/update_record.php?student_ID=" + record_ID + " &name=" + name + " &school=" + school + " &country=" + country + " &gender=" + selectedGender);
                    nextResponse = (HttpWebResponse)nextRequest.GetResponse();
                    StreamReader reader = new StreamReader(nextResponse.GetResponseStream());
                    res = reader.ReadToEnd();
                    Toast.MakeText(this, res, ToastLength.Long).Show();

                    editName.Text = "";
                    editSchool.Text = "";
                    autoCompleteCountry.Text = "";
                    gender.Check(0);
                }
                catch(Exception)
                {
                    Toast.MakeText(this, "FAILED: Student data cannot be found.", ToastLength.Long).Show();
                }
            }
        }

        public void DeleteRecord(object sender, EventArgs e)
        {
            name = editName.Text;
            school = editSchool.Text;
            country = autoCompleteCountry.Text;

            nextRequest = (HttpWebRequest)WebRequest.Create("http://" + IP_ADDRESS + "/IT140P/REST/delete_record.php?name=" + name + " &school=" + school + " &country=" + country + " &gender=" + selectedGender);
            nextResponse = (HttpWebResponse)nextRequest.GetResponse();
            StreamReader reader = new StreamReader(nextResponse.GetResponseStream());
            res = reader.ReadToEnd();
            Toast.MakeText(this, res, ToastLength.Long).Show();

            editName.Text = "";
            editSchool.Text = "";
            autoCompleteCountry.Text = "";
            gender.Check(0);
        }
    }
}