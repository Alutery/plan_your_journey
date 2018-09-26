using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Com.KD.Dynamic.Calendar.Generator;
using Java.Util;
using Microsoft.WindowsAzure.MobileServices;
using PlanYourJourney.Activities.Fragments;
using PlanYourJourney.Models;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace PlanYourJourney.Activities
{
    [Activity(Label = "EditArrangement")]
    public class EditArrangement : AppCompatActivity, DatePickerDialog.IOnDateSetListener
    {
        private TextInputLayout _titleLayout;
        private TextInputLayout _locationLayout;
        InputMethodManager _inputManager;

        private EditText _mDateEditext;
        private Calendar _mCurrentDate;
        private ImageGenerator _mGeneratorImage;
        private Arrangement _argArrangement;


        private EditText _title, _content, _location, _date;
        private string _filePath = null;
        private string id, auth;

        private FragmentManager _fm;
        private SingleChoiceDialogFragment _choice;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CurrentPlatform.Init();

            // Create your application here
            base.SetContentView(PlanYourJourney.Resource.Layout.Edit_Arrangement_layout);
            var toolbar = FindViewById<Toolbar>(PlanYourJourney.Resource.Id.toolbar);

            //Toolbar will now take on default actionbar characteristics
            SetSupportActionBar(toolbar);

            SupportActionBar.Title = "Edit";

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);


            _titleLayout = (TextInputLayout)base.FindViewById(PlanYourJourney.Resource.Id.name_layout);
            _locationLayout = (TextInputLayout)base.FindViewById(PlanYourJourney.Resource.Id.place_layout);

            _inputManager = (InputMethodManager)this.GetSystemService(Context.InputMethodService);


            _mGeneratorImage = new ImageGenerator(this);
            _mDateEditext = FindViewById<EditText>(PlanYourJourney.Resource.Id.txtDateEdited);

            _mGeneratorImage.SetIconSize(50, 50);
            _mGeneratorImage.SetDateSize(30);
            _mGeneratorImage.SetMonthSize(10);

            _mGeneratorImage.SetDatePosition(42);
            _mGeneratorImage.SetMonthPosition(14);

            _mGeneratorImage.SetDateColor(Color.Blue);
            _mGeneratorImage.SetMonthColor(Color.White);

            _mGeneratorImage.SetStorageToSDCard(true);

            _mDateEditext.Click += delegate
            {
                _mCurrentDate = Calendar.Instance;
                int mYear = _mCurrentDate.Get(CalendarField.Year);
                int mMonth = _mCurrentDate.Get(CalendarField.Month);
                int mDay = _mCurrentDate.Get(CalendarField.DayOfMonth);

                DatePickerDialog mDate = new DatePickerDialog(this, this, mYear, mMonth, mDay);
                mDate.Show();
            };

            _title = FindViewById<EditText>(PlanYourJourney.Resource.Id.name_edittext);
            _content = FindViewById<EditText>(PlanYourJourney.Resource.Id.notes_edittext);
            _location = FindViewById<EditText>(PlanYourJourney.Resource.Id.place_edittext);
            _date = FindViewById<EditText>(PlanYourJourney.Resource.Id.txtDateEdited);
            

            long ticks = Intent.GetLongExtra("Date", 0);
            var date = new DateTime(ticks);
            _date.Text = $"{date.Day:00}-{date.Month:00}-{date.Year:0000}";
            id = Intent.GetStringExtra("Id");

            _argArrangement = new Arrangement
            {
                Id = id,
                Title = Intent.GetStringExtra("Title"),
                Date = date,
                Location = Intent.GetStringExtra("Location"),
                ImageResourcePath = Intent.GetStringExtra("Img"),
                Contents = Intent.GetStringExtra("Contents"),
                Author = Intent.GetStringExtra("Author")
            };
            _filePath = _argArrangement.ImageResourcePath;

            _title.Text = _argArrangement.Title;
            _location.Text = _argArrangement.Location;
            _content.Text = _argArrangement.Contents; 
            _date.Text = _argArrangement.Date.ToString("dd-MM-yyyy");
            _filePath = _argArrangement.ImageResourcePath;
            auth = _argArrangement.Author;

            _fm = this.FragmentManager;
            _choice = new SingleChoiceDialogFragment();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            base.MenuInflater.Inflate(PlanYourJourney.Resource.Menu.actionbar_add, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public bool OnClick()
        {
            HideKeyboard();
            
            if (string.IsNullOrWhiteSpace(_titleLayout.EditText.Text))
            {
                Snackbar.Make(_titleLayout, "Fill in title.", Snackbar.LengthLong)
                    .SetAction("OK", (v) => { })
                    .Show();
                return false;
            }
            else if (string.IsNullOrWhiteSpace(_locationLayout.EditText.Text))
            {
                Snackbar.Make(_locationLayout, "Invalid location", Snackbar.LengthLong)
                    .SetAction("Set as unknown", (v) => { _locationLayout.EditText.Text = "Unknown"; })
                    .Show();
                return false;
            }
            else
            {
                return true;
            }
        }

        public void HideKeyboard()
        {
            _inputManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                alert.SetTitle("Do you what to save chaged?");
                alert.SetPositiveButton("Yes", (senderAlert, args) => {
                    if(OnClick()){
                        Toast.MakeText(this, "Saved", ToastLength.Short).Show();
                        Save_Click();
                        var myIntent = new Intent(this, typeof(ArrangementScreen));
                        SetResult(Result.Ok, myIntent);
                        Finish();
                    }
                    else
                    {
                        Toast.MakeText(this, "Incorrect", ToastLength.Short).Show();
                    }
                });

                alert.SetNegativeButton("No", (senderAlert, args) => {
                    Toast.MakeText(this, "Cancelled", ToastLength.Short).Show();
                    Finish();
                });

                Dialog dialog = alert.Create();
                dialog.Show();
                
            }
            else if (item.ItemId == Resource.Id.menu_save)
            {
                Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                alert.SetTitle("Confirm saving");
                alert.SetPositiveButton("Save", (senderAlert, args) =>
                {
                    if (OnClick())
                    {
                        Toast.MakeText(this, "Saved", ToastLength.Short).Show();
                        Save_Click();
                        var myIntent = new Intent(this, typeof(ArrangementScreen));
                        SetResult(Result.Ok, myIntent);
                        base.Finish();
                    }
                    else
                        Toast.MakeText(this, "Incorrect", ToastLength.Short).Show();

                });

                alert.SetNegativeButton("Cancel", (senderAlert, args) => {
                    Toast.MakeText(this, "Cancelled", ToastLength.Short).Show();
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            }
            else if (item.ItemId == Resource.Id.add_a_photo)
            {
                _choice.Show(_fm, "choose_image");
            }
            
            return base.OnOptionsItemSelected(item);
        }

        public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
        {
            _mDateEditext.Text = $"{dayOfMonth:00}-{month:00}-{year:0000}";
            _mCurrentDate.Set(year, month, dayOfMonth);
        }

        private async void Save_Click()
        {
            /*((MainApplication)Application).ArrangementRepository.EditArrangement(
                _title.Text, DateTime.ParseExact(_date.Text, "dd-mm-yyyy",
                    System.Globalization.CultureInfo.InvariantCulture), _location.Text, _filePath,_content.Text, _argArrangement);
        */
            try
            {
                // Insert the new item into the local store.
                _argArrangement = new Arrangement
                {
                    Id = id,
                    Title = _title.Text,
                    Date = DateTime.ParseExact(_date.Text, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture),
                    Location = _location.Text,
                    ImageResourcePath = _filePath,
                    Contents = _content.Text,
                    Author = auth
                };
                await ((MainApplication)Application).arrangementTable.UpdateAsync(_argArrangement);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " - Error");
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Android.Content.Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 0 && data != null)
            {
                // TestDialog
                var result = data.GetBundleExtra("Result");
                var image = result.GetString("ImageName");
                Toast.MakeText(this, image, ToastLength.Long).Show();
                _filePath = image;
            }
        }
        
    }
}