using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Location;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

[Serializable]
public class CollectInfoDialog : IDialog<object>
{
    private bool _haveLocation = false;
    private IList<Entity> _entities = new List<Entity>();
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
        await GetLocation(context);
    }

    public async Task GetLocation(IDialogContext context)
    {
        if (_haveLocation == false)
        {
            PromptDialog.Confirm(
                               context,
                               this.OnGetLocation,
                               "Could we have your location so we can find you resources in your area?",
                               "Sorry I don't understand!",
                               promptStyle: PromptStyle.Auto);
        }
        else
        {
            await GetResources(context);
        }

    }

    private async Task OnGetLocation(IDialogContext context, IAwaitable<bool> result)
    {
        var accepted = (await result);
        if (accepted)
        {
            /*
            var entity = new Entity();
            entity.SetAs(new Place()
            {
                Geo = new GeoCoordinates()
                {
                    Latitude = 32.4141,
                    Longitude = 43.1123123,
                }
            });
            _entities.Add(entity);
            /// Find location!
            /// 
            */
            await context.PostAsync("Ok let's look that up!");
            var apiKey = WebConfigurationManager.AppSettings["BingMapsApiKey"];
            var prompt = "Where are you? Type or say an address.";
            var locationDialog = new LocationDialog(apiKey, context.Activity.ChannelId, prompt, LocationOptions.None, LocationRequiredFields.StreetAddress | LocationRequiredFields.PostalCode);
            context.Call(locationDialog, this.ResumeAfterLocationDialogAsync);
        }
        else
        {
            await context.PostAsync("I don't think we can help you without a location!");
        }

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

            await context.PostAsync("Thanks, I will ship it to " + formattedAddress);
        }

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