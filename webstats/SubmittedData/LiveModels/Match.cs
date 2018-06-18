using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SubmittedData.LiveModels
{    
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class Match
    {
        public string Venue { get; set; }
        public string Location { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
        public string Winner { get; set; }
        public string WinnerCode { get; set; }
        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }

        [JsonIgnore]
        public string ResultSign => HomeTeam?.Goals > AwayTeam?.Goals ? "h" :
                                    HomeTeam?.Goals < AwayTeam?.Goals ? "b" : "u";
    }
}
