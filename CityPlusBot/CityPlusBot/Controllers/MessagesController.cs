using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace CityPlusBot
{
    using Dialogs;
    using System;
    using Activity = Microsoft.Bot.Connector.Activity;

    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            StateClient stateClient = activity.GetStateClient();
            var data=await stateClient.BotState.GetPrivateConversationDataAsync(activity.ChannelId, activity.Conversation.Id,activity.From.Id);
            int sessionId = data.GetProperty<int>("SessionId");
            if (sessionId==0)
            {
                sessionId = DataAnalyticProject.DataAnalyticAPI.AddSession(activity.ChannelId);
                data.SetProperty<int>("SessionId",sessionId);
                await stateClient.BotState.SetPrivateConversationDataAsync(activity.ChannelId, activity.Conversation.Id, activity.From.Id,data);
            }

            if (activity != null && activity.GetActivityType() == ActivityTypes.Message)
            {
                if (activity.Text!=null)
                {
                    await Conversation.SendAsync(activity, () => new LUISDialog(activity.Text,sessionId));
                }
                
            }
            else
            {
                HandleSystemMessage(activity);
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        private Activity HandleSystemMessage(Microsoft.Bot.Connector.Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                DataAnalyticProject.DataAnalyticAPI.RemoveUser(message.From.Id, message.From.Name);
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                switch(message.Action)
                {
                    case "add":
                        DataAnalyticProject.DataAnalyticAPI.AddUser(message.From.Id, message.From.Name, message.ChannelId);
                        break;
                    case "remove":
                        DataAnalyticProject.DataAnalyticAPI.RemoveUser(message.From.Id, message.From.Name);
                        break;
                }
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}