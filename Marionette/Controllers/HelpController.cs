using System.Web.Http;
using System.Web.Http.Description;

namespace Marionette
{
    [ApiExplorerSettings( IgnoreApi = true )]
    public class HelpController : ApiController
    {
        private string BuildLink( object routeValues )
        {
            return Url.Link( ApiRoute, routeValues );
        }

        private string ApiRoute { get; set; }

        public HelpController()
        {
            ApiRoute = Startup.DefaultApiRouteName;
        }

        [ResponseType( typeof( HelpResource ) )]
        public IHttpActionResult Get()
        {
            var help = new HelpResource
            {
                Href = BuildLink( null ),
                CreateApp = new HelpEntry
                {
                    Title = "Get all Apps",
                    Description = "Gets all the apps in the system",
                    Curl = string.Format( "curl -L {0}", BuildLink( new { controller = AppController.Prefix } ) ),
                    Httpie = string.Format( "http {0}", BuildLink( new { controller = AppController.Prefix } ) ),
                    Href = BuildLink( new { controller = AppController.Prefix } )
                }
            };
            return Ok( help );
        }
    }
}
