using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
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
    [Activity(Label = "AddArrangement")]
    public class AddArrangement : AppCompatActivity, DatePickerDialog.IOnDateSetListener
    {
        Android.Support.Design.Widget.TextInputEditText _passwordView;
        private LinearLayout _mainContent;
        private TextInputLayout _titleLayout;
        private TextInputLayout _locationLayout;
        InputMethodManager _inputManager;

        EditText _mDateEditext;
        Calendar _mCurrentDate;
        ImageGenerator _mGeneratorImage;

        private EditText _title, _content, _location, _date;
        private string _filePath = "airport_shuttle";

        private FragmentManager fm;
        private SingleChoiceDialogFragment choice;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CurrentPlatform.Init();

            // Create your application here
            base.SetContentView(PlanYourJourney.Resource.Layout.Edit_Arrangement_layout);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            //Toolbar will now take on default actionbar characteristics
            SetSupportActionBar(toolbar);

            SupportActionBar.Title = "Add";

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            _passwordView = FindViewById<TextInputEditText>(Resource.Id.place_edittext);

            _titleLayout = (TextInputLayout)base.FindViewById(Resource.Id.name_layout);
            _locationLayout = (TextInputLayout)base.FindViewById(Resource.Id.place_layout);
            _mainContent = (LinearLayout)base.FindViewById(Resource.Id.main_content);

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

            _content.SetSingleLine(false);

            DateTime today = DateTime.Today;
            _date.Text = $"{today.Day:00}-{today.Month:00}-{today.Year:0000}";

            fm = this.FragmentManager;
            choice = new SingleChoiceDialogFragment();
        }

        /// <summary>
        /// Создание actionbar.
        /// </summary>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            base.MenuInflater.Inflate(PlanYourJourney.Resource.Menu.actionbar_add, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        
        /// <summary>
        /// Проверка на корректность.
        /// </summary>
        /// <returns>Результат проверки.</returns>
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
                var alert = new Android.App.AlertDialog.Builder(this);
                alert.SetTitle("Do you what to save chaged?");
                alert.SetPositiveButton("Yes", (senderAlert, args) => {
                    if (OnClick())
                    {
                        Toast.MakeText(this, "Saved", ToastLength.Short).Show();
                        Save_Click();
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
            else if (item.ItemId == PlanYourJourney.Resource.Id.menu_save)
            {
                var alert = new Android.App.AlertDialog.Builder(this);
                alert.SetTitle("Confirm saving");
                alert.SetPositiveButton("Save", (senderAlert, args) =>
                {
                    if (OnClick())
                    {
                        Toast.MakeText(this, "Saved", ToastLength.Short).Show();
                        Save_Click();
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
                choice.Show(fm, "choose_image");
            }
            return base.OnOptionsItemSelected(item);
        }

        /// <summary>
        /// Отображение выбранной даты.
        /// </summary>
        public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
        {
            month++;
            _mDateEditext.Text = $"{dayOfMonth:00}-{month:00}-{year:0000}";
            _mCurrentDate.Set(year, month, dayOfMonth);
        }

        [Java.Interop.Export()]
        public async void Save_Click()
        {

            // Create a new item
            var item = new Arrangement
            { 
                Title = _title.Text, 
                Date = DateTime.ParseExact(_date.Text, "dd-MM-yyyy",
                    System.Globalization.CultureInfo.InvariantCulture), 
                Location = _location.Text,
                ImageResourcePath = _filePath,
                Contents = _content.Text,
                Author = MainActivity.MyUsername
            };

            try
            {
                // Insert the new item into the local store.
                await ((MainApplication)Application).arrangementTable.InsertAsync(item);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " - Error");
            }
            
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            
            if (requestCode == 0 && data != null) {
                var result = data.GetBundleExtra("Result");
                var image = result.GetString("ImageName");
                var toShow = result.GetString("ToShow");
                Toast.MakeText(this, toShow, ToastLength.Short).Show();
                _filePath = image;
            }
        }
        
    }
}
 