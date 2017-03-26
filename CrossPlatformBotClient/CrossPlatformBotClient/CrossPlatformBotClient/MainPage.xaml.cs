using CrossPlatformBotClient.Code;
using CrossPlatformBotClient.UserControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        Conversation conv;

        string clientId= "sbaydach@microsoft.com";

        public MainPage()
        {
            InitializeComponent();

            bot = new DirectLine(DirectLineKey);
            bot.OnNewMessage += Bot_OnNewMessage;
        }

        protected async override void OnAppearing()
        {
            conv=await bot.StartConversationAsync();

            base.OnAppearing();
        }

        private void Bot_OnNewMessage(ActivitySet args)
        {
            foreach(var activity in args.activities)
            {
                if (!activity.from.id.Equals(clientId))
                {
                    if (activity.text.Length > 0)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            stack.Children.Insert(0,(new BotTextMessage() { TextMessage = activity.text }));
                            
                        });
                    }
                    if (activity.attachments.Count>0)
                    {
                        foreach(var att in activity.attachments)
                        {
                           if (att.contentType.Equals("application/vnd.microsoft.card.hero"))
                            {
                                var card=((JObject)att.content).ToObject<HeroCard>();
                                if ((card.buttons!=null)&&(card.buttons.Count > 0))
                                {
                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        var message = new ConfirmationMessage() { TextMessage = card.text, Buttons = card.buttons };
                                        message.ButtonClicked += Message_ButtonClicked;
                                        stack.Children.Insert(0,(message));
                                    }
                                    );
                                }
                                else
                                {
                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        var message = new MapMessage() { TitleMessage=card.title, SubtitleMessage=card.subtitle, TextMessage=card.text, ImageUrl=card.images[0].url };
                                        stack.Children.Insert(0,message);
                                    }
);
                                }
                            }
                        }
                    }
                }
            }
        }
        private async void Message_ButtonClicked(string message)
        {
            var activity = Activity.CreateTextMessageActivity();
            activity.from = new ChannelAccount() { id = clientId, name = "Sergii" };
            activity.text = message;

            stack.Children.Insert(0,new UserTextMessage() { TextMessage = message });

            await bot.SendMessageAsync(conv.conversationId, activity);
        }

        private async void button_Clicked(object sender, EventArgs e)
        {
            var activity = Activity.CreateTextMessageActivity();
            activity.from = new ChannelAccount() { id = clientId, name = "Sergii" };
            activity.text = entry.Text;

            stack.Children.Add(new UserTextMessage() { TextMessage =entry.Text});

            entry.Text = "";

            await bot.SendMessageAsync(conv.conversationId, activity);
        }
    }
}
