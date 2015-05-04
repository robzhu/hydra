using Hydra.Shared;
using Microsoft.AspNet.SignalR;

namespace Hydra.Server
{
    public class StreamHub : Hub
    {
        public StreamHub()
        {
            Program.Register( this );
        }

        public void Send( string name, string message )
        {
            Clients.All.broadcastPayload( new { Name = name, Message = message } );
        }

        public void SendPayload( Payload payload )
        {
            Clients.All.sendPayload( payload );
        }

        public string Ping()
        {
            return "pong";
        }
    }
}
