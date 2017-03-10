using CrossPlatformBotClient.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        int messageCount = 0;
        private HttpClient _client;
        private Conversation _lastConversation;
        string DirectLineKey = "[Add Direct Line Key]";

        MessageSet ms = new MessageSet();


        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _client = new HttpClient();
            _client.BaseAddress = new Uri("https://directline.botframework.com/api/conversations/");
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("BotConnector",
                DirectLineKey);
            var response = await _client.GetAsync("/api/tokens/");
            if (response.IsSuccessStatusCode)
            {
                var conversation = new Conversation();
                HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(conversation), Encoding.UTF8,
                    "application/json");
                response = await _client.PostAsync("/api/conversations/", contentPost);
                if (response.IsSuccessStatusCode)
                {
                    var conversationInfo = await response.Content.ReadAsStringAsync();
                    _lastConversation = (Conversation)JsonConvert.DeserializeObject(conversationInfo);
                }

            }

        }
    }
}
