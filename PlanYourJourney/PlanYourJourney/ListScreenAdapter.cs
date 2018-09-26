 using System.Collections.Generic;
using Android.App;
using System.Linq;
 using Android.Content.Res;
 using Android.Views;
using Android.Widget;
using PlanYourJourney.Models;

namespace PlanYourJourney
{
    public class ListScreenAdapter : BaseAdapter<Arrangement>
    {
        private List<Arrangement> _arrangements = new List<Arrangement>();
        Activity context;
        public bool Flag { get; set; }

        public ListScreenAdapter(Activity context)
            : base()
        {
            this.context = context;
            Flag = true;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public Arrangement GetArrangement(int position)
        {
            return _arrangements.ElementAt(position);
        }
        public override Arrangement this[int position]
        {
            get { return _arrangements.ElementAt(position); }
        }
        public override int Count
        {
            get { return _arrangements.Count(); }
        }

        //Returns the view for a specific item on the list
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = this[position];
            var view = convertView;


            if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout.CustomView, null);

            view.FindViewById<TextView>(Resource.Id.Text1).Text = item.Title;
            view.FindViewById<TextView>(Resource.Id.Text2).Text = item.Contents;

            if (item.ImageResourcePath != null)
            {
                int resourceId = context.Resources.GetIdentifier(item.ImageResourcePath, "drawable", this.context.PackageName);

                view.FindViewById<ImageView>(Resource.Id.Image)
                    .SetImageResource(resourceId);


            } //currPaint.SetImageResource(Resource.Drawable.choice);

            return view;
        }

        public void Add(Arrangement item)
        {
            _arrangements.Add(item);
            NotifyDataSetChanged();
        }

        public void Clear()
        {
            _arrangements.Clear();
            NotifyDataSetChanged();
        }

        public void Remove(Arrangement item)
        {
            _arrangements.Remove(item);
            NotifyDataSetChanged();
        }

        public override bool IsEnabled(int position)
        {
            return Flag;
        }
    }
}