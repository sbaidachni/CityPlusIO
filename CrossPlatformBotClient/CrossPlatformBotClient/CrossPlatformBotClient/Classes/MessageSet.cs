using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossPlatformBotClient.Classes
{
    public class MessageSet
    {
        public BotMessage[] messages { get; set; }
        public string watermark { get; set; }
        public string eTag { get; set; }
    }
}
