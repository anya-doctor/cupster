/*
 * Created by SharpDevelop.
 * User: Lars Magnus
 * Date: 14.06.2014
 * Time: 23:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Diagnostics;
using Nancy.TinyIoc;
using SubmittedData;

namespace Modules
{
    /// <summary>
    /// Description of Bootstrapper.
    /// </summary>
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            string dataPath = ConfigurationManager.AppSettings["DataPath"].ToString();
            string tournamentFile = ConfigurationManager.AppSettings["TournamentFile"].ToString();
            string resultsFile = ConfigurationManager.AppSettings["ResultsFile"].ToString();

            // Register our app dependency as a normal singletons
            var tournament = new Tournament();
            tournament.Load(Path.Combine(dataPath, tournamentFile));
            container.Register<ITournament, Tournament>(tournament);

            var bets = new SubmittedBets();
            bets.TournamentFile = tournamentFile;
            bets.ActualResultsFile = resultsFile;
            bets.LoadAll(dataPath);
            container.Register<ISubmittedBets, SubmittedBets>(bets);

            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["LiveResultsUrl"]) ||
                !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["LiveResultsInterval"]))
            {
                RegisterLiveResultsCollection(container, tournament);
            }
            else
            {
                var resultCollection = new ResultCollection();
                resultCollection.Current = new Results();
                resultCollection.Current.Load(Path.Combine(dataPath, resultsFile));
                resultCollection.Previous = new Results();
                resultCollection.Previous.Load(Path.Combine(dataPath, resultsFile + ".bak"));
                container.Register<IResultCollection, ResultCollection>(resultCollection);
            }
        }

        private static void RegisterLiveResultsCollection(TinyIoCContainer container, Tournament tournament)
        {
            var resultCollection = new LiveResultsCollection
            {
                Current = new LiveResults(tournament),
                Previous = new LiveResults(tournament)
            };
            resultCollection.Current.Load(ConfigurationManager.AppSettings["LiveResultsUrl"]);
            resultCollection.Previous.Load(ConfigurationManager.AppSettings["LiveResultsUrl"]);

            Task.Run(async () => { await resultCollection.UpdateResultsAsync(new TimeSpan(0, int.Parse(ConfigurationManager.AppSettings["LiveResultsInterval"]), 0)); });
            container.Register<IResultCollection, LiveResultsCollection>(resultCollection);
        }

        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);

            conventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("Scripts")
            );
        }

        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get { return new DiagnosticsConfiguration { Password = @"kokko-bada-futu" }; }
        }

#if DEBUG
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            StaticConfiguration.DisableErrorTraces = false;
        }
#endif

    }
}
