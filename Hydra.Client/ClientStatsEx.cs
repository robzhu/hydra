using System.Linq;
using System;
using System.Collections.Generic;
using Hydra.Shared;
using Marionette.Driver;

namespace Hydra.Client
{
    public class ClientStatsEx : ClientStatsResource
    {
        public ClientStatsEx()
        {
            Ping = new Statistic() { RunningSampleSize = 60 };
            PayloadReceiveInterval = new Statistic() { RunningSampleSize = 60 };
            Availability = new AvailabilityStats();
            DataIntegrity = new IntegrityStats();
        }

        private object _payloadLock = new object();
        public void AddPayload( Payload payload )
        {
            lock( _payloadLock )
            {
                UpdateLastPayloadInterval();

                if( ( DataIntegrity.PayloadCount != 0 ) && ( payload.Sequence != ( DataIntegrity.PayloadSequence + 1 ) ) )
                {
                    DataIntegrity.PayloadSequenceErrors++;
                }

                DataIntegrity.PayloadCount++;
                DataIntegrity.TotalBytesReceived += payload.Data.Length;
                DataIntegrity.PayloadSequence = payload.Sequence;

                if( !payload.IsDataValid() )
                {
                    DataIntegrity.PayloadDataErrors++;
                }
            }
        }

        public void UpdateLastPayloadInterval()
        {
            UpdateTimeSinceLastPayloadReceived();
            Availability.LastPayloadReceived = DateTime.Now;

            if( DataIntegrity.PayloadCount == 0 )
            {
                return;
            }

            PayloadReceiveInterval.AddSample( Availability.TimeSinceLastPayloadReceived );
        }

        public void UpdateTimeSinceLastPayloadReceived()
        {
            Availability.TimeSinceLastPayloadReceived = (int)( DateTime.Now - Availability.LastPayloadReceived ).TotalMilliseconds;
        }

        public void Display()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine( "Client Name: {0}", Name );
            Console.WriteLine( "Process Id: {0}", Id );
            Console.WriteLine( "Connection State: {0}", ConnectionState );
            Console.WriteLine( "Client transport: {0}", Transport );
            Console.WriteLine();

            Console.WriteLine( "Availability:" );
            Console.WriteLine( "Number of disconnections: {0}", Availability.DisconnectCount );
            Console.WriteLine( "Ping Failures: {0}", Availability.PingFailures );
            Console.WriteLine( "Connection Establishment Failures: {0}", Availability.ConnectionEstablishmentFailCount );
            Console.WriteLine( "Last Payload Received: {0} ms ago", Availability.TimeSinceLastPayloadReceived );
            Console.WriteLine();

            Console.WriteLine( "Integrity:" );
            Console.WriteLine( "Payload sequence #: {0}", DataIntegrity.PayloadSequence );
            Console.WriteLine( "Payload sequence errors: {0}", DataIntegrity.PayloadSequenceErrors );
            Console.WriteLine( "Payload data errors: {0}", DataIntegrity.PayloadDataErrors );
            Console.WriteLine( "Total payloads received: {0}", DataIntegrity.PayloadCount );
            Console.WriteLine( "Total bytes received: {0}", DataIntegrity.TotalBytesReceived );
            Console.WriteLine();

            Console.WriteLine( "Quality:" );
            Console.WriteLine( "Last Ping (ms): {0:0}", Ping.LastSample );
            Console.WriteLine( "Ping Running Average (ms): {0:0}", Ping.RunningAverage );
            Console.WriteLine( "Ping Running Standard Deviation (ms): {0:0}", Ping.RunningStandardDeviation );

            var lastInterval = ( DataIntegrity.PayloadCount != 0 ) ? PayloadReceiveInterval.LastSample : 0;
            Console.WriteLine( "Last Payload interval (ms): {0:0}", lastInterval );
            Console.WriteLine( "Payload interval avarege (ms): {0:0}", PayloadReceiveInterval.RunningAverage );
            Console.WriteLine( "Payload interval std dev (ms): {0:0}", PayloadReceiveInterval.RunningStandardDeviation );
        }
    }
}
