using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Rz.Http;

namespace Marionette
{
    public static class ApiControllerExtensions
    {
        public static HttpResponseMessage CreatedAtSelfLocation( this ApiController controller, Resource resource )
        {
            var response = controller.Request.CreateResponse( HttpStatusCode.OK, resource );
            response.Headers.Location = new Uri( resource.Href );
            return response;
        }
    }
}
