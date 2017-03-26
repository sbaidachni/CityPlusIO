using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace CrossPlatformBotClient.UserControls
{
    public partial class MapMessage : ContentView
    {
        private string textMessage;
        public string TextMessage
        {
            set
            {
                textMessage = value;
                text.Text = textMessage;
            }
        }

        private string titleMessage;
        public string TitleMessage
        {
            set
            {
                titleMessage = value;
                title.Text = titleMessage;
            }
        }

        private string subtitleMessage;
        public string SubtitleMessage
        {
            set
            {
                subtitleMessage = value;
                subtitle.Text = subtitleMessage;
            }
        }

        private string imageUrl;
        public string ImageUrl
        {
            set
            {
                imageUrl = value;
                image.Source = ImageSource.FromUri(new Uri(imageUrl,UriKind.Absolute));
            }
        }

        public MapMessage()
        {
            InitializeComponent();
        }
    }
}
