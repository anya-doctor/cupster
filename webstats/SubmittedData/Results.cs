/*
 * Created by SharpDevelop.
 * User: Lars Magnus
 * Date: 20.06.2014
 * Time: 22:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using Toml;

namespace SubmittedData
{
    /// <summary>
    /// Description of ActualResults.
    /// </summary>

    public class Results : IResults
    {
        readonly IFileSystem _fileSystem;
        public Results(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        public Results()
            : this(
                fileSystem: new FileSystem()
            )
        {
        }


        dynamic _results;
        public Results(dynamic results)
            : this(
                fileSystem: new FileSystem()
            )
        {
            _results = results;
        }


        #region IResults implementation

        public void Load(string file)
        {
            if (_fileSystem.File.Exists(file))
            {
                string text = _fileSystem.File.ReadAllText(file);
                _results = text.ParseAsToml();
                _timeStamp = _fileSystem.File.GetLastWriteTime(file).ToString("F", CultureInfo.InvariantCulture);
                
            }        
            else
            {
                string text = @"[info]
user = ""actual""
";
                _results = text.ParseAsToml();
            }
        }

        string _timeStamp;
		public string GetTimeStamp()
		{
		    return _timeStamp;
		}

		dynamic GetSection(string section)
		{
			return ((IDictionary<String, Object>)_results)[section];
		}

		bool HasSection(string section)
		{
			return ((IDictionary<String, Object>)_results).ContainsKey(section);
		}

		dynamic GetKey(string section, string key)
		{
			var sec = GetSection(section);
			return ((IDictionary<String, Object>)sec)[key];
		}

		bool HasKey(string section, string key)
		{
			if (HasSection(section))
			{
				var sec = GetSection(section);
				return ((IDictionary<String, Object>)sec).ContainsKey(key);

			}
			else
			{
				return false;
			}
		}
		
		public bool HasStageOne()
		{
			return HasSection("stage-one");
		}

		public dynamic GetStageOne()
		{
			return GetSection("stage-one");
		}

		public bool IsStageOneComplete()
		{
			bool foundDash = false;
			for (int i = 0; i < GetStageOne().results.Length; i++)
			{
				for (int j = 0; j < GetStageOne().results[i].Length; j++)
				{
					if (GetStageOne().results[i][j] == "-")
					{
						foundDash = true;
						break;
					}
				}
			}
			return !foundDash;
		}

		public dynamic GetThirdPlaces()
		{
			return GetKey("stage-one", "third-places");
		}

		public bool HasThirdPlaces()
		{
			return HasKey("stage-one", "third-places");
		}

		public dynamic GetInfo()
        {
            return _results.info;
        }

		public bool HasStageTwo()
		{
			return HasSection("stage-two");
		}
		
        public bool HasRound16()
        {
			return HasKey("stage-two", "round-of-16");
        }

		public dynamic GetRound16Winners()
        {
			return GetKey("stage-two", "round-of-16");
        }

        public bool HasQuarterFinals()
        {
			return HasKey("stage-two", "quarter-final");
        }

		public dynamic GetQuarterFinalWinners()
        {
			return GetKey("stage-two", "quarter-final");
        }

		public bool HasSemiFinals()
        {
			return HasKey("stage-two", "semi-final");
        }
        public dynamic GetSemiFinalWinners()
        {
			return GetKey("stage-two", "semi-final");
        }
    	
        public List<string> GetBronseFinalists()
        {
            List<string> bfinalists = new List<string>();
            foreach (var team in GetQuarterFinalWinners())
            {
                object[] sf = GetSemiFinalWinners();
                if (Array.IndexOf(sf, team) == -1)
                {
                    bfinalists.Add(team);
                }
            }
            return bfinalists;
        }

        public bool HasBronseFinal()
        {
			return HasKey("finals", "bronse-final");
        }

        public string GetBronseFinalWinner()
        {
			return GetKey("finals", "bronse-final").ToString();
        }
        
        public bool HasFinal()
        {
			return HasKey("finals", "final") && !String.Equals(GetFinalWinner(), "-");
        }
        
        public string GetFinalWinner()
        {
            return _results.finals.final.ToString();
        }

        #endregion
    }
}
