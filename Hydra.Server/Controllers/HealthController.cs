using System.Web.Http;

namespace Hydra.Server
{
    public class HealthController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Ok();
        }
    }
}
