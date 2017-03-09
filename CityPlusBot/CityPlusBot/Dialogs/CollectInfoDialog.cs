using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Location;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace CityPlusBot
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
                // We have a location! 
                // Confirm that this is where they currently are!
            }
        }

        private async Task GetLocation(IDialogContext context)
        {

            var apiKey = WebConfigurationManager.AppSettings["BingMapsApiKey"];
            var prompt = "Where are you? Type or say an address or cross streets so we can find resources nearby.";
            // TODO: Override 
            var locationDialog = new LocationDialog(apiKey, "", prompt, LocationOptions.None, LocationRequiredFields.StreetAddress | LocationRequiredFields.PostalCode);
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

            var entity = new Entity();
            entity.SetAs(new Place()
            {
                Geo = place.GetGeoCoordinates()
            });

            context.UserData.SetValue(_currentLocationStr,entity);
            //entities.Add(entity);


            context.Done<string>(null);
        }

        public async Task OnGetLocation(IDialogContext context)
        {

        }

        public async Task GetDescription(IDialogContext context)
        {

        }

        public async Task GetResources(IDialogContext context)
        {

        }
    }
}