using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
        private dynamic _last16;
        private dynamic _last8;
        private dynamic _semiFinals;

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

            var resultTasks = new List<Task>();

            if (!IsStageOneComplete())
                resultTasks.Add(Task.Run( () => GetStageOneResults()));
            if (!HasRound16())
                resultTasks.Add(Task.Run(() => BuildLast16()));
            if(!HasQuarterFinals())
                resultTasks.Add(Task.Run(() => BuildLast8()));
            if(!HasBronseFinal())
                resultTasks.Add(Task.Run(() => BuildThirdPlacePlayoffs()));
            if(!HasSemiFinals())
                resultTasks.Add(Task.Run(() => BuildSemiFinals()));
            if(!HasFinal())
                resultTasks.Add(Task.Run(() => BuildFinals()));

            Task.WaitAll(resultTasks.ToArray());
        }

        private void GetStageOneResults()
        {
            var response = _httpClient.GetAsync("worldcups/2018/results").Result;

            if (response.IsSuccessStatusCode)
            {
                var liveResults =
                    JsonConvert.DeserializeObject<IEnumerable<Group>>(response.Content.ReadAsStringAsync().Result);

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

                currentResults.ForEach(g =>
                {
                    if (g.Matches.Any()) g.Matches = groupMatches[g.Id].ToList();
                });

                Groups = currentResults;
                _stageOne = null;
            }
        }

        private void BuildFinals()
        {
            Final = GetCupStage<Match>("worldcups/2018/results/final");
        }

        private void BuildThirdPlacePlayoffs()
        {
            ThirdPlacePlayoff = GetCupStage<Match>("worldcups/2018/results/thirdplace");
        }

        private void BuildSemiFinals()
        {
            SemiFinals = GetCupStage<IEnumerable<Match>>("worldcups/2018/results/semifinals");
            if(SemiFinals != null)
                _semiFinals = null;
        }

        private void BuildLast8()
        {
            Last8 = GetCupStage<IEnumerable<Match>>("worldcups/2018/results/last8");
            if(Last8 != null)
                _last8 = null;
        }

        private void BuildLast16()
        {
            Last16 = GetCupStage<IEnumerable<Match>>("worldcups/2018/results/last16");
            if(Last16 != null)
                _last16 = null;
        }

        private T GetCupStage<T>(string resource)
        {
            var response = _httpClient.GetAsync(resource).Result;

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
            }

            return default(T);
        }


        private void BuildStageOne()
        {
            var tomlBuilder = new StringBuilder();
            tomlBuilder.AppendLine("results = [");
            var winnersBuilder = new StringBuilder("winners = [");

            if(Groups == null)
                return;

            // Build group results
            Groups.ToList().ForEach(g =>
            {
                tomlBuilder.Append("[");
                for (var i = 0; i < g.Teams.Count() * 2 - 2; i++)
                {
                    if (g.Matches[i] != null)
                        tomlBuilder.Append($"\"{g.Matches[i].ResultSign}\",");
                    else
                        tomlBuilder.Append($"\"-\",");
                    
                }

                tomlBuilder.Append("],");
            });
            tomlBuilder.AppendLine("]");

            // Build group winners
            Groups.ToList().ForEach(g =>
            {
                winnersBuilder.Append("[");

                if ((g.Teams.Count() * 2 - 2) == g.Matches.Count(m => m != null))
                {
                    winnersBuilder.Append($"\"{FixName(g.Teams.ElementAt(0).Country)}\",");
                    winnersBuilder.Append($"\"{FixName(g.Teams.ElementAt(1).Country)}\",");
                }
                else
                {
                    winnersBuilder.Append($"\"-\",");
                    winnersBuilder.Append($"\"-\",");
                }
                winnersBuilder.Append("],");
            });
            winnersBuilder.AppendLine("]");

            tomlBuilder.AppendLine(winnersBuilder.ToString());

            _stageOne = tomlBuilder.ToString().ParseAsToml();
        }

        private string FixName(string team)
        {
            if (team == "Saudi Arabia")
                team = "Saudia Arabia";
            if (team == "Korea Republic")
                team = "South Korea";

            return team;
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
            return false;
        }

        public bool HasRound16()
        {
            return Last16 != null && Last16.Count() > 0;
        }

        public dynamic GetRound16Winners()
        {
            if (_last16 == null)
            {
                string[] ls = new string[8];
                for (int i = 0; i < 8; i++)
                {
                    ls[i] = (i < Last16.Count()) ? Last16.ElementAt(i).Winner : "-";
                }
                _last16 = ls;
            }

            return _last16;
        }

        public bool HasQuarterFinals()
        {
            return Last8 != null && Last8.Count() > 0;
        }

        public dynamic GetQuarterFinalWinners()
        {
            if (_last8 == null)
            {
                string[] ls = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    ls[i] = (i < Last8.Count()) ? Last8.ElementAt(i).Winner : "-";
                }
                _last8 = ls;
            }

            return _last8;
        }

        public bool HasSemiFinals()
        {
            return SemiFinals != null && SemiFinals.Count() > 0;
        }

        public dynamic GetSemiFinalWinners()
        {
            if (_semiFinals == null)
            {
                string[] ls = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    ls[i] = (i < SemiFinals.Count()) ? SemiFinals.ElementAt(i).Winner : "-";
                }
                _semiFinals = ls;
            }

            return _semiFinals;
        }

        public List<string> GetBronseFinalists()
        {
            return new List<string>
            {
                ThirdPlacePlayoff.HomeTeam.Country,
                ThirdPlacePlayoff.AwayTeam.Country
            };
        }

        public bool HasBronseFinal()
        {
            return ThirdPlacePlayoff != null;
        }

        public string GetBronseFinalWinner()
        {
            return ThirdPlacePlayoff.Winner;
        }

        public bool HasFinal()
        {
            return Final != null;
        }

        public string GetFinalWinner()
        {
            return Final.Winner;
        }

        public void Copy(ILiveResults results)
        {
            Timestamp = results.Timestamp;
            Groups = results.Groups;
            _stageOne = null;

            Last16 = results.Last16;
            _last16 = null;

            Last8 = results.Last8;
            _last8 = null;

            SemiFinals = results.SemiFinals;
            _semiFinals = null;

            ThirdPlacePlayoff = results.ThirdPlacePlayoff;

            Final = results.Final;
        }

        public Match Final { get; private set; }
        public Match ThirdPlacePlayoff { get; private set; }
        public IEnumerable<Match> SemiFinals { get; private set; }
        public IEnumerable<Match> Last8 { get; private set; }
        public IEnumerable<Match> Last16 { get; private set; }
        public IEnumerable<Group> Groups { get; private set; }
        public DateTime Timestamp { get; private set; }
    }
}
