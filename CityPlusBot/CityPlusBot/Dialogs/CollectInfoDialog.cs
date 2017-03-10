namespace CityPlusBot.Dialogs
{
    using Models;
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
    using System.Data.Spatial;

    [Serializable]
    public class CollectInfoDialog : IDialog<object>
    {
        #region Members
        //private static readonly Config config = ConfigurationManager.AppSettings.Map<Config>();
        private const string _checkInTimeStr = "LastCheckInTime";
        private const string _currentLocationStr = "CurrentLocation";
        //private const string _resourceStr = "Resources";
        private bool _locationConfirmed = false;
        private int sessionId = 0;
        private User _userProfile = null;
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
            sessionId= DataAnalyticProject.DataAnalyticAPI.AddSession(context.Activity.ChannelId);
            context.Wait(Introduction);
        }
        public async Task Introduction(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            DataAnalyticProject.DataAnalyticAPI.AddConversation(sessionId, message.Text);
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

                var geo = location.GetGeoCoordinates();

                
               
                /*var insert = $"INSERT INTO Person ([Location]) VALUES (geography::STPointFromText('POINT({location.Geo.longitude} {location.Geo.latitude})', 4326))";

                using (var connection = new SqlConnection(WebConfigurationManager.AppSettings["ConnectionString"]))
                {
                    // Save the user information
                    var executor = new Executor(connection);
                    await executor.NonQuery(insert);

                }*/

                //var point = DbGeography.FromGml($"'POINT({location.Geo.longitude} {location.Geo.latitude})'");
                var resources = (from a in DataAnalyticProject.DataAnalyticAPI.db.Resources
                                //where a.Location.Distance(point)<=1000
                                //where DbGeography::STGeomFromText('POINT({location.Geo.longitude} {location.Geo.latitude})', 4326).STDistance(latlong) <= 10000
                                select a).Take(3);


                // Return the results!
                if (resources != null && resources.Count() > 0)
                {

                        DataAnalyticProject.DataAnalyticAPI.AddResources(sessionId,  resources);

                    var attachments = await CreateResourceCards("", resources.ToList());
                    if (attachments.Count() > 0)
                    {
                        var locationsCardReply = context.MakeMessage();
                        locationsCardReply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        locationsCardReply.Attachments=attachments;
                        await context.PostAsync(locationsCardReply);

                        await context.PostAsync("We  will notify you know when new resources near you become available.");
                    }
                    else
                    {
                        // TODO: Set up notifications
                        await context.PostAsync("Sorry there are no available resources near you at this time. We have all the relevant information and will notify you know when resources near you become available.");

                    }
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
                DataAnalyticProject.DataAnalyticAPI.AddConversation(sessionId, "location confirmed");
                // This is the correct address move on to checking the form!
                _locationConfirmed = true;
            }
            else
            {
                DataAnalyticProject.DataAnalyticAPI.AddConversation(sessionId, "location is not confirmed");
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
                DataAnalyticProject.DataAnalyticAPI.AddConversation(sessionId, $"location is provided:{formattedAddress}");

                var geoLocationEntity = new Entity();
                geoLocationEntity.SetAs(place);
                _locationConfirmed = true;
                context.UserData.SetValue(_currentLocationStr, geoLocationEntity);
            }

            await GetInformation(context);
        }

        public async Task<IList<Attachment>> CreateResourceCards(string apiKey, IList<DataAnalyticProject.Resource> resources)
        {
            var attachments = new List<Attachment>();
            foreach (var resource in resources)
            {
                var heroCard = new HeroCard
                {
                    Title = resource.Name,
                    Subtitle = resource.Address,
                    Text = $"Resources Available : Medicine {resource.Medicine}, Shelter {resource.Shelter}, Food {resource.Food}, Clothes {resource.Clothes}",

                    };

                attachments.Add(heroCard.ToAttachment());


                /*if (resource.Address!=null)
                {
                    var helper = new BingHelper();
                    var locations = await helper.GetLocationsByPointAsync(apiKey, Convert.ToDouble(0), Convert.ToDouble(0));
                    var location = locations.Locations.FirstOrDefault();

                    if (location != null)
                    {
                        var image = new CardImage(helper.GetLocationMapImageUrl(apiKey, location));

                        var action = new CardAction()
                        {
                            Value = $"https://www.bing.com/maps?cp={lat}~{lon}&lvl=14&v=2&sV=2&form=S00027",
                            Type = "openUrl",
                            Title = "Open in maps"
                        };

                        heroCard.Images = new[] { image };
                    }
                    
                }*/
            }


            return attachments;
        }


    }
}