/*
 * Created by SharpDevelop.
 * User: Lars Magnus
 * Date: 21.06.2014
 * Time: 22:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;

namespace SubmittedData
{
    /// <summary>
    /// Description of ScoringSystem.
    /// </summary>
    public class ScoringSystem
    {
        IResults _user;
        IResults _actual;

		public static class Points
		{
			public const int StageOneMatchOutcome = 1;
			public const int QualifyingTeam = 2;
			public const int QualifyingPosition = 2;
			public const int Round16Winner = 4;
			public const int QuarterFinalWinner = 8;
			public const int SemiFinalWinner = 16;
			public const int BronseFinalWinner = 16;
			public const int FinalWinner = 32;
		}
		
        public ScoringSystem(IResults user, IResults actual)
        {
            _user = user;
            _actual = actual;
        }
        
        public int GetTotal()
        {
            int score = 0;
            score += GetStageOneMatchScore();
            score += GetQualifierScore();
            score += GetRound16Score();
            score += GetQuarterFinalScore();
            score += GetSemiFinalScore();
            score += GetBronseFinalScore();
            score += GetFinalScore();
            return score;
        }

        public int GetTotalWithoutBronse()
        {
            int score = 0;
            score += GetStageOneMatchScore();
            score += GetQualifierScore();
            score += GetRound16Score();
            score += GetQuarterFinalScore();
            score += GetSemiFinalScore();
            score += GetFinalScore();
            return score;
        }

        // 1 point per correct match outcome (win/loss/draw)
        public int GetStageOneMatchScore()
        {
            var stageOne = _actual.GetStageOne();
            int score = 0;
            var userStageOne = _user.GetStageOne();
            for (int i = 0; i < userStageOne.results.Length; i++)
            {
                for (int j = 0; j < userStageOne.results[i].Length; j++)
                {
                    var actual = stageOne.results[i][j].ToLower();
                    if (actual != "-" && userStageOne.results[i][j].ToLower() == actual)
						score += Points.StageOneMatchOutcome;
                }
            }
            return score;
        }

        // 2 points per correct qualifier
        // 2 points per correct position (winner/runner-up)
        public int GetQualifierScore()
        {
            var stageOne = _actual.GetStageOne();
            int score = 0;
            var userStageOne = _user.GetStageOne();
            for (int i = 0; i < userStageOne.winners.Length; i++)
            {
                for (int j = 0; j < userStageOne.winners[i].Length; j++)
                {
                    var team = userStageOne.winners[i][j];
                    
                    if (stageOne.winners[i][j] == "-")
                        continue;
                    if (Array.IndexOf(stageOne.winners[i], team) != -1)
						score += Points.QualifyingTeam;
		            
                    if (team.Equals(stageOne.winners[i][j]))
						score += Points.QualifyingPosition;
                }
		        
            }
            return score;
        }

        // 8 points per correct winner
        public int GetRound16Score()
        {
            int score = 0;
            if (_actual.HasRound16())
            {
                foreach (var team in _user.GetRound16Winners())
                {
                    if (team != "-" && Array.IndexOf(_actual.GetRound16Winners(), team) != -1)
    					score += Points.Round16Winner;
                }
    
            }            
            return score;
        }

        // 16 points per correct winner
        public int GetQuarterFinalScore()
        {
            int score = 0;
            if (_actual.HasQuarterFinals())
            {
                foreach (var team in _user.GetQuarterFinalWinners())
                {
                    if (team != "-" && Array.IndexOf(_actual.GetQuarterFinalWinners(), team) != -1)
						score += ScoringSystem.Points.QuarterFinalWinner;
                }
            }
            return score;
        }

        // 32 points per correct winner
        public int GetSemiFinalScore()
        {
            int score = 0;
            if (_actual.HasSemiFinals())
            {
                foreach (var team in _user.GetSemiFinalWinners())
                {
                    if (team != "-" && Array.IndexOf(_actual.GetSemiFinalWinners(), team) != -1)
						score += ScoringSystem.Points.SemiFinalWinner;
                }
            }
            return score;
        }

        // 16 points for correct winner
        public int GetBronseFinalScore()
        {
            int score = 0;
            if (_actual.HasBronseFinal() && _actual.GetBronseFinalWinner() != "-"
                && (_user.GetBronseFinalWinner() == _actual.GetBronseFinalWinner()))
				score += ScoringSystem.Points.BronseFinalWinner;
            return score;
        }

        // 32 points for correct winner
        public int GetFinalScore()
        {
            int score = 0;
            if (_actual.HasFinal() &&_actual.GetFinalWinner() != "-"
                && (_user.GetFinalWinner() == _actual.GetFinalWinner()))
				score += ScoringSystem.Points.FinalWinner;
            return score;
        }
    }
}
