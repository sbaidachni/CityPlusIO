using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Location;
using Microsoft.Bot.Connector;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace CityPlusBot.Dialogs
{
    [Serializable]
    public class CollectInfoDialog : IDialog<object>
    {
        const string _currentLocationStr = "CurrentLocation";
        //public List<Entity> _entities = new List<Entity>();
        // Location
        // User Identified Information (For sending notifications)
        // Description (?)
        // Resources they need
        // - Medical help
        // - Food/Water
        // - Clothing
        // - Shelter

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(Introduction);
        }
        public async Task Introduction(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            await context.PostAsync("Hi, I'm the CityPlus bot. I'm here to help find you the resources you need. Please provide me more information so I can better help you.");
            //context.Wait(Introduction);
            await GetInformation(context);
        }

        public async Task GetInformation(IDialogContext context)
        {
            Entity entity = null;  
            if (!context.UserData.TryGetValue<Entity>(_currentLocationStr, out entity))
                GetLocation(context).Wait();
            else
            {
                var address = entity;
                /*PromptDialog.Confirm(
                    context,
                    this.OnSpellCheckIntent,
                    $"Did you mean '{this.newMessage}'?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.Auto);*/
            }

            // Go to Form!


        }
/*
        private async Task OnSpellCheckIntent(IDialogContext context, IAwaitable<bool> result)
        {
            var accepted = (await result);
            if (accepted)
            {
                var uri = this.services[0].BuildUri(new LuisRequest(this.newMessage));
                var newResult = await this.services[0].QueryAsync(uri, new CancellationToken());
                switch (newResult.Intents[0].Intent.ToLower())
                {
                    case "none":
                        await this.None(context, newResult);
                        break;
                    case "hello":
                        await this.Hello(context, newResult);
                        break;
                    case "viasport.intent.howtocoach":
                        await this.HowToCoach(context, newResult);
                        break;
                    case "viasport.intent.findresource":
                        await this.FindResource(context, newResult);
                        break;
                    case "viasport.intent.findprogram":
                        await this.FindProgram(context, newResult);
                        break;
                }
            }
            else
            {
                var message = "Ok, can you tell me what you meant?";
                await context.PostAsync(BotDbStrings.MakeItAcceptable(message));
                context.Wait(this.MessageReceived);
            }
        }*/
        private async Task GetLocation(IDialogContext context)
        {
            var apiKey = WebConfigurationManager.AppSettings["BingMapsApiKey"];
            var prompt = "Where are you? Type or say an address or cross streets so we can find resources nearby.";
            // TODO: Override 
            var locationDialog = new LocationDialog(apiKey, "", prompt, LocationOptions.UseNativeControl, LocationRequiredFields.None);
           context.Call(locationDialog, this.ResumeAfterLocationDialogAsync);
        }

        private async Task ResumeAfterLocationDialogAsync(IDialogContext context, IAwaitable<Place> result)
        {
            var place = await result;
            if (place != null)
            {
                var address = place.GetPostalAddress();
                var formattedAddress = string.Join(", ", new[]
                {
                        address.StreetAddress,
                        address.Locality,
                        address.Region,
                        address.PostalCode,
                        address.Country
                    }.Where(x => !string.IsNullOrEmpty(x)));

                await context.PostAsync("Thanks! I will find resources near " + formattedAddress);
            }

            var geoLocationEntity = new Entity();
            geoLocationEntity.SetAs(new Place(place));

            context.UserData.SetValue(_currentLocationStr,geoLocationEntity);
            


            context.Done<string>(null);
        }
    }
}