using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Microsoft.WindowsAzure.MobileServices;
using PlanYourJourney.Models;
using Steelkiwi.Com.Library;

namespace PlanYourJourney.Activities
{
    [Activity(Label = "Login", Theme = "@style/Theme.AppCompat", MainLauncher = false)]
    public class Login : Activity
    {
        private static readonly string TAG = "Login";

        public static List<Account> List;
        InputMethodManager _inputManager;
        private TextView forgotPassword;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //CurrentPlatform.Init();
            // Create your application here
            // Set our view from the "main" layout resource
            base.SetContentView(Resource.Layout.Login_layout);
            Log.Debug(TAG, "Login.OnCreate");
            //Initializing button from layout
            var login = FindViewById<Button>(Resource.Id.login);
            var signUp = FindViewById<Button>(Resource.Id.sign_up);
            var layoutHere = FindViewById<RelativeLayout>(Resource.Id.drawer);
            //dots = FindViewById<DotsLoaderView>(Resource.Id.dotsLoaderView);
            //_progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

            EditText edit = FindViewById<EditText>(PlanYourJourney.Resource.Id.userName);
            EditText password = FindViewById<EditText>(Resource.Id.password);

            ISharedPreferences settings = GetSharedPreferences("MyPrefsFile", 0);
            List = new List<Account>();

            //dots.Show();
           
             UpdateFirst();            
            //dots.Hide();

            forgotPassword = FindViewById<TextView>(Resource.Id.forgot_password);

            _inputManager = (InputMethodManager) this.GetSystemService(Context.InputMethodService);


            if (settings.GetBoolean("my_first_time", true))
            {
                //the app is being launched for first time, do something        
                Console.WriteLine("Comments - First time");

                // first time task



                // record the fact that the app has been started at least once
                //settings.Edit().PutBoolean("my_first_time", false).Apply();
            }
            else
            {
                Console.WriteLine("Comments - Second time");

                edit.Text = settings.GetString("author", "");
                password.Text = settings.GetString("password", "");

            }

            //SignUp button click action
            signUp.Click += (object sender, EventArgs e) =>
            {
                Log.Debug(TAG, "HideKeyboard");
                HideKeyboard();
                Log.Debug(TAG, "Update()");
                Update();
                Log.Debug(TAG, " var intent = new Intent(this, typeof(SignUp));");
                var intent = new Intent(this, typeof(SignUp));
                Log.Debug(TAG, "StartActivity(intent);");
                StartActivity(intent);
                
            };

            //Login button click action
            login.Click += (object sender, EventArgs e) =>
            {
                try
                {
                    Update();

                    string username = edit.Text;
                    string pass = password.Text;
                    
                    HideKeyboard();

                    if (CorrectLogin(username, pass))
                    {
                        Toast.MakeText(this, username, ToastLength.Long).Show();

                        settings.Edit().PutString("author", username).Apply();
                        settings.Edit().PutString("password", pass).Apply();
                        settings.Edit().PutBoolean("my_first_time", false).Apply();

                        var intent = new Intent(this, typeof(MainActivity));
                        StartActivity(intent);
                        Finish();
                    }
                    else
                    {
                        edit.Text = "";
                        password.Text = "";
                        Snackbar.Make(layoutHere, "Incorrect input.", Snackbar.LengthLong)
                            .SetAction("OK", (v) => { })
                            .Show();
                    }
                }
                catch (Exception exp)
                {
                    Toast.MakeText(this, exp.Message, ToastLength.Long).Show();
                }
            };

            forgotPassword.Click += (object sender, EventArgs e) =>
            {
                LayoutInflater layoutInflater = LayoutInflater.From(this);
                View view = layoutInflater.Inflate(Resource.Layout.InputEmail_Fragment, null);
                Android.Support.V7.App.AlertDialog.Builder alertbuilder =
                    new Android.Support.V7.App.AlertDialog.Builder(this);
                alertbuilder.SetView(view);
                var userdata = view.FindViewById<EditText>(Resource.Id.email_input);
                alertbuilder.SetCancelable(false)
                    .SetPositiveButton("Submit", delegate
                    {
                        string email = userdata.Text;
                        if (SendEmail.IsValidEmail(email) && FindEmail(email, out string user, out string pass))
                        {
                            SendEmail.Send(email, user, pass);
                            Toast.MakeText(this, "Sent to " + email, ToastLength.Short).Show();
                        }
                        else
                        {
                            Toast.MakeText(this, "Email does not exist", ToastLength.Short).Show();
                        }
                    })
                    .SetNegativeButton("Cancel", delegate { alertbuilder.Dispose(); });
                Android.Support.V7.App.AlertDialog dialog = alertbuilder.Create();
                dialog.Show();
            };
        }

        private async void Update()
        {
            try
            {
                List = await ((MainApplication)Application).accountTable.ToListAsync();
            }
            catch (Exception ex)
            {
                
            }
        }

        public void HideKeyboard()
        {
            _inputManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
        }

        private bool CorrectLogin(string username, string password)
        {
            Update();
            foreach (var i in List)
            {
                if (i.UserName == username && i.Password == password)
                    return true;
            }

            return false;
        }

        private static bool FindEmail(string email, out string username, out string password)
        {
            username = null;
            password = null;
            foreach (var i in List)
            {
                if (i.Email != email) continue;
                username = i.UserName;
                password = i.Password;
                return true;
            }

            return false;
        }

        private void UpdateFirst()
        {
            try
            {
                Task t = Task.Run(UpdateFirstFisrt);
                t.Wait();
            }
            catch
        }

        private async Task UpdateFirstFisrt()
        {
            List = await ((MainApplication)Application).accountTable.ToListAsync();
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