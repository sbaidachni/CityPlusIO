namespace CityPlusBot.Dialogs
{
    using CityPlusBot.Models;
    using King.Mapper;
    using King.Mapper.Data;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Location;
    using Microsoft.Bot.Connector;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Configuration;

    [Serializable]
    public class CollectInfoDialog : IDialog<object>
    {
        #region Members
        //private static readonly Config config = ConfigurationManager.AppSettings.Map<Config>();
        private const string _checkInTimeStr = "LastCheckInTime";
        private const string _currentLocationStr = "CurrentLocation";
        //private const string _resourceStr = "Resources";
        private bool _locationConfirmed = false;
        //private bool _formConfirmed = false;
        #endregion

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

               // var geo = location.GetGeoCoordinates();


                var insert = $"INSERT INTO Person ([Location]) VALUES (geography::STPointFromText('POINT({location.Geo.longitude} {location.Geo.latitude})', 4326))";
                var select = $"SELECT [Name], [Location], [Food], [Shelter], [Clothes], [Medicine], [Id] FROM Resources WHERE geography::STGeomFromText('POINT({location.Geo.longitude} {location.Geo.latitude})', 4326).STDistance(latlong) <= 10000";
                IList<Resource> resources = null;
                using (var connection = new SqlConnection(WebConfigurationManager.AppSettings["ConnectionString"]))
                {
                    // Save the user information
                    var executor = new Executor(connection);
                    await executor.NonQuery(insert);

                    // Query the database
                    var reader = await executor.Query(select);
                    resources = reader.Models<Resource>().ToList();
                }

                // Return the results!
                if (resources != null && resources.Count > 0)
                {
                    var locationsCardReply = context.MakeMessage();
                    locationsCardReply.Attachments = await CreateResourceCards("", resources);
                    locationsCardReply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    await context.PostAsync(locationsCardReply);

                    // Return the results!

                    // Check if they want to subscribe for notifications...
                    await context.PostAsync("We have all the relevant information and will notify you know when resources near you become available.");
                }
                else
                {
                    // TODO: Set up notifications
                    await context.PostAsync("Sorry there are no available resources near you at this time. We have all the relevant information and will notify you know when resources near you become available.");
                }
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
            var locationDialog = new LocationDialog(apiKey, string.Empty, prompt, LocationOptions.UseNativeControl, LocationRequiredFields.None);
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

        public async Task<IList<Attachment>> CreateResourceCards(string apiKey, IList<Resource> resources)
        {
            var attachments = new List<Attachment>();
            foreach (var resource in resources)
            {
                var heroCard = new HeroCard
                {
                    Title = resource.Name,
                    Subtitle = resource.Location,
                    Text = $"Resources Available : Medicine {resource.Medicine}, Shelter {resource.Shelter}, Food {resource.Food}, Clothes {resource.Clothes}",

                };


                if (resource.latitude != 0 && resource.longitude != 0)
                {
                    var helper = new BingHelper();
                    var locations = await helper.GetLocationsByPointAsync(apiKey, Convert.ToDouble(resource.latitude), Convert.ToDouble(resource.longitude));
                    var location = locations.Locations.FirstOrDefault();

                    if (location != null)
                    {
                        var image =
                            new CardImage(helper.GetLocationMapImageUrl(apiKey, location));


                        // https://www.bing.com/maps?osid=384772a7-d16a-4176-9be8-46fe10113ce8&cp=49.275533~-123.156743&lvl=14&v=2&sV=2&form=S00027
                        // Open directions from current location to here....
                        /*
                        var action = new CardAction()
                        {
                            Type = "postBack",
                            Title = "Open in Maps",
                            Value = "other"
                        };*/

                        heroCard.Images = new[] { image };
                    }


                }
            }


            return attachments;
        }


    }
}