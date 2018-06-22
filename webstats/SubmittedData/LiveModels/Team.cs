using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SubmittedData.LiveModels
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class Team
    {
        public int Id { get; set; }
        public string Country { get; set; }
        
        public string Code { get; set; }
        public int Goals { get; set; }
        public string TeamTbd { get; set; }
        public int Points { get; set; }

        public string FifaCode
        {
            get => Code;
            set => Code = value;
        } 
    }
}
