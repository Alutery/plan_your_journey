using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Java.Lang;
using PlanYourJourney;
using PlanYourJourney.Activities.Fragments;
using Object = Java.Lang.Object;

namespace PlanYourJourney
{
    public class CustomPagerAdapter : FragmentStatePagerAdapter
    {
        private const int PageCount = 3;
        private readonly string[] _tabTitles = { "ALL", "MY PAGE", "FAVORITES" };
        private readonly Context _context;

        public CustomPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public CustomPagerAdapter(Context context, FragmentManager fm) : base(fm)
        {
            this._context = context;
        }

        public override int Count => PageCount;

        public override Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0:
                    return new ListScreenFragment();
                case 1:
                    return new MyPageFragment();
                default:
                    return new MyFavoritesFragment();
            }
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            // Generate title based on item position
            return CharSequence.ArrayFromStringArray(_tabTitles)[position];
        }

        public View GetTabView(int position)
        {
            // Given you have a custom layout in `res/layout/custom_tab.xml` with a TextView
            var tv = (TextView)LayoutInflater.From(_context).Inflate(Resource.Layout.custom_tab, null);
            tv.Text = _tabTitles[position];
            /*View tv;
            switch (position)
            {
                case 0:
                    tv = LayoutInflater.From(context).Inflate(Resource.Layout.ListScreen, null);
                    
                    break;
                case 1:
                    tv = LayoutInflater.From(context).Inflate(Resource.Layout.MyPage, null);
                    break;
                default:
                    tv = LayoutInflater.From(context).Inflate(Resource.Layout.MyFavorites, null);
                    break;
            }*/
            return tv;
        }

        public override int GetItemPosition(Object @object)
        {
            return PositionNone;
        }
    }
}