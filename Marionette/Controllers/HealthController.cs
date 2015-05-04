using System.Web.Http;
using System.Web.Http.Description;

namespace Marionette
{
    [ApiExplorerSettings( IgnoreApi = true )]
    public class HealthController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Ok();
        }
    }
}
