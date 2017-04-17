using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Cognitive.LUIS;

namespace CityPlusBot.Dialogs
{

    [Serializable]
    public class LUISDialog : IDialog<object>
    {
        private string initialQuery = "";
        private int SessionId = 0;
        private string appId = "aad71ae3-3077-4a30-a9b6-2f30feb291da";
        private string appKey = "ee0eb3abaff24096bec04edb6ede5c9b";

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(WaitForMessage);
            //await GenerateReply(initialQuery,context);
        }

        private async Task GenerateReply(string message,IDialogContext context)
        {
            DataAnalyticProject.DataAnalyticAPI.AddConversation(SessionId, message);
            switch (message)
            {
                case "help":
                    await context.PostAsync("Just type your question and we will try to find all needed information");
                    context.Wait(WaitForMessage);
                    break;
                case "delete":
                    DataAnalyticProject.DataAnalyticAPI.RemoveUser(context.Activity.From.Id, context.Activity.From.Name);
                    await context.PostAsync("Your account is deleted from our database");
                    context.Wait(WaitForMessage);
                    break;
                default:
                    LuisClient luis = new LuisClient(appId,appKey);
                    var result=await luis.Predict(message);
                    CollectInfoDialog collectionDialog;
                    switch (result.Intents[0].Name)
                    {
                        case "None":
                            await context.PostAsync("Sorry, I cannot understand you");
                            context.Wait(WaitForMessage);
                            break;
                        case "Hello":
                            await context.PostAsync("I'm the CityPlus bot. I'm here to help you find the nearby resources you need.");
                            context.Wait(WaitForMessage);
                            break;
                        case "findFood":
                            collectionDialog = new CollectInfoDialog(SessionId);
                            context.Call(collectionDialog, this.ResumeAfterLocationDialogAsync);
                            break;
                        case "findClothes":
                            collectionDialog = new CollectInfoDialog(SessionId);
                            context.Call(collectionDialog, this.ResumeAfterLocationDialogAsync);
                            break;
                        case "findMedicine":
                            collectionDialog = new CollectInfoDialog(SessionId);
                            context.Call(collectionDialog, this.ResumeAfterLocationDialogAsync);
                            break;
                        case "findShelter":
                            collectionDialog = new CollectInfoDialog(SessionId);
                            context.Call(collectionDialog, this.ResumeAfterLocationDialogAsync);
                            break;
                    }

                    break;
            }
        }

        private async Task ResumeAfterLocationDialogAsync(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(WaitForMessage);
        }

        public async Task WaitForMessage(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            if (message.Text!=null)
                await GenerateReply(message.Text, context);
        }

            public LUISDialog(string message, int sessionId)
        {
            this.initialQuery = message;
            this.SessionId = sessionId;
        }
    }
}