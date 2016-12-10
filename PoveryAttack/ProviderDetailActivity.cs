using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using SQLite;
using Newtonsoft.Json;

namespace PoveryAttack
{
    [Activity(Label = "ProviderDetailActivity")]
    public class ProviderDetailActivity : Activity
    {
        List<ProviderOrg> items;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "providers.db3");

            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.ProviderDetail);

            //what provider do we want?
            int resourceID = Intent.GetIntExtra("id", 0);

            //load the json data to populate a list of objects
            Android.Content.Res.AssetManager assets = this.Assets;
            using (StreamReader sr = new StreamReader(assets.Open("services.json")))
            {
                string json = sr.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<ProviderOrg>>(json);
            }

            //get the specific record for the recordID
            var record = from i in items
                         where i.RESOURCEID == resourceID
                         select i;

            foreach(var r in record)
            {
                TextView providerName = FindViewById<TextView>(Resource.Id.providerNameBox);
                providerName.Text = r.RESOURCENAME;
                TextView address1 = FindViewById<TextView>(Resource.Id.address1Box);
                address1.Text = r.ADDRESS1;
                TextView address2 = FindViewById<TextView>(Resource.Id.address2Box);
                address2.Text = r.ADDRESS2;
                string cityStateZip = r.CITY + ", " + r.STATE + " " + r.ZIP;
                TextView city = FindViewById<TextView>(Resource.Id.cityBox);
                city.Text = cityStateZip;
                TextView county = FindViewById<TextView>(Resource.Id.countyBox);
                county.Text = r.COUNTY + " County";
                TextView phone = FindViewById<TextView>(Resource.Id.phoneBox);
                phone.Text = r.PHONE;
                TextView services = FindViewById<TextView>(Resource.Id.servicesBox);
                services.Text = r.SERVICES;
                ImageButton fbButton = FindViewById<ImageButton>(Resource.Id.fbButton);
                if(r.FACEBOOKURL == "")
                {
                    fbButton.Enabled = false;
                }
                fbButton.Click += delegate
                {
                    var uri = Android.Net.Uri.Parse(r.FACEBOOKURL);
                    var intent = new Intent(Intent.ActionView, uri);
                    StartActivity(intent);
                };

                ImageButton websiteButton = FindViewById<ImageButton>(Resource.Id.websiteButton);
                if (r.WEBPAGE == "")
                {
                    fbButton.Enabled = false;
                }
                websiteButton.Click += delegate
                {
                    var uri = Android.Net.Uri.Parse(r.WEBPAGE);
                    var intent = new Intent(Intent.ActionView, uri);
                    StartActivity(intent);
                };

                ImageButton emailButton = FindViewById<ImageButton>(Resource.Id.emailButton);
                if (r.EMAIL == "")
                {
                    emailButton.Enabled = false;
                }
                emailButton.Click += delegate
                {
                    var email = new Intent(Android.Content.Intent.ActionSend);
                    email.PutExtra(Android.Content.Intent.ExtraEmail, new string[] { r.EMAIL });

                    email.PutExtra(Android.Content.Intent.ExtraSubject, "Email Message from App");
                    email.SetType("message/rfc822");
                    StartActivity(email);
                };

            }

            


        }
    }
}