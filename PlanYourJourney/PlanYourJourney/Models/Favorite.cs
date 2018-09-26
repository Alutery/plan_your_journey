using Newtonsoft.Json;
using SQLite;

namespace PlanYourJourney.Models
{
    internal class Favorite
    {
        [PrimaryKey, AutoIncrement]
        public long IdId { get; set; }
        public string Id { get; set; }


        public override string ToString()
        {
            return Id;
        }
    }
}