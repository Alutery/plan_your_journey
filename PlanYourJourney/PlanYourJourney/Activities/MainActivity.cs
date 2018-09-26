using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Microsoft.WindowsAzure.MobileServices;
using PlanYourJourney.Models;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace PlanYourJourney.Activities
{
    [Activity(Label = "Plan Your Journey", MainLauncher = false)]
    public class MainActivity : AppCompatActivity
    {
        private ViewPager _pager;
        private TabLayout _tabLayout;
        private CustomPagerAdapter _adapter;
        private Toolbar _toolbar;
        public List<Arrangement> List;

        public static bool byDateFilter;
        public static bool byAuthorFilter;
        public static DateTime dateFilter;
        public static string Author;
        public static string MyUsername;

        protected override void OnCreate(Bundle bundle)
        {

            base.OnCreate(bundle);
            CurrentPlatform.Init();

            byDateFilter = false;
            byAuthorFilter = false;

            // Set our view from the "main" layout resource
            base.SetContentView(PlanYourJourney.Resource.Layout.Main);
            List = new List<Arrangement>();
            UpdateData();

            ISharedPreferences settings = GetSharedPreferences("MyPrefsFile", 0);
            MyUsername = settings.GetString("author", "");

            // Find views
            _pager = FindViewById<ViewPager>(PlanYourJourney.Resource.Id.pager);
            _tabLayout = FindViewById<TabLayout>(PlanYourJourney.Resource.Id.sliding_tabs);
            _adapter = new CustomPagerAdapter(this, SupportFragmentManager);
            _toolbar = FindViewById<Toolbar>(PlanYourJourney.Resource.Id.my_toolbar);

            // Setup Toolbar
            SetSupportActionBar(_toolbar);
            base.SupportActionBar.Title = GetString(PlanYourJourney.Resource.String.app_name);

            // Set adapter to view pager
            _pager.Adapter = _adapter;

            // Setup tablayout with view pager
            _tabLayout.SetupWithViewPager(_pager);

            // Iterate over all tabs and set the custom view
            for (int i = 0; i < 3; i++)
            {
                TabLayout.Tab tab = _tabLayout.GetTabAt(i);
                tab.SetCustomView(_adapter.GetTabView(i));
            }

        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            base.MenuInflater.Inflate(Resource.Menu.home, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.ic_action_add_circle)
            {
                var intent = new Intent(this, typeof(AddArrangement));
                base.StartActivity(intent);
            }
            else if (item.ItemId == Resource.Id.ic_action_cached)
            {
                Update();
            }

            return base.OnOptionsItemSelected(item);
        }

        private async void Update()
        {
            try
            {
                List = await ((MainApplication)Application).arrangementTable.ToListAsync();
            }
            catch (Exception e)
            {
                HelpMe.CreateAndShowDialog(e.Message, "Error", this);
            }
            foreach (var i in List)
            {
                Console.Write(i.Date + " ");
            }
            List.Sort((a, b) => a.Date.Date.CompareTo(b.Date.Date));
            _pager.Adapter.NotifyDataSetChanged();
        }

        protected override void OnResume()
        {
            base.OnResume();
            UpdateData();
        }

        private async void UpdateData()
        {
            List = await ((MainApplication)Application).arrangementTable.ToListAsync();
            List.Sort((a, b) => a.Date.CompareTo(b.Date));
        }

        /// <summary>
        /// OnBack is pressed.
        /// </summary>
        public override void OnBackPressed()
        {
            var alert = new Android.App.AlertDialog.Builder(this);
            alert.SetTitle("Really Exit?");
            alert.SetMessage("Are you sure you want to exit?");
            alert.SetPositiveButton("Yes", (senderAlert, args) =>
            {
                base.OnBackPressed();
            });

            alert.SetNegativeButton("No", (senderAlert, args) =>
            {
                Toast.MakeText(this, "Cancelled", ToastLength.Short).Show();
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }
    }
}


