namespace CityPlusBot.Dialogs
{
    using King.Mapper.Data;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Location;
    using Microsoft.Bot.Connector;
    using System;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using CityPlusBot.Models;
    using System.Web.Configuration;
    using System.Configuration;
    using King.Mapper;

    [Serializable]
    public class CollectInfoDialog : IDialog<object>
    {
        private static readonly Config config = ConfigurationManager.AppSettings.Map<Config>();
        private const string _checkInTimeStr = "LastCheckInTime";
        private const string _currentLocationStr = "CurrentLocation";
        //private const string _resourceStr = "Resources";
        private bool _locationConfirmed = false;
        //private bool _formConfirmed = false;

        // User Data
        //  - Last Check in time
        //  - User Identified Information (For sending notifications) (?)
        //  - Channel Id (For sending notifications) (?)
        //  - Location
        //  - List of Resources
        //      - Medical help
        //      - Food/Water
        //      - Clothing
        //      - Shelter


        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(Introduction);
        }
        public async Task Introduction(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            await context.PostAsync("I'm the CityPlus bot. I'm here to help you find the nearby resources you need.");
            //context.Wait(Introduction);
            await GetInformation(context);
        }

        public async Task GetInformation(IDialogContext context)
        {
            // First we always need the address!
            Place location = null;
            //string[] resources = null;
            if (!context.UserData.TryGetValue<Place>(_currentLocationStr, out location))
                GetLocation(context).Wait();
            else if (!_locationConfirmed)
            {
                var locationName = location.Name ?? location.GetPostalAddress()?.FormattedAddress;
                PromptDialog.Confirm(
                    context,
                    this.OnLocationCheck,
                    $"Are you still at '{locationName}'?",
                    $"Sorry didn't get that! Are you still at '{locationName}' ? ",
                    promptStyle: PromptStyle.Auto);
            }/*
            else if (!context.UserData.TryGetValue<string[]>(_resourceStr, out resources))
                GetResources(context).Wait();
            else if (!_formConfirmed)
            {
                var r = String.Join(",", resources);
                PromptDialog.Confirm(
                    context,
                    this.OnFormCheck,
                    // TO DO: fill in
                    $"Are looking for {r}?",
                    $"Sorry didn't get that! Are you still looking for {r}? ",
                    promptStyle: PromptStyle.Auto);
            }*/
            else
            {
                context.UserData.SetValue<DateTimeOffset>(_checkInTimeStr, DateTimeOffset.Now);
                // All the relevant information has been collected!
                

                var insert = $"INSERT INTO Person ([Location]) VALUES (geography::STPointFromText('{location.Geo.Latitude}', '{location.Geo.Longitude}', 4326))";
                var select = $"SELECT [Name], [Location], [Food], [Shelter], [Clothes], [Medicine], [Id] FROM Resource WHERE Location.geography::Point({location.Geo.Latitude}, {location.Geo.Longitude}, 4326) <= 100";

                using (var connection = new SqlConnection(config.ConnectionString))
                {
                    // Save the user information
                    var executor = new Executor(connection);
                    await executor.NonQuery(insert);
                    
                    // Query the database
                    var reader = await executor.Query(select);

                    var users = reader.Models<Resource>();

                }
                // Return the results!

                // Check if they want to subscribe for notifications...
                await context.PostAsync("We have all the relevant information and will notify you know when resources near you become available.");
            }
        }

        private async Task OnLocationCheck(IDialogContext context, IAwaitable<bool> result)
        {
            var accepted = (await result);
            if (accepted)
            {
                // This is the correct address move on to checking the form!
                _locationConfirmed = true;
            }
            else
            {
                // The location is wrong!
                _locationConfirmed = false;
                // Clear out the information and start again!
                context.UserData.RemoveValue(_currentLocationStr);
            }
            await GetInformation(context);
        }
        /*
        private async Task OnFormCheck(IDialogContext context, IAwaitable<bool> result)
        {
            var accepted = (await result);
            if (accepted)
            {
                // This is the form is good to go!
                _formConfirmed = true;
            }
            else
            {
                // The form is wrong!
                _formConfirmed = false;
                // Clear out the information and start again!
                context.UserData.RemoveValue(_resourceStr);
            }
            await GetInformation(context);
        }
        */
        private async Task GetLocation(IDialogContext context)
        {
            var apiKey = WebConfigurationManager.AppSettings["BingMapsApiKey"];
            var prompt = "Where are you? We need you location to find resources nearby.";
            var locationDialog = new LocationDialog(apiKey, "", prompt, LocationOptions.UseNativeControl, LocationRequiredFields.None);
            context.Call(locationDialog, this.ResumeAfterLocationDialogAsync);
        }

/*
        private async Task GetResources(IDialogContext context)
        {
            // Call the form Dialog!
        }
        */
        private async Task ResumeAfterLocationDialogAsync(IDialogContext context, IAwaitable<Place> result)
        {
            var place = await result;
            if (place != null && place.Geo != null)
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

                await context.PostAsync("I have set your location as : " + formattedAddress);


                var geoLocationEntity = new Entity();
                geoLocationEntity.SetAs(place);
                _locationConfirmed = true;
                context.UserData.SetValue(_currentLocationStr, geoLocationEntity);
            }
            await GetInformation(context);
        }
    }
}