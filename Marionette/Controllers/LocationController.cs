using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using Marionette.Driver;

namespace Marionette
{
    public class LocationController : ApiController
    {
        public const string Prefix = "Location";
        private string ApiRoute { get; set; }

        public LocationController()
        {
            ApiRoute = Startup.DefaultApiRouteName;
        }

        private string BuildLink( object routeValues )
        {
            return Url.Link( ApiRoute, routeValues );
        }

        /// <summary>
        /// Retrieves the location of the host.
        /// </summary>
        [ResponseType( typeof( LocationResource ) )]
        public IHttpActionResult Get()
        {
            return Ok( ConvertToResource( LocationProvider.Location ) );
        }

        private LocationResource ConvertToResource( LocationModel model )
        {
            LocationResource resource = Mapper.Map<LocationResource>( model );
            resource.Href = BuildLink( new { controller = Prefix } );
            return resource;
        }
    }
}
