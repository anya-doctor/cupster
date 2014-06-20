﻿using Nancy;
using SubmittedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules
{
    public class BetterPageModule : NancyModule
    {
        public BetterPageModule(ITournament tournament, ISubmittedBets bets)
        {
            Get["/{better}"] = _ =>
            {
            	return View["betterpage.sshtml", new BetterPageViewModel(tournament, bets.GetSingleBet(_.better))];
            };
        }
    }
}
