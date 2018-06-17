using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SubmittedData.LiveModels
{
    //    {
    //    "venue": "Moscow",
    //    "location": "Luzhniki Stadium",
    //    "datetime": "2018-06-14T17:00:00.000-05:00",
    //    "status": "in progress",
    //    "home_team": {
    //    "country": "Russia",
    //    "code": "RUS",
    //    "goals": 5
    //},
    //"away_team": {
    //"country": "Saudi Arabia",
    //"code": "KSA",
    //"goals": 0
    //},
    //"winner": null,
    //"winner_code": null,
    //"home_team_events": [],
    //"away_team_events": []
    //}
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
