using System;
using Android.App;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Microsoft.WindowsAzure.MobileServices;
using PlanYourJourney.Models;
using Toolbar = Android.Support.V7.Widget.Toolbar;


namespace PlanYourJourney.Activities
{
    [Activity(Label = "ViewArrangementActivity")]
    public class ViewArrangementActivity : AppCompatActivity
    {
        private IMenu _menu;
        private Toolbar _toolbar;
        private bool _favorite;
        private Arrangement _argArrangement;

        private ImageView _imageView;

        string _id;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CurrentPlatform.Init();

            // Create your application here
            base.SetContentView(PlanYourJourney.Resource.Layout.Arrangement_layout);

            _imageView = (ImageView) base.FindViewById(PlanYourJourney.Resource.Id.arrangement_picture);
            
            _id = Intent.GetStringExtra("Id");
            

            try
            {
                if (_id != "")
                {
                    _argArrangement = await ((MainApplication) this.Application).arrangementTable.LookupAsync(_id);
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            catch (Exception e)
            {
                Toast.MakeText(this, e.ToString(), ToastLength.Short).Show();
                _argArrangement = new Arrangement
                {
                    Id = "",
                    Title = "",
                    Date = DateTime.MinValue,
                    Location = "",
                    ImageResourcePath = null,
                    Contents = "",
                    Author = ""
                };
                _toolbar.Enabled = false;
            }

            _toolbar = FindViewById<Toolbar>(PlanYourJourney.Resource.Id.toolbar);
            //Toolbar will now take on default actionbar characteristics
            SetSupportActionBar(_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.Title = _argArrangement.Title;

            FindViewById<TextView>(Resource.Id.arrangement_name_2).Text = _argArrangement.Title;
            FindViewById<TextView>(Resource.Id.arrangement_name).Text = _argArrangement.Title;
            FindViewById<TextView>(Resource.Id.arrangement_place).Text = _argArrangement.Location;
            FindViewById<TextView>(PlanYourJourney.Resource.Id.arrangement_notes).Text = _argArrangement.Contents;
            FindViewById<TextView>(PlanYourJourney.Resource.Id.arrangement_author).Text = _argArrangement.Author;
            var date = _argArrangement.Date;
            FindViewById<TextView>(PlanYourJourney.Resource.Id.date).Text = _argArrangement.Date.ToString("dd-MM-yyy");
            var favs = ((MainApplication)Application).FavoriteRepository.GetAllFavorites();
            _favorite = false;
            foreach (var i in favs)
            {
                if (_argArrangement.Id == i.ToString())
                {
                    _favorite = true;
                    break;
                }
            }
            //_favorite = MainActivity.Favorites.Contains(_argArrangement.Id);
            if (_favorite)
            {
                InvalidateOptionsMenu();
            }

            SetImage(_argArrangement.ImageResourcePath);

            
            

        }

        /// <Docs>The options menu in which you place your items.</Docs>
        /// <returns>To be added.</returns>
        /// <summary>
        /// This is the menu for the Toolbar/Action Bar to use
        /// </summary>
        /// <param name="menu">Menu.</param>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            base.MenuInflater.Inflate(PlanYourJourney.Resource.Menu.view_arrangement_with_star, menu);
            _menu = menu;
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            //InvalidateOptionsMenu();
            menu.GetItem(0).SetIcon(_favorite
                ? ContextCompat.GetDrawable(this, PlanYourJourney.Resource.Drawable.ic_action_star)
                : ContextCompat.GetDrawable(this, PlanYourJourney.Resource.Drawable.ic_action_star_border));
            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    base.Finish();
                    break;
                case PlanYourJourney.Resource.Id.star:
                    if(_favorite == false)
                    {
                        _favorite = true;
                        base.InvalidateOptionsMenu();
                        //MainActivity.Favorites.Add(_argArrangement.Id);
                        ((MainApplication)Application).FavoriteRepository.AddFavorite(_argArrangement.Id);
                    }
                    else
                    {
                        _favorite = false;
                        base.InvalidateOptionsMenu();
                        //MainActivity.Favorites.Remove(_argArrangement.Id);
                        ((MainApplication)Application).FavoriteRepository.DeleteFavorite(_argArrangement.Id);
                    }

                    break;
            }

            return base.OnOptionsItemSelected(item);
        }
        

        private void SetImage(string path)
        {
            if (path != null)
            {
                int resourceId = Resources.GetIdentifier(path, "drawable", this.PackageName);

                _imageView.SetImageResource(resourceId);


            }
        }
    }
}