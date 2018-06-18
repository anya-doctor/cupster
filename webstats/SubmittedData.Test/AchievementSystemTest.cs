/*
 * Created by SharpDevelop.
 * User: Lars Magnus
 * Date: 30.06.2014
 * Time: 19:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;
using Shouldly;
using Toml;

namespace SubmittedData.Test
{
    /// <summary>
    /// Description of AchievementSystemTest.
    /// </summary>
    [TestFixture]
    public class AchievementSystemTest
    {
        [Test]
        [Ignore("Not implemented feature")]
        public void TestPerfectGroup_ReturnsTrue_WhenAllGroupMatchesAndBothQualifiersAreCorrect()
        {
		    string userBet = @"[stage-one]
winners = [ [ ""Germany"", ""Algerie"",], ]
";
		    string actualResults = @"[stage-one]
winners = [ [ ""Brasil"", ""Mexico"",], ]
";
		    var user = new Results(userBet.ParseAsToml());
		    var actual = new Results(actualResults.ParseAsToml());

		    var a = new AchievementSystem(user, actual);
        }
        
        [Test]
        public void TestAchievements_ShouldNotContainDoubleRainbow_WhenNoGroupWithBothQualifiers()
        {
		    string userBet = @"[stage-one]
winners = [ [ ""Germany"", ""Algerie"",], ]
results = [ [ ] ]
";
		    string actualResults = @"[stage-one]
winners = [ [ ""Brasil"", ""Mexico"",], ]
results = [ [ ] ]
";
		    var user = new Results(userBet.ParseAsToml());
		    var actual = new Results(actualResults.ParseAsToml());

		    var a = new AchievementSystem(user, actual);
		    a.Achievements.ShouldNotContain(a.AchievementRepo[AchievementTypes.DoubleRainbow]);
        }

        [Test]
        public void TestAchievements_ShouldContainDoubleRainbow_WhenAtLeastOneGroupWithBothQualifiers()
        {
		    string userBet = @"[stage-one]
winners = [ [ ""Brasil"", ""Algerie"",], [ ""Spania"", ""Nederland"",], ]
results = [ [ ] ]
";
		    string actualResults = @"[stage-one]
winners = [ [ ""Brasil"", ""Mexico"",], [ ""Spania"", ""Nederland"",], ]
results = [ [ ] ]
";
		    var user = new Results(userBet.ParseAsToml());
		    var actual = new Results(actualResults.ParseAsToml());

		    var a = new AchievementSystem(user, actual);
		    a.Achievements.ShouldContain(a.AchievementRepo[AchievementTypes.DoubleRainbow]);
        }
        
        [Test]
        public void TestAchievements_ShouldContainNotEvenClose_WhenBothFinalistsAreKnockedOutInGroup()
        {
		    string userBet = @"[stage-two]
semi-final = [ ""Spania"", ""Italia"",]
";
		    string actualResults = @"[stage-one]
results = [ [ ""h"", ""h"", ""u"", ""b"", ""b"", ""b"",], [ ""b"", ""h"", ""b"", ""b"", ""b"", ""h"",], [ ""h"", ""h"", ""h"", ""u"", ""b"", ""h"",], [ ""b"", ""b"", ""h"", ""b"", ""b"", ""u"",], [ ""h"", ""h"", ""b"", ""b"", ""b"", ""u"",], [ ""h"", ""u"", ""h"", ""h"", ""b"", ""h"",], [ ""h"", ""b"", ""u"", ""u"", ""b"", ""h"",], [ ""h"", ""u"", ""h"", ""b"", ""b"", ""u"",],]
winners = [ [ ""Brasil"", ""Mexico"",], [ ""Nederland"", ""Chile"",], [ ""Colombia"", ""Hellas"",], ]
[stage-two]
";
		    var user = new Results(userBet.ParseAsToml());
		    var actual = new Results(actualResults.ParseAsToml());

		    var a = new AchievementSystem(user, actual);
		    a.Achievements.ShouldContain(a.AchievementRepo[AchievementTypes.NotEvenClose]);
        }

		[Test]
		public void TestAchievements_ShouldNotContainNotEvenClose_WhenStageOneIsNotComplete()
		{
			string userBet = @"[stage-two]
semi-final = [ ""Spania"", ""Italia"",]
";
			string actualResults = @"[stage-one]
results = [ [ ""h"", ""h"", ""u"", ""b"", ""b"", ""b"",], [ ""b"", ""h"", ""b"", ""b"", ""b"", ""h"",], [ ""h"", ""h"", ""h"", ""u"", ""-"", ""-"",], [ ""b"", ""b"", ""h"", ""b"", ""b"", ""u"",], [ ""h"", ""h"", ""b"", ""b"", ""b"", ""u"",], [ ""h"", ""u"", ""h"", ""h"", ""b"", ""h"",], [ ""h"", ""b"", ""u"", ""u"", ""b"", ""h"",], [ ""h"", ""u"", ""h"", ""b"", ""b"", ""u"",],]
winners = [ [ ""Brasil"", ""Mexico"",], [ ""Nederland"", ""Chile"",], [ ""Colombia"", ""Hellas"",], ]
[stage-two]
";
			var user = new Results(userBet.ParseAsToml());
			var actual = new Results(actualResults.ParseAsToml());

			var a = new AchievementSystem(user, actual);
			a.Achievements.ShouldNotContain(a.AchievementRepo[AchievementTypes.NotEvenClose]);
		}

		[Test]
        public void TestAchievements_ShouldContainCompleteMiss_WhenGroupwithNoCorrectMatches()
        {
		    string userBet = @"[info]
user = ""user1""
[stage-one]
results = [ [ ""H"", ""B"", ""H"", ""H"", ""B"", ""H"",], [ ""B"", ""B"", ""H"", ""B"", ""B"", ""H"",], [ ""U"", ""B"", ""U"", ""H"", ""H"", ""U"",], [ ""B"", ""B"", ""B"", ""H"", ""H"", ""B"",], [ ""B"", ""H"", ""B"", ""B"", ""U"", ""B"",], [ ""H"", ""U"", ""H"", ""U"", ""B"", ""H"",], [ ""H"", ""B"", ""H"", ""B"", ""b"", ""h"",], [ ""b"", ""h"", ""b"", ""u"", ""u"", ""u"",],]
winners = [ [ ""Brasil"", ""Mexico"",], [ ""Spania"", ""Nederland"",], ]
";
		    string actualResults = @"[info]
user = ""user1""
[stage-one]
results = [ [ ""h"", ""h"", ""u"", ""b"", ""b"", ""b"",], [ ""b"", ""h"", ""b"", ""b"", ""b"", ""h"",], [ ""h"", ""h"", ""h"", ""u"", ""b"", ""h"",], [ ""b"", ""b"", ""h"", ""b"", ""b"", ""u"",], [ ""h"", ""h"", ""b"", ""b"", ""b"", ""u"",], [ ""h"", ""u"", ""h"", ""h"", ""b"", ""h"",], [ ""h"", ""b"", ""u"", ""u"", ""b"", ""h"",], [ ""h"", ""u"", ""h"", ""b"", ""b"", ""u"",],]
winners = [ [ ""Brasil"", ""Mexico"",], [ ""Spania"", ""Nederland"",], ]
";
		    var user = new Results(userBet.ParseAsToml());
		    var actual = new Results(actualResults.ParseAsToml());

		    var a = new AchievementSystem(user, actual);
		    a.Achievements.ShouldContain(a.AchievementRepo[AchievementTypes.CompleteMiss]);
        }

		[Test]
		public void TestAchievements_ShouldNotContainCompleteMiss_WhenStageOneIsNotComplete()
		{
			string userBet = @"[info]
user = ""user1""
[stage-one]
results = [ [ ""H"", ""B"", ""H"", ""H"", ""B"", ""H"",], [ ""B"", ""B"", ""H"", ""B"", ""B"", ""H"",], [ ""U"", ""B"", ""U"", ""H"", ""H"", ""U"",], [ ""B"", ""B"", ""B"", ""H"", ""H"", ""B"",], [ ""B"", ""H"", ""B"", ""B"", ""U"", ""B"",], [ ""H"", ""U"", ""H"", ""U"", ""B"", ""H"",], [ ""H"", ""B"", ""H"", ""B"", ""b"", ""h"",], [ ""b"", ""h"", ""b"", ""u"", ""u"", ""u"",],]
winners = [ [ ""Brasil"", ""Mexico"",], [ ""Spania"", ""Nederland"",], ]
";
			string actualResults = @"[info]
user = ""user1""
[stage-one]
results = [ [ ""h"", ""h"", ""u"", ""b"", ""b"", ""b"",], [ ""b"", ""h"", ""b"", ""b"", ""b"", ""h"",], [ ""h"", ""h"", ""h"", ""u"", ""-"", ""-"",], [ ""b"", ""b"", ""h"", ""b"", ""b"", ""u"",], [ ""h"", ""h"", ""b"", ""b"", ""b"", ""u"",], [ ""h"", ""u"", ""h"", ""h"", ""b"", ""h"",], [ ""h"", ""b"", ""u"", ""u"", ""b"", ""h"",], [ ""h"", ""u"", ""h"", ""b"", ""b"", ""u"",],]
winners = [ [ ""Brasil"", ""Mexico"",], [ ""Spania"", ""Nederland"",], ]
";
			var user = new Results(userBet.ParseAsToml());
			var actual = new Results(actualResults.ParseAsToml());

			var a = new AchievementSystem(user, actual);
			a.Achievements.ShouldNotContain(a.AchievementRepo[AchievementTypes.CompleteMiss]);
		}

		[Test]
        public void TestAchievements_ShouldContainSweet16_WhenEveryRound16WinnerIsCorrect()
        {
		    string userBet = @"[stage-two]
round-of-16 = [ ""Brasil"", ""Nederland"", ""Colombia"", ]
";
		    string actualResults = @"[stage-two]
round-of-16 = [ ""Brasil"", ""Nederland"", ""Colombia"", ]
";
		    var user = new Results(userBet.ParseAsToml());
		    var actual = new Results(actualResults.ParseAsToml());

		    var a = new AchievementSystem(user, actual);
		    a.Achievements.ShouldContain(a.AchievementRepo[AchievementTypes.Sweet16]);
        }

        [Test]
        public void TestAchievements_ShouldNotContainSweet16_WhenAtLeastOneRound16WinnerIsWrong()
        {
		    string userBet = @"[info]
user = ""user1""
[stage-two]
round-of-16 = [ ""Brasil"", ""Tyskland"", ""Colombia"", ]
";
		    string actualResults = @"[info]
user = ""user1""
[stage-two]
round-of-16 = [ ""Brasil"", ""Nederland"", ""Colombia"", ]
";
		    var user = new Results(userBet.ParseAsToml());
		    var actual = new Results(actualResults.ParseAsToml());

		    var a = new AchievementSystem(user, actual);
		    a.Achievements.ShouldNotContain(a.AchievementRepo[AchievementTypes.Sweet16]);
        }

        [Test]
        public void TestAchievements_ShouldContainQuarterback_WhenEveryQuarterFinalWinnerIsCorrect()
        {
		    string userBet = @"[stage-two]
quarter-final = [ ""Brasil"", ""Nederland"", ""Tyskland"", ""Argentina"",]
";
		    string actualResults = @"[stage-two]
quarter-final = [ ""Brasil"", ""Nederland"", ""Tyskland"", ""Argentina"",]
";
		    var user = new Results(userBet.ParseAsToml());
		    var actual = new Results(actualResults.ParseAsToml());

		    var a = new AchievementSystem(user, actual);
		    a.Achievements.ShouldContain(a.AchievementRepo[AchievementTypes.Quarterback]);
        }

        [Test]
        public void TestAchievements_ShouldNotContainQuarterback_WhenAtLeastOneQuarterFinalWinnerIsWrong()
        {
		    string userBet = @"[stage-two]
quarter-final = [ ""Brasil"", ""Nederland"", ""Tyskland"", ""Frankrike"",]
";
		    string actualResults = @"[stage-two]
quarter-final = [ ""Brasil"", ""Nederland"", ""Tyskland"", ""Argentina"",]
";
		    var user = new Results(userBet.ParseAsToml());
		    var actual = new Results(actualResults.ParseAsToml());

		    var a = new AchievementSystem(user, actual);
		    a.Achievements.ShouldNotContain(a.AchievementRepo[AchievementTypes.Quarterback]);
        }

        [Test]
        public void TestAchievements_ShouldContainBronse_WhenBronseFinalWinnerIsCorrect()
        {
		    string userBet = @"[finals]
bronse-final = ""Brasil""
";
		    string actualResults = @"[finals]
bronse-final = ""Brasil""
";
		    var user = new Results(userBet.ParseAsToml());
		    var actual = new Results(actualResults.ParseAsToml());

		    var a = new AchievementSystem(user, actual);
		    a.Achievements.ShouldContain(a.AchievementRepo[AchievementTypes.Bronze]);
        }

        [Test]
        public void TestAchievements_ShouldContainSilver_WhenRunnerUpIsCorrect()
        {
		    string userBet = @"[stage-two]
semi-final = [ ""Tyskland"", ""Argentina"",]
[finals]
final = ""Tyskland""
";
		    string actualResults = @"[stage-two]
semi-final = [ ""Tyskland"", ""Argentina"",]
[finals]
final = ""Tyskland""
";
		    var user = new Results(userBet.ParseAsToml());
		    var actual = new Results(actualResults.ParseAsToml());

		    var a = new AchievementSystem(user, actual);
		    a.Achievements.ShouldContain(a.AchievementRepo[AchievementTypes.Silver]);
        }

        [Test]
        public void TestAchievements_ShouldNotContainSilver_WhenFinalIsNotPlayed()
        {
		    string userBet = @"[stage-two]
semi-final = [ ""Tyskland"", ""Argentina"",]
[finals]
final = ""Tyskland""
";
		    string actualResults = @"[stage-two]
semi-final = [ ""Tyskland"", ""Argentina"",]
[finals]
final = ""-""
";
		    var user = new Results(userBet.ParseAsToml());
		    var actual = new Results(actualResults.ParseAsToml());

		    var a = new AchievementSystem(user, actual);
		    a.Achievements.ShouldNotContain(a.AchievementRepo[AchievementTypes.Silver]);
        }

        [Test]
        public void TestAchievements_ShouldNotContainSilver_WhenRunnerUpIsSelectedAsWinner()
        {
		    string userBet = @"[stage-two]
semi-final = [ ""Tyskland"", ""Argentina"",]
[finals]
final = ""Argentina""
";
		    string actualResults = @"[stage-two]
semi-final = [ ""Tyskland"", ""Argentina"",]
[finals]
final = ""Tyskland""
";
		    var user = new Results(userBet.ParseAsToml());
		    var actual = new Results(actualResults.ParseAsToml());

		    var a = new AchievementSystem(user, actual);
		    a.Achievements.ShouldNotContain(a.AchievementRepo[AchievementTypes.Silver]);
        }

        [Test]
        public void TestAchievements_ShouldContainGold_WhenFinalWinnerIsCorrect()
        {
		    string userBet = @"[finals]
final = ""Tyskland""
";
		    string actualResults = @"[finals]
final = ""Tyskland""
";
		    var user = new Results(userBet.ParseAsToml());
		    var actual = new Results(actualResults.ParseAsToml());

		    var a = new AchievementSystem(user, actual);
		    a.Achievements.ShouldContain(a.AchievementRepo[AchievementTypes.Gold]);
        }
        
        [Test]
        public void TestAchivements_ShouldContainFullPot_WhenScoreIsMax()
        {
            
            string userBet = @"[stage-one]
winners = [ [ ""Germany"", ""Algerie"",], ]
results = [ [ ] ]
";
            string actualResults = @"[stage-one]
winners = [ [ ""Germany"", ""Algerie"",], ]
results = [ [ ] ]
";
            var user = new Results(userBet.ParseAsToml());
            var actual = new Results(actualResults.ParseAsToml());

            var a = new AchievementSystem(user, actual);
            a.Achievements.ShouldContain(a.AchievementRepo[AchievementTypes.FullPot]);
        }

        [Test]
        public void TestAchivements_ShouldNotContainFullPot_WhenScoreIsLessThanMax()
        {
            
            string userBet = @"[stage-one]
winners = [ [ ""Germany"", ""Algerie"",], ]
results = [ [ ] ]
";
            string actualResults = @"[stage-one]
winners = [ [ ""Mexico"", ""Algerie"",], ]
results = [ [ ] ]
";
            var user = new Results(userBet.ParseAsToml());
            var actual = new Results(actualResults.ParseAsToml());

            var a = new AchievementSystem(user, actual);
            a.Achievements.ShouldNotContain(a.AchievementRepo[AchievementTypes.FullPot]);
        }
    }
}
