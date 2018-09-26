using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Microsoft.WindowsAzure.MobileServices;
using PlanYourJourney.Models;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace PlanYourJourney.Activities
{
    [Activity(Label = "Arrangement_screen", MainLauncher = false)]
    public class ArrangementScreen : AppCompatActivity
    {
        private Arrangement _argArrangement;
        private ImageView _imageView;
        private string _id;
        private TextView _title, _title_2, _content, _location, _date, _author;
        private bool _flag;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CurrentPlatform.Init();

            // Set our view from the "main" layout resource
            base.SetContentView(PlanYourJourney.Resource.Layout.Arrangement_layout);

            var toolbar = FindViewById<Toolbar>(PlanYourJourney.Resource.Id.toolbar);

            //Toolbar will now take on default actionbar characteristics
            SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.Title = "Loading...";
            _flag = false;

            _id = Intent.GetStringExtra("Id");
            try
            {
                if (_id != "")
                {
                    _argArrangement = await ((MainApplication) this.Application).arrangementTable.LookupAsync(_id);
                    _flag = true;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            catch (Exception e)
            {
                Toast.MakeText(this, "Error: not exist", ToastLength.Short).Show();
                _argArrangement = new Arrangement
                {
                    Id = "",
                    Title = "Not exist",
                    Date = DateTime.MinValue,
                    Location = "",
                    ImageResourcePath = null,
                    Contents = "",
                    Author = ""
                };
                toolbar.Clickable = false;

                _flag = false;
            }


            _title = FindViewById<TextView>(PlanYourJourney.Resource.Id.arrangement_name);
            _title_2 = FindViewById<TextView>(PlanYourJourney.Resource.Id.arrangement_name_2);
            _content = FindViewById<TextView>(PlanYourJourney.Resource.Id.arrangement_notes);
            _location = FindViewById<TextView>(PlanYourJourney.Resource.Id.arrangement_place);
            _author = FindViewById<TextView>(PlanYourJourney.Resource.Id.arrangement_author);
            _date = FindViewById<TextView>(PlanYourJourney.Resource.Id.date);
            _imageView = FindViewById<ImageView>(PlanYourJourney.Resource.Id.arrangement_picture);

            _title.Text = _argArrangement.Title;
            _title_2.Text = _argArrangement.Title;
            _location.Text = _argArrangement.Location;
            _content.Text = _argArrangement.Contents;
            _author.Text = _argArrangement.Author;
            _date.Text = _argArrangement.Date.ToString("dd-MM-yyy");

            
            SetImage(_argArrangement.ImageResourcePath);
            SupportActionBar.Title = _argArrangement.Title;
        }
        
        /// <summary>
        /// This is the menu for the Toolbar/Action Bar to use
        /// </summary>
        /// <param name="menu">Menu.</param>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            base.MenuInflater.Inflate(PlanYourJourney.Resource.Menu.mypage_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
                Finish();
            else if (item.ItemId == PlanYourJourney.Resource.Id.menu_edit && _flag)
            {
                var intent = new Intent(this, typeof(EditArrangement));
                intent.PutExtra("Title", _argArrangement.Title);
                intent.PutExtra("Date", _argArrangement.Date.Ticks);
                intent.PutExtra("Location", _argArrangement.Location);
                intent.PutExtra("Contents", _argArrangement.Contents);
                intent.PutExtra("Id", _argArrangement.Id);
                intent.PutExtra("Author", _argArrangement.Author);
                intent.PutExtra("Img", _argArrangement.ImageResourcePath);
                base.StartActivityForResult(intent, 1);
            }
            else if(item.ItemId == PlanYourJourney.Resource.Id.menu_delete && _flag)
            {
                Deletion();
            }
            else if (item.ItemId == Resource.Id.ic_action_cached)
            {
                Refresh();
            }
            return base.OnOptionsItemSelected(item);
        }

        /// <summary>
        /// Set an image.
        /// </summary>
        /// <param name="path">Path to image.</param>
        private void SetImage(string path)
        {
            if (path != null)
            {
                int resourceId = Resources.GetIdentifier(path, "drawable", this.PackageName);

                _imageView.SetImageResource(resourceId);


            }
        }

        /// <summary>
        /// Refresh the screen after editing.
        /// </summary>
        private async void Refresh()
        {
            _argArrangement = await ((MainApplication)this.Application).arrangementTable.LookupAsync(_id);
            _title.Text = _argArrangement.Title;
            _title_2.Text = _argArrangement.Title;
            _location.Text = _argArrangement.Location;
            _content.Text = _argArrangement.Contents;
            _author.Text = _argArrangement.Author;
            _date.Text = _argArrangement.Date.ToString("dd-MM-yyy");
            SetImage(_argArrangement.ImageResourcePath);
            SupportActionBar.Title = _argArrangement.Title;
        }

        private void Deletion()
        {
            var alert = new Android.App.AlertDialog.Builder(this);
            alert.SetTitle("Are you sure to delete?");
            alert.SetPositiveButton("Yes", (senderAlert, args) =>
            {
                Delete();
                Toast.MakeText(this, "Delete", ToastLength.Short).Show();
                Finish();
            });

            alert.SetNegativeButton("No", (senderAlert, args) => {
                Toast.MakeText(this, "Cancelled", ToastLength.Short).Show();
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }

         void Delete()
        {
             ((MainApplication)this.Application).arrangementTable.DeleteAsync(_argArrangement);

        }
        

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok && requestCode == 1 && data != null)
            {
                Refresh();
            }
        }
    }
}