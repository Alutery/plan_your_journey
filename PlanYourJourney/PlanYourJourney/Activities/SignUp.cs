using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using PlanYourJourney.Models;

namespace PlanYourJourney.Activities
{
    [Activity(Label = "SignUp")]
    public class SignUp : Activity
    {
        private EditText _username, _password, _email;
        private Button _signUpBtn, _cancelBtn;
        private string userInput, passInput, emailInput;
        InputMethodManager _inputManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            // Set our view from the "main" layout resource
            base.SetContentView(Resource.Layout.SignUp_layout);

            var layout = FindViewById<RelativeLayout>(Resource.Id.drawer);

            _username = FindViewById<EditText>(Resource.Id.userName);
            _password = FindViewById<EditText>(Resource.Id.password);
            _email = FindViewById<EditText>(Resource.Id.email);

            _signUpBtn = FindViewById<Button>(Resource.Id.sign_up);
            _cancelBtn = FindViewById<Button>(Resource.Id.cancel);

            _inputManager = (InputMethodManager)this.GetSystemService(Context.InputMethodService);


            _signUpBtn.Click += (sender, args) =>
            {
                HideKeyboard();
                userInput = _username.Text;
                passInput = _password.Text;
                emailInput = _email.Text;
                if (userInput != "" && passInput != "" && emailInput != "")
                {
                    if (SendEmail.IsValidEmail(emailInput))
                    {
                        if(IsValidUsername(userInput))
                        {
                            AddUser();
                            Finish();
                        }
                        else
                        {
                            Snackbar.Make(layout, "Username already exists.", Snackbar.LengthLong)
                                .SetAction("OK", (v) => { })
                                .Show();
                        }
                    }
                    else
                    {
                        Snackbar.Make(layout, "Incorrect email.", Snackbar.LengthLong)
                            .SetAction("OK", (v) => { })
                            .Show();
                    }

                }
                else
                {
                    Snackbar.Make(layout, "Incorrect input.", Snackbar.LengthLong)
                        .SetAction("OK", (v) => { })
                        .Show();
                }

            };

            _cancelBtn.Click += (sender, args) => { Finish(); };
        }

        async void AddUser()
        {
            var item = new Account
            {
                UserName = userInput,
                Password = passInput,
                Email = emailInput
            };

            try
            {
                // Insert the new item into the local store.
                await((MainApplication)Application).accountTable.InsertAsync(item);
                Toast.MakeText(this, "Success"  , ToastLength.Long).Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " - Error");
            }
        }

        bool IsValidUsername(string username)
        {
            var list = Login.List;
            foreach (var i in list)
            {
                if (i.UserName == username)
                    return false;
            }

            return true;
        }

        public void HideKeyboard()
        {
            _inputManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
        }
    }
}