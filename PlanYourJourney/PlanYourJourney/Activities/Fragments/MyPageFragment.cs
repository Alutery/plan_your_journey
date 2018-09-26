using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Microsoft.WindowsAzure.MobileServices;
using PlanYourJourney.Models;

namespace PlanYourJourney.Activities.Fragments
{
    public class MyPageFragment : Android.Support.V4.App.Fragment
    {
        private ListView _listView;
        private TextView _textView;
        private TextView _myUsename;

        // Adapter to map the items list to the view
        private ListScreenAdapter _adapter;


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            CurrentPlatform.Init();

            View view = inflater.Inflate(PlanYourJourney.Resource.Layout.MyPage, null);

            _textView = view.FindViewById<TextView>(PlanYourJourney.Resource.Id.my_plans);
            _listView = view.FindViewById<ListView>(PlanYourJourney.Resource.Id.List_of_Mine);
            _myUsename = view.FindViewById<TextView>(Resource.Id.account_name);
            _myUsename.Text = MainActivity.MyUsername;
            //CurrentPlatform.Init();

            _adapter = new ListScreenAdapter(this.Activity);
            _listView.Adapter = _adapter;
            _listView.ItemClick += OnListItemClick; // to be defined
            
            return view;
        }

        private void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var intent = new Intent(Activity, typeof(ArrangementScreen));
            string note;
            try
            {
                note = ((ListScreenAdapter) _listView.Adapter).GetArrangement(e.Position).Id;
            }
            catch (Exception)
            {
                note = "";
            }
            intent.PutExtra("Id", note);

            StartActivity(intent);
        }


        public override void OnResume()
        {
            base.OnResume();
            RefreshItemsFromTable();
        }
        
        //Refresh the list with the items in the local store.
        private void RefreshItemsFromTable()
        {
            try
            {
                //_adapter.Flag = false;
                //_listView.Clickable = false;
                _listView.Enabled = false;
                _textView.Text = "Loading..";
                // Get the items that weren't marked as completed and add them in the adapter
                var list = ((MainActivity)Activity).List;
                _adapter.Clear();

                foreach (Arrangement current in list)
                {
                    if (current.Author == MainActivity.MyUsername)
                    {
                        _adapter.Add(current);
                    }
                }
                   
                _listView.Enabled = true;
                _adapter.Flag = true;
                _textView.Text = GetString(Resource.String.my_plans);
            }
            catch (Exception e)
            {
                CreateAndShowDialog(e.Message, "Error");
            }
        }
        private void CreateAndShowDialog(string message, string title)
        {
            var builder = new AlertDialog.Builder(Activity);

            builder.SetMessage(message);
            builder.SetTitle(title);
            builder.Create().Show();
        }
    }
}