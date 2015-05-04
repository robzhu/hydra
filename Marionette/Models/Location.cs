using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Marionette
{
    public class LocationModel
    {
        public int Id { get; set; }
        public string AzureRegion { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public static class Locations
    {
        public static Dictionary<int, LocationModel> Data = new Dictionary<int, LocationModel>();

        static Locations()
        {
            foreach( var location in DefineLocations() )
            {
                Data.Add( location.Id, location );
            }
        }

        private static IEnumerable<LocationModel> DefineLocations()
        {
            yield return new LocationModel
            {
                Id = 0,
                AzureRegion = "None (Eze Boston)",
                Name = "Massachusetts",
                Latitude = 42.351,
                Longitude = -71.049,
            };

            yield return new LocationModel
            {
                Id = 1,
                AzureRegion = "East US",
                Name = "Virginia",
                Latitude = 37.431,
                Longitude = -78.657,
            };

            yield return new LocationModel
            {
                Id = 2,
                AzureRegion = "Central US",
                Name = "Iowa",
                Latitude = 41.878,
                Longitude = -93.098,
            };

            yield return new LocationModel
            {
                Id = 3,
                AzureRegion = "North Central US",
                Name = "Illinois",
                Latitude = 40.633,
                Longitude = -89.399,
            };

            yield return new LocationModel
            {
                Id = 4,
                AzureRegion = "South Central US",
                Name = "Texas",
                Latitude = 31.969,
                Longitude = -99.902,
            };

            yield return new LocationModel
            {
                Id = 5,
                AzureRegion = "West US",
                Name = "California",
                Latitude = 36.778,
                Longitude = -119.418,
            };

            yield return new LocationModel
            {
                Id = 6,
                AzureRegion = "West Europe",
                Name = "Netherlands",
                Latitude = 52.133,
                Longitude = 5.291,
            };

            yield return new LocationModel
            {
                Id = 7,
                AzureRegion = "East Asia",
                Name = "Hong Kong",
                Latitude = 22.396,
                Longitude = 114.109,
            };

            yield return new LocationModel
            {
                Id = 8,
                AzureRegion = "Southeast Asia",
                Name = "Singapore",
                Latitude = 1.352,
                Longitude = 103.820,
            };

            yield return new LocationModel
            {
                Id = 9,
                AzureRegion = "Japan East",
                Name = "Saitama Prefecture",
                Latitude = 35.857,
                Longitude = 139.649,
            };

            yield return new LocationModel
            {
                Id = 10,
                AzureRegion = "Japan West",
                Name = "Osaka Prefecture",
                Latitude = 34.686,
                Longitude = 135.520,
            };

            yield return new LocationModel
            {
                Id = 11,
                AzureRegion = "Brazil South",
                Name = "Sao Paulo State",
                Latitude = -23.543,
                Longitude = -46.629,
            };

            yield return new LocationModel
            {
                Id = 12,
                AzureRegion = "Australia East",
                Name = "New South Wales",
                Latitude = -33.864,
                Longitude = 151.205,
            };

            yield return new LocationModel
            {
                Id = 13,
                AzureRegion = "Australia Southeast",
                Name = "Victoria",
                Latitude = -37.471,
                Longitude = 144.785,
            };
        }
    }
}
