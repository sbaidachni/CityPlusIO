﻿namespace Microsoft.Bot.Builder.Location
{
    using System.Collections.Generic;
    using System.Linq;
    using Bing;
    using Connector;
    using ConnectorEx;

    /// <summary>
    /// A static class for creating location cards.
    /// </summary>
    public static class LocationCard
    {
        /// <summary>
        /// Creates locations hero cards (carousel).
        /// </summary>
        /// <param name="apiKey">The geo spatial API key.</param>
        /// <param name="locations">List of the locations.</param>
        /// <returns>The locations card as attachments.</returns>
        public static List<Attachment> CreateLocationHeroCard(string apiKey, IList<Location> locations)
        {
            var attachments = new List<Attachment>();

            int i = 1;
            if (locations.Count == 1)
            {
                var location = locations.FirstOrDefault();
                string address = location.Address.FormattedAddress;

                var heroCard = new HeroCard
                {
                    Subtitle = address
                };

                if (location.Point != null)
                {
                    var image =
                        new CardImage(
                            url: new BingGeoSpatialService().GetLocationMapImageUrl(apiKey, location, i));

                    heroCard.Images = new[] { image };
                }
                attachments.Add(heroCard.ToAttachment());
                return attachments;
            }
            if (locations.Count > 1)
            { 
                // Include the search again option!
                var heroCard = new HeroCard
                {
                    Subtitle = "Don't see your location?"
                };

                var image =
                        new CardImage("https://upload.wikimedia.org/wikipedia/commons/c/cf/A_large_blank_world_map_with_oceans_marked_in_blue.PNG");

                heroCard.Images = new[] { image };

                var action = new CardAction()
                {
                    Type = "postBack",
                    Title = "Search again",
                    Value = "other"
                };

                heroCard.Buttons = new List<CardAction>() { action };
                attachments.Add(heroCard.ToAttachment());
            }
            foreach (var location in locations)
            {
                string address = locations.Count > 1 ? $"{i}. {location.Address.FormattedAddress}" : location.Address.FormattedAddress;

                var heroCard = new HeroCard
                {
                    Subtitle = address
                };

                if (location.Point != null)
                {
                    var image =
                        new CardImage(
                            url: new BingGeoSpatialService().GetLocationMapImageUrl(apiKey, location, i));

                    heroCard.Images = new[] { image };
                }
                var action = new CardAction()
                {
                    Type = "postBack",
                    Title = "Select Location",
                    Value = i.ToString()
                };

                heroCard.Buttons = new List<CardAction>() { action };
                attachments.Add(heroCard.ToAttachment());

                i++;
            }

            return attachments;
        }

        /// <summary>
        /// Creates location keyboard cards (buttons).
        /// </summary>
        /// <param name="locations">The list of locations.</param>
        /// <param name="selectText">The card prompt.</param>
        /// <returns>The keyboard cards.</returns>
        public static List<Attachment> CreateLocationKeyboardCard(IEnumerable<Location> locations, string selectText)
        {
            int i = 1;
            var keyboardCard = new KeyboardCard(
                selectText,
                locations.Select(a => new CardAction
                {
                    Type = "imBack",
                    Title = i.ToString(),
                    Value = (i++).ToString()
                }).ToList());

            return new List<Attachment> { keyboardCard.ToAttachment() };
        }
        
    }
}
