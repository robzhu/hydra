using Rz.Http;

namespace Marionette.Driver
{
    public class LocationResource : Resource
    {
        public string AzureRegion { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
