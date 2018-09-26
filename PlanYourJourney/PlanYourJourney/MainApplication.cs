using System;
using Android.App;
using Android.Runtime;
using Microsoft.WindowsAzure.MobileServices;
using PlanYourJourney.DataFavoritesAccess;
using PlanYourJourney.Models;

namespace PlanYourJourney
{
    [Application(Debuggable = true)]
    class MainApplication : Application
    {
        public OrmFavoriteRepository FavoriteRepository { get; set; }
        public IMobileServiceTable<Arrangement> arrangementTable;
        public IMobileServiceTable<Account> accountTable;

        public MainApplication(IntPtr handle, JniHandleOwnership transfer)
            : base(handle, transfer)
        {
        }
        public override void OnCreate()
        {
            base.OnCreate();
            CurrentPlatform.Init();
            FavoriteRepository = new OrmFavoriteRepository(this);


            arrangementTable = MobileService.GetTable<Arrangement>();
            accountTable = MobileService.GetTable<Account>();
        }

        public static MobileServiceClient MobileService =
            new MobileServiceClient(
                "https://planyourjourneywithaccounts.azurewebsites.net"
            );
    }
}