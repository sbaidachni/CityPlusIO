using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CrossPlatformBotClient.Code
{
    public class DirectLine
    {
        private string DirectLineKey;
        Websockets.IWebSocketConnection connection;

        public delegate void NewMessageDelegate(ActivitySet args);

        public event NewMessageDelegate OnNewMessage;

        public DirectLine(string key)
        {
            DirectLineKey = key;

            connection = Websockets.WebSocketFactory.Create();
            connection.OnOpened += Connection_OnOpened;
            connection.OnMessage += Connection_OnMessage;
        }

        private void Connection_OnMessage(string obj)
        {
            var result=JsonConvert.DeserializeObject<ActivitySet>(obj);
            if (OnNewMessage != null) OnNewMessage(result);
        }

        private void Connection_OnOpened()
        {
            Debug.WriteLine("WebSocket is opened");
        }

        public async Task<Conversation> StartConversationAsync()
        {
            var _client = new System.Net.Http.HttpClient();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                DirectLineKey);
            var response = await _client.PostAsync("https://directline.botframework.com/v3/directline/conversations", new StringContent(String.Empty));
            if (response.IsSuccessStatusCode)
            {
                string content=await response.Content.ReadAsStringAsync();
                var conversation=JsonConvert.DeserializeObject<Conversation>(content);

                connection.Open(conversation.streamUrl);

                return conversation;
            }
            return null;
        }

        public async Task SendMessageAsync(string conversationId, Activity activity)
        {
            var _client = new System.Net.Http.HttpClient();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                DirectLineKey);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string jsonText = JsonConvert.SerializeObject(activity);

            var response = await _client.PostAsync($"https://directline.botframework.com/v3/directline/conversations/{conversationId}/activities", 
                new StringContent(jsonText,Encoding.UTF8, "application/json"));
        }
    }
}
