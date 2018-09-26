using Android.Content;
using Android.Database.Sqlite;

namespace PlanYourJourney.DataFavoritesAccess
{
    class FavoriteDatabaseHelper : SQLiteOpenHelper
    {
        private const string DATABASE_NAME = "Favorites";
        private const int DATABASE_VERSION = 1;

        public FavoriteDatabaseHelper(Context context)
            : base(context, DATABASE_NAME, null, DATABASE_VERSION)
        {
        }

        public override void OnCreate(SQLiteDatabase db)
        {
            db.ExecSQL(@"
                    CREATE TABLE Favorite (
                        Id TEXT NOT NULL
                    )");
        }
        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            db.ExecSQL("DROP TABLE IF EXISTS Favorite");

            OnCreate(db);
        }
    }
}