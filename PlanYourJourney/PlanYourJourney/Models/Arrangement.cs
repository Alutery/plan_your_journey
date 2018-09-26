using System;
using Newtonsoft.Json;
using SQLite;

namespace PlanYourJourney.Models
{
    public class Arrangement
    {
        //[PrimaryKey, AutoIncrement]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }
        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }
        [JsonProperty(PropertyName = "imageResourcePath")]
        public string ImageResourcePath { get; set; }
        [JsonProperty(PropertyName = "contents")]
        public string Contents { get; set; }
        ///[JsonProperty(PropertyName = "favorite")]
        //public int Favorite { get; set; }
        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }
    }
    public class ArrangementWrapper : Java.Lang.Object
    {
        public ArrangementWrapper(Arrangement item)
        {
            Arrangement = item;
        }

        public Arrangement Arrangement { get; private set; }
    }
}