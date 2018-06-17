using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SubmittedData.LiveModels
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class Group
    {
        public Group()
        {
            Matches = new List<Match>();
        }
        public int Id { get; set; }
        public char Letter { get; set; }
        
        public IEnumerable<Team> Teams { get; set; }

        public IList<Match> Matches { get; set; }

    }
}
