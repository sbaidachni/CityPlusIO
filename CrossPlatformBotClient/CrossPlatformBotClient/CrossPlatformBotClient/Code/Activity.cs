using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossPlatformBotClient.Code
{
    public class Activity
    {
        public string type { get; set; }

        public string text { get; set; }

        public ChannelAccount from { get; set; }

        public string id { get; set; }

        public string timestamp { get; set; }

        public string localTimestamp { get; set; }

        public string serviceUrl { get; set; }

        public string channelId { get; set; }

        public ConversationAccount conversation { get; set; }

        public ChannelAccount recipient { get; set; }

        public string textFormat { get; set; }

        public string attachmentLayout { get; set; }

        public List<ChannelAccount> membersAdded { get; set; }

        public List<ChannelAccount> membersRemoved { get; set; }

        public string topicName { get; set; }

        public bool historyDisclosed { get; set; }

        public string locale { get; set; }

        public string summary { get; set; }

        public List<Attachment> attachments { get; set; }

        public List<Entity> entities { get; set; }

        public string action { get; set; }

        public string replyToId { get; set; }

        public object value { get; set; }

        public object channelData { get; set; }

        private Activity()
        { }

        public static Activity CreateTextMessageActivity()
        {
            return new Activity() { type = "message" };
        }
    }
}



