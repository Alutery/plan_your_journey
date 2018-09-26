using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.KD.Dynamic.Calendar.Generator;
using Java.Util;
using Microsoft.WindowsAzure.MobileServices;

namespace PlanYourJourney.Activities.Fragments
{
    public class ListScreenFragment : Android.Support.V4.App.Fragment, DatePickerDialog.IOnDateSetListener
    {
        private ListView _listView;
        private EditText _mDateEditext;
        private Calendar _mCurrentDate;
        private ImageGenerator _mGeneratorImage;
        private EditText _authorEdittext;
        private Button _clearBtn;

        // Adapter to map the items list to the view
        private ListScreenAdapter _adapter;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(PlanYourJourney.Resource.Layout.ListScreen, null);

            _listView = view.FindViewById<ListView>(Resource.Id.List);
            _listView.ItemClick += OnListItemClick; // to be defined
            
            _mGeneratorImage = new ImageGenerator(this.Activity);
            _mDateEditext = view.FindViewById<EditText>(Resource.Id.txtDateEdited);
            _authorEdittext = view.FindViewById<EditText>(Resource.Id.authorFilter);

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
                var mYear = _mCurrentDate.Get(CalendarField.Year);
                var mMonth = _mCurrentDate.Get(CalendarField.Month);
                var mDay = _mCurrentDate.Get(CalendarField.DayOfMonth);

                var mDate = new DatePickerDialog(this.Activity, this, mYear, mMonth, mDay);
                mDate.Show();
            };


            _authorEdittext.Click += (object sender, EventArgs e) =>
            {
                LayoutInflater layoutInflater = LayoutInflater.From(this.Activity);
                View v = layoutInflater.Inflate(Resource.Layout.InputAuthor_Fragment, null);
                Android.Support.V7.App.AlertDialog.Builder alertbuilder =
                    new Android.Support.V7.App.AlertDialog.Builder(this.Activity);
                alertbuilder.SetView(v);
                var userdata = v.FindViewById<EditText>(Resource.Id.author_input);
                alertbuilder.SetCancelable(false)
                    .SetPositiveButton("Submit", delegate
                    {
                        string author = userdata.Text;
                        MainActivity.byAuthorFilter = true;
                        MainActivity.Author = author;
                        _authorEdittext.Text = author;
                        Toast.MakeText(this.Activity, "Author: " + author, ToastLength.Short).Show();
                    })
                    .SetNegativeButton("Cancel", delegate { alertbuilder.Dispose(); });
                Android.Support.V7.App.AlertDialog dialog = alertbuilder.Create();
                dialog.Show();
            };

            _clearBtn = view.FindViewById<Button>(Resource.Id.clear_filters_btn);

            _clearBtn.Click += delegate
            {
                MainActivity.byDateFilter = false;
                MainActivity.byAuthorFilter = false;
                _mDateEditext.Text = "DD-MM-YYYY";
                _authorEdittext.Text = "By author";
            };
            
            _adapter = new ListScreenAdapter(this.Activity);
            _listView.Adapter = _adapter;
            
            return view;
        }

        private void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var intent = new Intent(Activity, typeof(ViewArrangementActivity));

            var note = ((ListScreenAdapter)_listView.Adapter).GetArrangement(e.Position).Id;
            intent.PutExtra("Id", note);

            StartActivity(intent);
        }

        public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
        {
            month++;
            _mDateEditext.Text = $"{dayOfMonth:00}-{month:00}-{year:0000}";
            _mCurrentDate.Set(year, month, dayOfMonth);
            MainActivity.byDateFilter = true;
            MainActivity.dateFilter = new DateTime(year, month, dayOfMonth);
        }
        
        public override void OnResume()
        {
            base.OnResume();
            RefreshItemsFromTable();
        }

       
        //Refresh the list.
        private void RefreshItemsFromTable()
        {
            try
            {
                _adapter.Flag = false;
                var tmp = _mDateEditext.Text;
                _mDateEditext.Text = "Loading..";

                // Get the items and add them in the adapter
                var list = ((MainActivity)Activity).List;
                _adapter.Clear();
                
                if (MainActivity.byDateFilter)
                {
                    if (MainActivity.byAuthorFilter)
                    {
                        foreach (var current in list)
                            if (current.Date.Date == MainActivity.dateFilter.Date && current.Author == MainActivity.Author)
                                _adapter.Add(current);
                    }
                    else
                    {
                        foreach (var current in list)
                            if(current.Date.Date == MainActivity.dateFilter.Date)
                                _adapter.Add(current);
                    }
                }
                else if(MainActivity.byAuthorFilter)
                {
                    foreach (var current in list)
                        if (current.Author == MainActivity.Author)
                            _adapter.Add(current);
                }
                else
                {
                    foreach (var current in list)
                        _adapter.Add(current);
                }

                if (_adapter.Count == 0 && list.Count != 0)
                {
                    Toast.MakeText(this.Activity, "Not Found", ToastLength.Short).Show();
                }
                _adapter.Flag = true;
                _mDateEditext.Text = tmp;
            }
            catch (Exception e)
            {
                HelpMe.CreateAndShowDialog(e.Message, "Error", Activity);
            }
        }
        
    }
}