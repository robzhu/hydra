using System.Web.Http;
using System.Web.Http.Description;

namespace Marionette
{
    public class MetricsController : ApiController
    {
        public const string Prefix = "Metrics";
        public const string Route_GetProcessById = "GetById";

        private MetricsProvider Provider { get; set; }
        private string ApiRoute { get; set; }

        public MetricsController()
        {
            Provider = new MetricsProvider();
            ApiRoute = Startup.DefaultApiRouteName;
        }

        private string BuildLink( object routeValues )
        {
            return Url.Link( ApiRoute, routeValues );
        }

        [ResponseType( typeof( MetricsResource ) )]
        public IHttpActionResult Get()
        {
            var resource = Provider.GetUsage();
            resource.Href = BuildLink( null );
            return Ok( resource );
        }
    }
}
