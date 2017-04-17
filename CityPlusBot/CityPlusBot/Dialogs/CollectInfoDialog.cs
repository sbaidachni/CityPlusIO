namespace CityPlusBot.Dialogs
{
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
    using Newtonsoft.Json.Linq;

    [Serializable]
    public class CollectInfoDialog : IDialog<object>
    {
        private const string _checkInTimeStr = "LastCheckInTime";
        private const string _currentLocationStr = "CurrentLocation";
        private bool _locationConfirmed = false;
        private int sessionId = 0;

        public CollectInfoDialog(int sessionId)
        {
            this.sessionId = sessionId;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await GetInformation(context);
        }
 

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        public static double CalculateDistance(double? lon1, double? lat1, double? lon2, double? lat2)
        {
            double circumference = 40000.0; // Earth's circumference at the equator in km
            double distance = 0.0;

            //Calculate radians
            double latitude1Rad = DegreesToRadians(lat1.Value);
            double longitude1Rad = DegreesToRadians(lon1.Value);
            double latititude2Rad = DegreesToRadians(lat2.Value);
            double longitude2Rad = DegreesToRadians(lon2.Value);

            double logitudeDiff = Math.Abs(longitude1Rad - longitude2Rad);

            if (logitudeDiff > Math.PI)
            {
                logitudeDiff = 2.0 * Math.PI - logitudeDiff;
            }

            double angleCalculation =
                Math.Acos(
                  Math.Sin(latititude2Rad) * Math.Sin(latitude1Rad) +
                  Math.Cos(latititude2Rad) * Math.Cos(latitude1Rad) * Math.Cos(logitudeDiff));

            distance = circumference * angleCalculation / (2.0 * Math.PI);

            return distance;
        }

        public async Task GetInformation(IDialogContext context)
        {
            Place location = null;
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
            }
            else 
            {
                context.UserData.SetValue<DateTimeOffset>(_checkInTimeStr, DateTimeOffset.Now);

                var jObj = (JObject)location.Geo;
                double lat = Convert.ToDouble(jObj.First.First);
                double lon = Convert.ToDouble(jObj.First.Next.First);

                var resources = (from a in DataAnalyticProject.DataAnalyticAPI.db.Resources
                                 select a);

                List<DataAnalyticProject.Resource> oResources = new List<DataAnalyticProject.Resource>();

                foreach(var r in resources)
                {
                    var dist=CalculateDistance(lon, lat, r.Lon, r.Lat);
                    if (dist < 10) oResources.Add(r);
                }

                var outputResources = (from a in oResources select a).Take(3).ToList();

                if (outputResources != null && outputResources.Count() > 0)
                {
                    DataAnalyticProject.DataAnalyticAPI.AddResources(sessionId, outputResources);

                    var attachments = await CreateResourceCards("", outputResources.ToList());
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
                        await context.PostAsync("Sorry there are no available resources near you at this time. We have all the relevant information and will notify you know when resources near you become available.");
                    }
                }
                else
                {
                    await context.PostAsync("Sorry there are no available resources near you at this time. We have all the relevant information and will notify you know when resources near you become available.");
                }
                context.Done<bool>(true);
            }
        }
            

        private async Task OnLocationCheck(IDialogContext context, IAwaitable<bool> result)
        {
            var accepted = (await result);
            if (accepted)
            {
                DataAnalyticProject.DataAnalyticAPI.AddConversation(sessionId, "location confirmed");
                _locationConfirmed = true;
            }
            else
            {
                DataAnalyticProject.DataAnalyticAPI.AddConversation(sessionId, "location is not confirmed");
                _locationConfirmed = false;
                context.UserData.RemoveValue(_currentLocationStr);
            }
            await GetInformation(context);
        }

        private async Task GetLocation(IDialogContext context)
        {
            var apiKey = WebConfigurationManager.AppSettings["BingMapsApiKey"];

            var prompt = "Where are you? We need you location to find resources nearby.";
            var locationDialog = new LocationDialog(apiKey, string.Empty, prompt, LocationOptions.UseNativeControl, LocationRequiredFields.None);
            context.Call(locationDialog, this.ResumeAfterLocationDialogAsync);
        }

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
            apiKey = WebConfigurationManager.AppSettings["BingMapsApiKey"];
            var attachments = new List<Attachment>();
            foreach (var resource in resources)
            {
                var heroCard = new HeroCard
                {
                    Title = resource.Name,
                    Subtitle = resource.Address,
                    Text = $"Resources Available : Medicine {resource.Medicine}, Shelter {resource.Shelter}, Food {resource.Food}, Clothes {resource.Clothes}"
                };

                attachments.Add(heroCard.ToAttachment());

                if (resource.Address!=null)
                {
                    var helper = new BingHelper();
                    double lat=Convert.ToDouble(resource.Lat);
                    double lon= Convert.ToDouble(resource.Lon);
                    var locations = await helper.GetLocationsByPointAsync(apiKey, lat, lon);
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
                }
            }
            return attachments;
        }
    }
}