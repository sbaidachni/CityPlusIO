using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossPlatformBotClient.Code
{
    public class Conversation
    {
        public string conversationId { get; set; }

        public string token { get; set; }

        public string streamUrl { get; set; }

        public string eTag { get; set; }

        public int expires_in { get; set; }
    }
}
