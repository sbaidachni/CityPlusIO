using CrossPlatformBotClient.Code;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CrossPlatformBotClient
{
    public partial class MainPage : ContentPage
    {
        DirectLine bot;
        string DirectLineKey = "";

        public MainPage()
        {
            InitializeComponent();

            bot = new DirectLine(DirectLineKey);
            bot.OnNewMessage += Bot_OnNewMessage;
        }

        protected async override void OnAppearing()
        {
            var conv=await bot.StartConversationAsync();

            var activity = Activity.CreateTextMessageActivity();
            activity.from = new ChannelAccount() { id = "sbaydach@microsoft.com", name="Sergii" };
            activity.text = "test";

            await bot.SendMessageAsync(conv.conversationId, activity);

            base.OnAppearing();
        }

        private void Bot_OnNewMessage(ActivitySet args)
        {
            throw new NotImplementedException();
        }
    }
}
