/*
 * Created by SharpDevelop.
 * User: Lars Magnus
 * Date: 12.06.2014
 * Time: 20:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text;
using Nancy;
using SubmittedData;

namespace Modules
{
	/// <summary>
	/// Description of WebService.
	/// </summary>
	public class LegendPageModule : NancyModule
	{
		public LegendPageModule(IResultCollection rc)
		{
			Get["/legend"] = _ => 
            {
				return View["legend.sshtml", new LegendPageViewModel(rc.Current)];
			};
		}
	}
	
	
}
