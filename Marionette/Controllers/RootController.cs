using System.Web.Http;
using System.Web.Http.Description;

namespace Marionette
{
    [ApiExplorerSettings( IgnoreApi = true )]
    public class RootController : ApiController
    {
        public IHttpActionResult Get()
        {
            var request = Request.RequestUri;
            return Redirect( request.ToString() + "swagger/ui/index" );
        }
    }
}
