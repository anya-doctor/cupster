using SubmittedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modules
{
    public class BetterPageViewModel
    {
        IResults _bet;
        ITournament _tournament;
        IResults _results;
        ScoringSystem _userScore;
        ScoringSystem _totalScore;

        public BetterPageViewModel(ITournament t, IResults bet, IResults actual)
        {
            _tournament = t;
            _bet = bet;
            _results = actual;
            _userScore = new ScoringSystem(bet, actual);
            _totalScore = new ScoringSystem(actual, actual);
            CreateGroupMatches();
			if (WorldCupRules)
			{
				CreateRound16Matches();
				CreateQuarterFinalMatches();
				CreateSemiFinalMatches();
				CreateBronseFinalMatch();
			}
			else
			{
				CreateEuroRound16Matches();
				CreateEuroQuarterFinalMatches();
				CreateEuroSemiFinalMatches();
			}
            CreateFinalMatch();
        }

		public object Better
        {
            get { return _bet.GetInfo().user; }
        }

        public object PageTitle
        {
            get { return _bet.GetInfo().user; }
        }

        public bool WorldCupRules
        {
            get { return _tournament.IsFifaWorldCup(); }
        }
        
        List<GroupMatches> _groups = new List<GroupMatches>();
        public List<GroupMatches> Groups
        {
            get { return _groups; }
            private set { _groups = value; }
        }
		
        public int Score
        {
            get 
            {
                if (WorldCupRules)
                {
                    return _userScore.GetTotal();
                }
                else
                {
                    return _userScore.GetTotalWithoutBronse();
                }
            }
        }

        public int GroupScore
        {
            get { return _userScore.GetStageOneMatchScore() + _userScore.GetQualifierScore(); }
        }

        public int Round16Score
        {
            get { return _userScore.GetRound16Score(); }
        }

        public int QuarterFinalScore
        {
            get { return _userScore.GetQuarterFinalScore(); }
        }

        public int SemiFinalScore
        {
            get { return _userScore.GetSemiFinalScore(); }
        }

        public int BronseFinalScore
        {
            get { return _userScore.GetBronseFinalScore(); }
        }

        public int FinalScore
        {
            get { return _userScore.GetFinalScore(); }
        }

        public int Total
        {
            get { return _totalScore.GetTotal(); }
        }

        public int TotalGroup
        {
            get { return _totalScore.GetStageOneMatchScore() + _totalScore.GetQualifierScore(); }
        }

        public int TotalRound16
        {
            get { return _totalScore.GetRound16Score(); }
        }

        public int TotalQuarterFinal
        {
            get { return _totalScore.GetQuarterFinalScore(); }
        }

        public int TotalSemiFinal
        {
            get { return _totalScore.GetSemiFinalScore(); }
        }

        public int TotalBronseFinal
        {
            get { return _totalScore.GetBronseFinalScore(); }
        }

        public int TotalFinal
        {
            get { return _totalScore.GetFinalScore(); }
        }

        List<KnockoutMatch> _round16 = new List<KnockoutMatch>();
        public List<KnockoutMatch> Round16
        {
            get { return _round16; }
            private set { _round16 = value; }
        }

        List<KnockoutMatch> _quarterFinals = new List<KnockoutMatch>();
        public List<KnockoutMatch> QuarterFinals
        {
            get { return _quarterFinals; }
            private set { _quarterFinals = value; }
        }

        List<KnockoutMatch> _semiFinals = new List<KnockoutMatch>();
        public List<KnockoutMatch> SemiFinals
        {
            get { return _semiFinals; }
            private set { _semiFinals = value; }
        }

        List<KnockoutMatch> _bronseFinal = new List<KnockoutMatch>();
        public List<KnockoutMatch> BronseFinal
        {
            get { return _bronseFinal; }
            private set { _bronseFinal = value; }
        }

        List<KnockoutMatch> _final = new List<KnockoutMatch>();
		public List<KnockoutMatch> Final
		{
			get { return _final; }
			private set { _final = value; }
		}

		string GetSelectedWinnerClass(string team, object[] winners)
		{
		    if (Array.IndexOf(winners, team) != -1)
		        return "success";
		    else
		        return "normal";
		}
		string GetSelectedWinnerClass(string team, string winner)
		{
		    if (winner.Equals(team))
		        return "success";
		    else
		        return "normal";
		}

		void CreateRound16Matches()
        {
            for (int i = 0; i < _bet.GetStageOne().winners.Length; i += 2)
            {
                _round16.Add(Add16Match(i, i + 1));
                _round16.Add(Add16Match(i + 1, i));
            }
        }

        KnockoutMatch Add16Match(int i1, int i2)
        {
            var k = new KnockoutMatch();
            k.SelectedMatch = _bet.GetStageOne().winners[i1][0] + " vs. " + _bet.GetStageOne().winners[i2][1];
            if (_bet.HasRound16())
                k.SelectedWinner = _bet.GetRound16Winners()[i1].ToString();
            k.ActualMatch = _results.GetStageOne().winners[i1][0] + " vs. " + _results.GetStageOne().winners[i2][1];
            if (_results.HasRound16())
            {
                k.ActualWinner = _results.GetRound16Winners()[i1].ToString();
                k.CellClass = GetSelectedWinnerClass(k.SelectedWinner, _results.GetRound16Winners());
            }
            return k;
        }

		void CreateEuroRound16Matches()
		{
			List<string> userMatches = new List<string>();
			if (_bet.HasThirdPlaces())
			{
				userMatches = GetListOfMatches(_bet.GetStageOne().winners, _bet.GetThirdPlaces());
			}

			// Repeat for actual results
			List<string> resultMatches = new List<string>();
			if (_results.HasThirdPlaces())
			{
				resultMatches = GetListOfMatches(_results.GetStageOne().winners, _results.GetThirdPlaces());

			}

			for (int i = 0; i < userMatches.Count; i++)
			{
				var k = new KnockoutMatch();
				if (_bet.HasThirdPlaces())
				{
					k.SelectedMatch = userMatches[i];
					if (_bet.HasRound16())
						k.SelectedWinner = _bet.GetRound16Winners()[i].ToString();
				}
				if (_results.HasThirdPlaces())
				{
					k.ActualMatch = resultMatches[i];
					if (_results.HasRound16())
					{
						k.ActualWinner = _results.GetRound16Winners()[i].ToString();
						k.CellClass = GetSelectedWinnerClass(k.SelectedWinner, _results.GetRound16Winners());
					}
				}
				_round16.Add(k);
			}

		}

		List<string> GetListOfMatches(dynamic qualifiers, dynamic thirds)
		{
			
			// find groups
			List<char> groups = new List<char>();
			Dictionary<char, string> lookup = new Dictionary<char, string>();
			foreach (var team in thirds)
			{
				char group = _tournament.FindGroup(team);
				groups.Add(group);
				lookup[group] = team;
			}

			// lookup in table
			var matchUp = GetThirdPlaceMatchUp(groups);

			// create match combinations
			List<string> matches = new List<string>();
			// Match 1: Runner-up Group A v Runner-up Group C
			matches.Add(qualifiers[0][1] + " vs. " + qualifiers[2][1]);
			// Match 2: Winner Group D v 3rd Place Group B/E/F
			matches.Add(qualifiers[3][0] + " vs. " + lookup[matchUp[3]]);
			// Match 3: Winner Group B v 3rd Place Group A/C/D
			matches.Add(qualifiers[1][0] + " vs. " + lookup[matchUp[1]]);
			// Match 4: Winner Group F v Runner-up Group E
			matches.Add(qualifiers[5][0] + " vs. " + qualifiers[4][1]);
			// Match 5: Winner Group C v 3rd Place Group A/B/F
			matches.Add(qualifiers[2][0] + " vs. " + lookup[matchUp[2]]);
			// Match 6: Winner Group E v Runner-up Group D
			matches.Add(qualifiers[4][0] + " vs. " + qualifiers[3][1]);
			// Match 7: Winner Group A v 3rd Place Group C/D/E
			matches.Add(qualifiers[0][0] + " vs. " + lookup[matchUp[0]]);
			// Match 8: Runner-up Group B v Runner-up Group F
			matches.Add(qualifiers[1][1] + " vs. " + qualifiers[5][1]);

			return matches;
		}

		public List<char> GetThirdPlaceMatchUp(List<char> groups)
		{
			Dictionary<string, List<char>> combo = new Dictionary<string, List<char>>();
			combo["ABCD"] = new List<char>() { 'C', 'D', 'A', 'B' };
			combo["ABCE"] = new List<char>() { 'C', 'A', 'B', 'E' };
			combo["ABCF"] = new List<char>() { 'C', 'A', 'B', 'F' };
			combo["ABDE"] = new List<char>() { 'D', 'A', 'B', 'E' };
			combo["ABDF"] = new List<char>() { 'D', 'A', 'B', 'F' };
			combo["ABEF"] = new List<char>() { 'E', 'A', 'B', 'F' };
			combo["ACDE"] = new List<char>() { 'C', 'D', 'A', 'E' };
			combo["ACDF"] = new List<char>() { 'C', 'D', 'A', 'F' };
			combo["ACEF"] = new List<char>() { 'C', 'A', 'F', 'E' };
			combo["ADEF"] = new List<char>() { 'D', 'A', 'F', 'E' };
			combo["BCDE"] = new List<char>() { 'C', 'D', 'B', 'E' };
			combo["BCDF"] = new List<char>() { 'C', 'D', 'B', 'F' };
			combo["BCEF"] = new List<char>() { 'E', 'C', 'B', 'F' };
			combo["BDEF"] = new List<char>() { 'E', 'D', 'B', 'F' };
			combo["CDEF"] = new List<char>() { 'C', 'D', 'F', 'E' };

			if (groups[0] != '\0')
			{
				groups.Sort();
				string joined = new String(groups.ToArray());
				return combo[joined];
			}
			else
			{
				return new List<char>() { '\0', '\0', '\0', '\0' };
			}
		}

		void CreateQuarterFinalMatches()
        {
			int qw = 0;
            for (int i = 0; _bet.HasRound16() && i < _bet.GetRound16Winners().Length; i += 4)
            {
				_quarterFinals.Add(AddQuarterFinalMatch(i, i + 2, qw++));
                _quarterFinals.Add(AddQuarterFinalMatch(i + 1, i + 3, qw++));
            }
        }

		void CreateEuroQuarterFinalMatches()
		{
			int qw = 0;
			for (int i = 0; _bet.HasRound16() && i < _bet.GetRound16Winners().Length; i += 4)
			{
				_quarterFinals.Add(AddQuarterFinalMatch(i, i + 1, qw++));
				_quarterFinals.Add(AddQuarterFinalMatch(i + 2, i + 3, qw++));
			}
		}

        KnockoutMatch AddQuarterFinalMatch(int i1, int i2, int qw)
        {
            var k = new KnockoutMatch();
            k.SelectedMatch = _bet.GetRound16Winners()[i1] + " vs. " + _bet.GetRound16Winners()[i2];
            if (_bet.HasQuarterFinals())
            {
                k.SelectedWinner = _bet.GetQuarterFinalWinners()[qw].ToString();
            }
            if (_results.HasRound16())
            {
                k.ActualMatch = _results.GetRound16Winners()[i1] + " vs. " + _results.GetRound16Winners()[i2];
                if (_results.HasQuarterFinals())
                {
                    k.ActualWinner = _results.GetQuarterFinalWinners()[qw].ToString();
                    k.CellClass = GetSelectedWinnerClass(k.SelectedWinner, _results.GetQuarterFinalWinners());
                }   
            }
            return k;
        }

        void CreateSemiFinalMatches()
        {
            if (_bet.HasQuarterFinals())
            {
                _semiFinals.Add(AddSemiFinalMatch(0, 2));
                _semiFinals.Add(AddSemiFinalMatch(1, 3));
            }
        }

		void CreateEuroSemiFinalMatches()
		{
			if (_bet.HasQuarterFinals())
			{
				_semiFinals.Add(AddSemiFinalMatch(0, 1));
				_semiFinals.Add(AddSemiFinalMatch(2, 3));
			}
		}

        KnockoutMatch AddSemiFinalMatch(int i1, int i2)
        {
			int sw = (i1 < 2) ? i1 : i1 - 1;
            var k = new KnockoutMatch();
            k.SelectedMatch = _bet.GetQuarterFinalWinners()[i1] + " vs. " + _bet.GetQuarterFinalWinners()[i2];
            if (_bet.HasSemiFinals())
            {
                k.SelectedWinner = _bet.GetSemiFinalWinners()[sw].ToString();
            }
            if (_results.HasQuarterFinals())
            {
                k.ActualMatch = _results.GetQuarterFinalWinners()[i1] + " vs. " + _results.GetQuarterFinalWinners()[i2];
                if (_results.HasSemiFinals())
                {
                    k.ActualWinner = _results.GetSemiFinalWinners()[sw].ToString();
                    k.CellClass = GetSelectedWinnerClass(k.SelectedWinner, _results.GetSemiFinalWinners());
                }
            }
            return k;
        }

		void CreateBronseFinalMatch()
		{
		    if (!_bet.HasSemiFinals())
		        return;
		    
            var k = new KnockoutMatch();
            k.SelectedMatch = _bet.GetBronseFinalists()[0] + " vs. " + _bet.GetBronseFinalists()[1];
            if (_bet.HasBronseFinal())
            {
                k.SelectedWinner = _bet.GetBronseFinalWinner();
            }
            if (_results.HasSemiFinals())
            {
                var bf = _results.GetBronseFinalists();
                if (bf.Count > 0)
                    k.ActualMatch = _results.GetBronseFinalists()[0] + " vs. " + _results.GetBronseFinalists()[1];
                else
                    k.ActualMatch = "- vs. -";
                if (_results.HasBronseFinal())
                {
                    k.ActualWinner = _results.GetBronseFinalWinner();
                    k.CellClass = GetSelectedWinnerClass(k.SelectedWinner, _results.GetBronseFinalWinner());
                }
            }
            _bronseFinal.Add(k);
		}

		void CreateFinalMatch()
		{
		    if (!_bet.HasSemiFinals())
		        return;
		    
            var k = new KnockoutMatch();
            k.SelectedMatch = _bet.GetSemiFinalWinners()[0] + " vs. " + _bet.GetSemiFinalWinners()[1];
            if (_bet.HasFinal())
            {
                k.SelectedWinner = _bet.GetFinalWinner();
            }
            if (_results.HasSemiFinals())
            {
                k.ActualMatch = _results.GetSemiFinalWinners()[0] + " vs. " + _results.GetSemiFinalWinners()[1];
                if (_results.HasFinal())
                {
                    k.ActualWinner = _results.GetFinalWinner();
                    k.CellClass = GetSelectedWinnerClass(k.SelectedWinner, _results.GetFinalWinner());
                }
            }
            _final.Add(k);
		}
		
        void CreateGroupMatches()
        {
            char gn = 'A';
            int i = 0;
            foreach (object[] group in _tournament.GetGroups())
            {
                dynamic stageOne = _bet.GetStageOne();
                dynamic stageOneActual = _results.GetStageOne();
                var g = new GroupMatches() { Name = "Group " + gn };
                g.CreateMatches(group, stageOne.results[i], stageOneActual.results[i]);
                g.AddQualifiers(stageOne.winners[i], stageOneActual.winners[i], stageOneActual.results[i]);
                _groups.Add(g);
                gn++;
                i++;
            }
        }
		
		
        public class GroupMatches
        {
            public string Name { get; set; }
			
            List<Tuple<string, string, string, string>> _matches = new List<Tuple<string, string, string, string>>();

            List<string> _betQualifiers = new List<string>();

            List<string> _actualQualifiers = new List<string>();
            
            public string MatchesAsHtml
            {
                get {
                    StringBuilder s = new StringBuilder();
                    s.Append("<thead>\n<tr>\n<th>Match</th>\n<th>Selected</th>\n<th>Actual</th>\n</tr>\n</thead>");
                    s.AppendLine();
                    s.Append("<tbody>");
                    s.AppendLine();
                    foreach (var match in _matches)
                    {
                        string selected = GetResults(match, match.Item3);
                        string actual = GetResults(match, match.Item4);
                        s.Append(GetTr(selected, actual));
                        s.AppendLine();
                        s.AppendFormat("	<td>{0} vs. {1}</td>", match.Item1, match.Item2);
                        s.AppendLine();
                        s.AppendFormat("	<td>{0}</td>", selected);
                        s.AppendLine();
                        s.AppendFormat("	<td>{0}</td>", actual);
                        s.AppendLine();
                        s.Append("</tr>");
                        s.AppendLine();
                    }
					
                    // Qualifiers
                    s.Append("<tr>");
                    s.AppendLine();
                    s.Append(" <td></td>\n<td></td>\n<td></td>");
                    s.AppendLine();
                    s.Append(GetTr(_betQualifiers[0], _actualQualifiers[0], _actualQualifiers));
                    s.AppendLine();
                    s.Append(" <td><b>Winner</b></td>");
                    s.AppendLine();
                    s.AppendFormat("	<td>{0}</td>", _betQualifiers[0]);
                    s.AppendLine();
                    s.AppendFormat("	<td>{0}</td>", _actualQualifiers[0]);
                    s.AppendLine();
                    s.Append("</tr>");
                    s.AppendLine();
                    s.Append(GetTr(_betQualifiers[1], _actualQualifiers[1], _actualQualifiers));
                    s.AppendLine();
                    s.Append(" <td><b>Runner-up</b></td>");
                    s.AppendLine();
                    s.AppendFormat("	<td>{0}</td>", _betQualifiers[1]);
                    s.AppendLine();
                    s.AppendFormat("	<td>{0}</td>", _actualQualifiers[1]);
                    s.AppendLine();
                    s.Append("</tr>");
                    s.AppendLine();
                    s.Append("</tbody>");
                    return s.ToString();
                }
            }

            string GetTr(string selected, string actual, List<string> qual = null)
            {
                if (selected.Equals(actual))
                    return "<tr class=\"success\">";
                else if (qual != null && qual.Contains(selected))
                    return "<tr class=\"warning\">";
                else if (actual.Length == 0)
                    return "<tr class=\"active\">";
                else
                    return "<tr>";
            }
			
            public void CreateMatches(object[] group, object[] results, object[] actuals)
            {
                _matches.Add(new Tuple<string, string, string, string>(group[0].ToString(), group[1].ToString(), 
                    results[0].ToString(), actuals[0].ToString()));
                _matches.Add(new Tuple<string, string, string, string>(group[2].ToString(), group[3].ToString(), 
                    results[1].ToString(), actuals[1].ToString()));
                _matches.Add(new Tuple<string, string, string, string>(group[0].ToString(), group[2].ToString(), 
                    results[2].ToString(), actuals[2].ToString()));
                _matches.Add(new Tuple<string, string, string, string>(group[3].ToString(), group[1].ToString(), 
                    results[3].ToString(), actuals[3].ToString()));
                _matches.Add(new Tuple<string, string, string, string>(group[3].ToString(), group[0].ToString(), 
                    results[4].ToString(), actuals[4].ToString()));
                _matches.Add(new Tuple<string, string, string, string>(group[1].ToString(), group[2].ToString(), 
                    results[5].ToString(), actuals[5].ToString()));
            }

            public void AddQualifiers(object[] bet, object[] actual, object[] results)
            {
                foreach (var team in bet)
                {
                    _betQualifiers.Add(team.ToString());
                }
                int index = Array.FindIndex(results, r => r.ToString() == "-");
                foreach (var team in actual)
                {
                    if (index == -1)
                        _actualQualifiers.Add(team.ToString());
                    else
                        _actualQualifiers.Add("");
                }
            }
            public string GetResults(Tuple<string, string, string, string> match)
            {
                return GetResults(match, match.Item3);
            }
            public string GetResults(Tuple<string, string, string, string> match, string result)
            {
                if (result.ToLower().Equals("h"))
                {
                    return match.Item1;
                } else if (result.ToLower().Equals("b"))
                {
                    return match.Item2;
                } else if (result.ToLower().Equals("u"))
                {
                    return "Draw";
                } else
                {
                    return "";
                }
            }
        }
        
        public class KnockoutMatch
        {
            public string SelectedMatch { get; set; }
            public string SelectedWinner { get; set; }
            public string ActualMatch { get; set; }
            public string ActualWinner { get; set; }
            public string CellClass { get; set; }
        }

    }
}
