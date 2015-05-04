using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Hydra.Client
{
    class Program
    {
        const string DefaultServerUrl = "http://localhost:15050/";
        //const string DefaultServerUrl = "http://us-east-01.cloudapp.net:15050/";
        //const string DefaultServerUrl = "http://rzhuws2012:15050/";
        const int PingInterval = 500;
        static ConnectionRetryOptions RetrySettings = new ConnectionRetryOptions
                {
                    Retries = 20,
                    RetryInterval = 1000,
                };

        static string GetServerUrl( string[] args )
        {
            if( args == null || args.Length == 0 )
            {
                return DefaultServerUrl;
            }

            var serverName = args[ 0 ];

            if( !serverName.StartsWith( "http" ) )
            {
                serverName = "http://" + serverName;
            }
            if( !serverName.EndsWith( "15050" ) )
            {
                serverName = serverName + ":15050/";
            }
            return serverName;
        }

        static void Main( string[] args )
        {
            var serverAddress = GetServerUrl( args );
            var healthUrl = serverAddress + "health";
            var httpClient = new HttpClient();
            try
            {
                //httpClient.GetAsync( healthUrl ).Result.EnsureSuccessStatusCode();
                var response = httpClient.GetAsync( healthUrl ).Result;
                Console.WriteLine( "HTTP Health check on {0} succeeded. ", healthUrl );
            }
            catch
            {
                Console.WriteLine( "HTTP Health check against {0} failed. Giving up.", healthUrl );
                return;
            }

            ScaleTestClient client = new ScaleTestClient();
            client.ConnectAsync( serverAddress, RetrySettings ).Wait();

            client.OnReceivedPayload += ( payload ) =>
                {
                    client.DisplayStats();
                };

            client.OnConnectionClosed += () =>
                {
                    throw new Exception( "ERROR: disconnected from server." );
                };

            Task.Run( async () => { await PingLoop( client ); } );

            Console.ReadLine();
        }

        private static async Task PingLoop( ScaleTestClient client )
        {
            var sw = new Stopwatch();
            while( true )
            {
                sw.Reset();
                sw.Start();
                

                if( !client.Connected )
                {
                    try
                    {
                        client.DisplayStats();
                        await client.ConnectAsync( DefaultServerUrl, RetrySettings );
                    }
                    catch
                    {
                        Console.WriteLine( "Could not connect, giving up." );
                        break;
                    }

                    //client.UpdateLastPayloadInterval();
                    //client.DisplayStats();
                    //Thread.Sleep( 1000 );
                    //continue;
                }

                var pong = await client.PingAsync();
                if( pong != null )
                {
                    client.ProcessPong( pong );
                    client.SendStats().Wait();
                }
                //client.UpdateTimeSinceLastPayloadReceived();
                client.DisplayStats();

                sw.Stop();

                var timeRemaining = PingInterval - (int)sw.ElapsedMilliseconds;
                if( timeRemaining > 0 ) Thread.Sleep( timeRemaining );
            }
        }
    }
}
