using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using PlanYourJourney.Activities;

namespace PlanYourJourney
{
    [Activity(Label = "Plan Your Journey", Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = false)]
    public class SplashActivity : Activity
    {
        static readonly string Tag = "X:" + typeof(SplashActivity).Name;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Intent intent = new Intent(this, typeof(Login));
            StartActivity(intent);
            Finish();
            // Create your application here
            Log.Debug(Tag, "SplashActivity.OnCreate");
        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            //Task startupWork = new Task(() => { SimulateStartup(); });
            //startupWork.Start();
        }

        // Simulates background work that happens behind the splash screen
        async void SimulateStartup()
        {
            Log.Debug(Tag, "Performing some startup work that takes a bit of time.");
            await Task.Delay(1000); // Simulate a bit of startup work.
            Log.Debug(Tag, "Startup work is finished - starting MainActivity.");
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }

        public override void OnBackPressed() { }

    }
}