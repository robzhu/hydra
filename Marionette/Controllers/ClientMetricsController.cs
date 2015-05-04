using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using Hydra.Shared;
using Marionette.Driver;
using Rz.Http;

namespace Marionette
{
    public class ClientMetricsController : ApiController
    {
        public const string Prefix = "ClientMetrics";

        private ClientMetricsProvider Provider { get; set; }
        private string ApiRoute { get; set; }

        public ClientMetricsController()
        {
            Provider = ClientMetricsProvider.Instance;
            ApiRoute = Startup.DefaultApiRouteName;
        }

        private string BuildLink( object routeValues )
        {
            return Url.Link( ApiRoute, routeValues );
        }

        /// <summary>
        /// Retrieves the client performance metrics for a client.
        /// </summary>
        /// <param name="id">The Process ID of the client to retrieve data for</param>
        [ResponseType( typeof( ClientStatsResource ) )]
        public IHttpActionResult Get( int id )
        {
            ClientStatsResource stats = Provider.GetById( id );
            if( stats != null ) 
            {
                return Ok( ConvertToResource( stats ) );
            }
            return NotFound();
        }

        /// <summary>
        /// Retrieves all the client performance metrics for this host.
        /// </summary>
        [ResponseType( typeof( ResourceCollection<ClientStatsResource> ) )]
        public IHttpActionResult Get()
        {
            List<ClientStatsResource> resources = new List<ClientStatsResource>();

            resources.AddRange( Provider.GetAll().Select( stat => ConvertToResource( stat ) ) );

            var resourceCollection = new ResourceCollection<ClientStatsResource>()
            {
                Href = BuildLink( new { controller = Prefix } ),
                Items = resources,
            };

            return Ok( resourceCollection );
        }

        /// <summary>
        /// Uploads streaming client performance metrics.
        /// </summary>
        /// <param name="metrics">The metrics data.</param>
        public IHttpActionResult Post( ClientStatsResource metrics )
        {
            Provider.CreateOrUpdateById( metrics );
            return Ok();
        }

        private ClientStatsResource ConvertToResource( ClientStatsResource model )
        {
            string url = BuildLink( new { controller = ProcessController.Prefix, id = model.Id } );
            model.Process = url;
            model.Href = BuildLink( new { controller = Prefix, id = model.Id } );
            model.ClientLocation = ConvertToResource( LocationProvider.Location );
            return model;
        }

        private LocationResource ConvertToResource( LocationModel model )
        {
            LocationResource resource = Mapper.Map<LocationResource>( model );
            resource.Href = BuildLink( new { controller = LocationController.Prefix } );
            return resource;
        }
    }
}
