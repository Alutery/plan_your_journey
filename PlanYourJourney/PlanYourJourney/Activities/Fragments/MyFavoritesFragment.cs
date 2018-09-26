using System;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace PlanYourJourney.Activities.Fragments
{
    public class MyFavoritesFragment : Android.Support.V4.App.Fragment
    {
        private ListView _listView;
        private TextView _text_view;

        // Adapter to map the items list to the view
        private ListScreenAdapter _adapter;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.MyFavorites, null);
            _listView = view.FindViewById<ListView>(Resource.Id.List_of_Favorites);
            _text_view = view.FindViewById<TextView>(Resource.Id.text_fav);
            _adapter = new ListScreenAdapter(this.Activity);
            _listView.Adapter = _adapter;
            _listView.ItemClick += OnListItemClick; // to be defined

            return view;
        }


        private void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var intent = new Intent(Activity, typeof(ViewArrangementActivity));

            var note = ((ListScreenAdapter)_listView.Adapter).GetArrangement(e.Position).Id;
            intent.PutExtra("Id", note);
            
            StartActivity(intent);
        }

        public override void OnResume()
        {
            base.OnResume();
            RefreshItemsFromTable();
        }
        
        private void RefreshItemsFromTable()
        {
            try
            {
                _adapter.Flag = false;
                _text_view.Text = "Loading..";

                // Get the items that weren't marked as completed and add them in the adapter
                var list = ((MainActivity)Activity).List;
                var favs = ((MainApplication) Activity.Application).FavoriteRepository.GetAllFavorites();
                _adapter.Clear();
                HashSet<string> set = new HashSet<string>();
                foreach (var i in favs)
                {
                    set.Add(i.ToString());
                }
                foreach (var current in list)
                    if(set.Contains(current.Id))
                        _adapter.Add(current);

                _adapter.Flag = true;
                _text_view.Text = GetString(Resource.String.your_favorites);
            }
            catch (Exception e)
            {
                HelpMe.CreateAndShowDialog(e.Message, "Error", Activity);
            }
        }
        
    }
}