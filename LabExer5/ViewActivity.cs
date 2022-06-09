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
    [Activity(Label = "ViewActivity")]
    public class ViewActivity : Activity
    {
        //readonly string IP_ADDRESS = "192.168.1.130"; //mark
        readonly string IP_ADDRESS = "192.168.18.4"; //charmaine

        EditText editName, editSchool, searchName;
        Button btnUpdate, btnHome, btnDelete;
        RadioGroup gender;
        AutoCompleteTextView autoCompleteCountry;
        HttpWebResponse nextResponse;
        HttpWebRequest nextRequest;
        string name = "", school = "", country = "", selectedGender = "", res = "";
        int record_ID = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.view_layout);

            editName = FindViewById<EditText>(Resource.Id.editName);
            editSchool = FindViewById<EditText>(Resource.Id.editSchool);
            searchName = FindViewById<EditText>(Resource.Id.searchName);
            btnUpdate = FindViewById<Button>(Resource.Id.buttonUpdate);
            btnHome = FindViewById<Button>(Resource.Id.buttonHome);
            btnDelete = FindViewById<Button>(Resource.Id.buttonDelete);

            gender = FindViewById<RadioGroup>(Resource.Id.radioGroup1);
            gender.CheckedChange += myRadioGroup_CheckedChange;

            autoCompleteCountry = FindViewById<AutoCompleteTextView>(Resource.Id.autoCompleteTextViewCountry);
            var country = new string[] { "Cambodia", "Indonesia", "Philippines" };
            ArrayAdapter adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, country);
            autoCompleteCountry.Adapter = adapter;

            editName.Text = Intent.GetStringExtra("Name");
            editSchool.Text = Intent.GetStringExtra("School");
            autoCompleteCountry.Text = Intent.GetStringExtra("Country");
            gender.Check(Convert.ToInt32(Intent.GetStringExtra("Gender")));
            record_ID = Convert.ToInt32(Intent.GetStringExtra("ID"));

            btnHome.Click += this.BackHome;
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

        public void BackHome(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(HomeActivity));
            StartActivity(i);
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
                catch (Exception)
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