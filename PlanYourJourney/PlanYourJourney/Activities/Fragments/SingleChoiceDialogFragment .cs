using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace PlanYourJourney.Activities.Fragments
{
    public class SingleChoiceDialogFragment : DialogFragment
    {
        private ListView _lv;
        private readonly string[] _images =
        {
            "Restaurant", "Grocery Store", "Airport Shuttle",
            "Default", "Museum", "Dinner", "Love", "Flight", "Sun", "Movie", "Coffee", "Boat",
            "Car", "City", "Bar"
        };
        private readonly string[] _names = { "restaurant_menu", "local_grocery_store",
            "airport_shuttle", "photo", "account_balance", "local_dining", "favorite",
            "flight", "wb_sunny", "camera_roll", "free_breakfast", "directions_boat",
            "directions_car", "location_city", "local_bar"
        };

        private ArrayAdapter _adapter;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var v = inflater.Inflate(PlanYourJourney.Resource.Layout.Choice_Fragment, container, false);

            //SET TITLE FOR DIALOG
            this.Dialog.SetTitle("Choose image");

            _lv = v.FindViewById<ListView>(Resource.Id.lv);

            //ADAPTER
            _adapter = new ArrayAdapter(this.Activity, Android.Resource.Layout.SimpleListItem1, _images);
            _lv.Adapter = _adapter;

            //ITEM CLICKS
            _lv.ItemClick += Lv_ItemClick;

            return v;
        }

        private void Lv_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var result = new Intent();
            var bundle = new Bundle();
            bundle.PutString("ImageName", _names[e.Position]);
            bundle.PutString("ToShow", _images[e.Position]);
            result.PutExtra("Result", bundle);

            this.Activity.CreatePendingResult(0, result, PendingIntentFlags.OneShot).Send();
            Dismiss();
        }
    }
}