using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossPlatformBotClient.Code
{
    public class HeroCard
    {
        [JsonProperty(PropertyName ="text")]
        public string text { get; set; }

        public string title { get; set; }

        public string subtitle { get; set; }

        public List<HeroImage> images { get; set; }

        public List<HeroButton> buttons { get; set; }
    }
}
