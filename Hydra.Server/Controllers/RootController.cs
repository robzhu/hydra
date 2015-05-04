using System.Web.Http;
using System.Web.Http.Description;

namespace Hydra.Server
{
    [ApiExplorerSettings( IgnoreApi = true )]
    public class RootController : ApiController
    {
        public IHttpActionResult Get()
        {
            var request = Request.RequestUri.ToString();

            if( request.EndsWith( "/" ) )
            {
                return Redirect( request + "swagger/ui/index" );
            }
            else
            {
                return Redirect( request + "/swagger/ui/index" );
            }
        }
    }
}
