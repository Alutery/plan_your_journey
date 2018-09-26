using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Provider;
using PlanYourJourney.Models;
using SQLite;

namespace PlanYourJourney.DataFavoritesAccess
{
    class OrmFavoriteRepository
    {
        private readonly FavoriteDatabaseHelper _helper;

        public OrmFavoriteRepository(Context context)
        {
            _helper = new FavoriteDatabaseHelper(context);
        }

        public List<Favorite> GetAllFavorites()
        {
            using (var database = new SQLiteConnection(_helper.WritableDatabase.Path))
            {
                return database
                    .Table<Favorite>()
                    .ToList();
            }
        }
        
        public long AddFavorite(string id)
        {
            using (var database = new SQLiteConnection(_helper.WritableDatabase.Path))
            {
                return database.Insert(new Favorite
                {
                    Id = id
                });
            }
        }


        /*public long EditArrangement(string title, DateTime date, string loc, string img, string contents, Arrangement baseArg)
        {
            using (var database = new SQLiteConnection(_helper.WritableDatabase.Path))
            {
                return database.Update(new Arrangement
                {
                    Id = baseArg.Id,
                    Title = title,
                    Date = date,
                    Location = loc,
                    ImageResourcePath = img,
                    Contents = contents,
                    Author = baseArg.Author
                });
            }
        }*/

        public long DeleteFavorite(string id)
        {
            using (var database = new SQLiteConnection(_helper.WritableDatabase.Path))
            {
                return database.Delete(new Favorite
                {
                    Id = id
                });
            }
        }

        /*public long EditFavorite(int favorite, Arrangement baseArg)
        {
            using (var database = new SQLiteConnection(_helper.WritableDatabase.Path))
            {

                return database.Update(new Arrangement
                {
                    Id = baseArg.Id,
                    Title = baseArg.Title,
                    Date = baseArg.Date,
                    Location = baseArg.Location,
                    ImageResourcePath = baseArg.ImageResourcePath,
                    Contents = baseArg.Contents,
                    Author = baseArg.Author
                });
            }
        }*/

        /*public long LastId()
        {
            using (var database = new SQLiteConnection(_helper.WritableDatabase.Path))
            {
                //var res = database.Query<long>("SELECT MAX(Id) from Arrangement");
                //var com = database.CreateCommand(@"SELECT MAX(Id) from Arrangement");
                //var com_com = database.CreateCommand(@"SELECT last_insert_rowid();");
                var com = database.CreateCommand("SELECT * FROM Favorite ORDER BY Id DESC LIMIT 1;");
                var r = com.ExecuteQuery<Arrangement>();
                //return r.Count == 0 ? 0 : r[0].Id ?? 0;
                //return database.Query<long>("select max(Id) from Arrangement;")[0];
                //long? res = database.ExecuteScalar<long>("SELECT MAX(Id) from Arrangement");
                //return res ?? 0;
                //return res;
                return 1;//!!!!!!!!!!!!!!!!!!!!!!!!
            }
        }

        public Arrangement GetById(long? id)
        {
            using (var database = new SQLiteConnection(_helper.WritableDatabase.Path))
            {
                var com = database.CreateCommand(@"SELECT * from Arrangement where Id=" + id);
                var res = com.ExecuteQuery<Arrangement>();
                return res[0];
            }
        }*/
    }
}