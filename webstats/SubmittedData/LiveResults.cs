using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using SubmittedData.LiveModels;
using Toml;

namespace SubmittedData
{
    public class LiveResults : ILiveResults
    {
        private readonly ITournament _tournament;
        private readonly HttpClient _httpClient;
        private dynamic _stageOne;

        public LiveResults(ITournament tournament)
        {
            _tournament = tournament;
            _httpClient = new HttpClient();
        }
        public void Load(string baseUrl)
        {
            Timestamp = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(baseUrl))
                _httpClient.BaseAddress = new Uri(baseUrl);

            var response = _httpClient.GetAsync("worldcups/2018/results").Result;

            if (response.IsSuccessStatusCode)
            {
                var liveResults = JsonConvert.DeserializeObject<IEnumerable<Group>>(response.Content.ReadAsStringAsync().Result);

                //order matches based on this layout

                //matches.append([group[0], group[1]])
                //matches.append([group[2], group[3]])
                //matches.append([group[0], group[2]])
                //matches.append([group[3], group[1]])
                //matches.append([group[3], group[0]])
                //matches.append([group[1], group[2]])


                var groupMatches = new Dictionary<int, IEnumerable<Match>>();
                var currentResults = liveResults.ToList();

                char groupLetter = 'A';
                foreach (object[] tournamentGroup in _tournament.GetGroups())
                {
                    var liveGroup = currentResults.FirstOrDefault(g => g.Letter == groupLetter);

                    if (liveGroup != null && liveGroup.Matches.Any())
                    {
                        var teamOne = tournamentGroup[0].ToString();
                        var teamTwo = tournamentGroup[1].ToString();
                        var teamThree = tournamentGroup[2].ToString();
                        var teamFour = tournamentGroup[3].ToString();

                        var matches = new Queue<Match>();
                        matches.Enqueue(liveGroup.Matches.FirstOrDefault(m => IsCorrectMatch(m, teamOne, teamTwo)));
                        matches.Enqueue(liveGroup.Matches.FirstOrDefault(m => IsCorrectMatch(m, teamThree, teamFour)));
                        matches.Enqueue(liveGroup.Matches.FirstOrDefault(m => IsCorrectMatch(m, teamOne, teamThree)));
                        matches.Enqueue(liveGroup.Matches.FirstOrDefault(m => IsCorrectMatch(m, teamFour, teamTwo)));
                        matches.Enqueue(liveGroup.Matches.FirstOrDefault(m => IsCorrectMatch(m, teamFour, teamOne)));
                        matches.Enqueue(liveGroup.Matches.FirstOrDefault(m => IsCorrectMatch(m, teamTwo, teamThree)));

                        groupMatches.Add(liveGroup.Id, matches);
                    }

                    groupLetter++;
                }

                currentResults.ForEach(g => { if (g.Matches.Any()) g.Matches = groupMatches[g.Id].ToList(); });

                Groups = currentResults;
                _stageOne = null;
            }

        }

        private void BuildStageOne()
        {
            var tomlBuilder = new StringBuilder();
            tomlBuilder.AppendLine("results = [");
            var winnersBuilder = new StringBuilder("winners = [");

            if(Groups == null)
                return;

            Groups.ToList().ForEach(g =>
            {
                tomlBuilder.Append("[");
                winnersBuilder.Append("[");
                for (var i = 0; i < g.Teams.Count() * 2 - 2; i++)
                {
                    if (g.Matches[i] != null)
                        tomlBuilder.Append($"\"{g.Matches[i].ResultSign}\",");
                    else
                        tomlBuilder.Append($"\"-\",");
                    
                    winnersBuilder.Append("\"-\",");
                }

                tomlBuilder.Append("],");
                winnersBuilder.Append("],");
            });
            tomlBuilder.AppendLine("]");
            winnersBuilder.AppendLine("]");

            tomlBuilder.AppendLine(winnersBuilder.ToString());

            _stageOne = tomlBuilder.ToString().ParseAsToml();
        }

        private bool IsCorrectMatch(Match match, string homeTeam, string awayTeam)
        {
            if (homeTeam == "Saudia Arabia")
                homeTeam = "Saudi Arabia";
            if (awayTeam == "Saudia Arabia")
                awayTeam = "Saudi Arabia";

            if (homeTeam == "South Korea")
                homeTeam = "Korea Republic";
            if (awayTeam == "South Korea")
                awayTeam = "Korea Republic";

            return (match.HomeTeam.Country == homeTeam && match.AwayTeam.Country == awayTeam) ||
                   (match.HomeTeam.Country == awayTeam && match.AwayTeam.Country == homeTeam);
        }

        public string GetTimeStamp()
        {
            return Timestamp.ToString("F", CultureInfo.InvariantCulture);
        }

        public bool HasStageOne()
        {
            return Groups != null;
        }

        public dynamic GetStageOne()
        {
            if(_stageOne == null)
                BuildStageOne();

            return _stageOne;
        }

        public bool IsStageOneComplete()
        {
            if (Groups == null)
                return false;
            return Groups.All(g => (g.Teams.Count() * 2 - 2) == g.Matches.Count(m => m != null));
        }

        public dynamic GetThirdPlaces()
        {
            return null; //TODO
        }

        public bool HasThirdPlaces()
        {
            return false; //TODO
        }

        public dynamic GetInfo()
        {
            return "actual";
        }

        public bool HasStageTwo()
        {
            return false; //TODO
        }

        public bool HasRound16()
        {
            return false; //TODO
        }

        public dynamic GetRound16Winners()
        {
            return null; //TODO
        }

        public bool HasQuarterFinals()
        {
            return false; //TODO
        }

        public dynamic GetQuarterFinalWinners()
        {
            return null; //TODO
        }

        public bool HasSemiFinals()
        {
            return false; //TODO
        }

        public dynamic GetSemiFinalWinners()
        {
            return null; //TODO
        }

        public List<string> GetBronseFinalists()
        {
            return null; //TODO
        }

        public bool HasBronseFinal()
        {
            return false; //TODO
        }

        public string GetBronseFinalWinner()
        {
            return null; //TODO
        }

        public bool HasFinal()
        {
            return false; //TODO
        }

        public string GetFinalWinner()
        {
            return null; //TODO
        }

        public void Copy(ILiveResults results)
        {
            Timestamp = results.Timestamp;
            Groups = results.Groups;
            _stageOne = null;
        }

        public IEnumerable<Group> Groups { get; private set; }
        public DateTime Timestamp { get; private set; }
    }
}
