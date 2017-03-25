using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossPlatformBotClient.Code
{
    public class Attachment
    {
        public string contentUrl { get; set; }

        public string contentType { get; set; }

        public object content { get; set; }

        public string name { get; set; }

        public string thumbnailUrl { get; set; }
    }
}
