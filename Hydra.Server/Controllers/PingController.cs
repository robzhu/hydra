using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Hydra.Shared;
using Marionette.Driver;

namespace Hydra.Server
{
    public class PingController : ApiController
    {
        static Client Client = new Client( "localhost" );

        static Location _Location;
        static Location Location
        {
            get
            {
                if( _Location == null )
                {
                    var locationResource = Client.GetHostLocationAsync().Result;
                    _Location = new Location
                    {
                        AzureRegion = locationResource.AzureRegion,
                        Latitude = locationResource.Latitude,
                        Longitude = locationResource.Longitude,
                        Name = locationResource.Location,
                    };
                }
                return _Location;
            }
        }

        [ResponseType( typeof( Pong ) )]
        public IHttpActionResult Get( string nonce = null )
        {
            return Ok( new Pong
            {
                Nonce = nonce,
                LastSentPayloadIndex = Program.LastSentPayloadIndex,
                SendPayloadInterval = Program.SendPayloadInterval,
                PayloadSizeBytes = Program.PayloadSizeBytes,
                Location = PingController.Location
            } );
        }
    }
}
