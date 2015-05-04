using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Threading.Tasks;
using Hydra.Shared;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using Newtonsoft.Json;

namespace Hydra.Client
{
    public struct ConnectionRetryOptions
    {
        public int Retries { get; set; }
        public int RetryInterval { get; set; }
    }

    public class ScaleTestClient
    {
        public event Action<Payload> OnReceivedPayload;
        public event Action OnConnectionClosed
        {
            add { _hubConnection.Closed += value; }
            remove { _hubConnection.Closed -= value; }
        }

        public event Action<Exception> ConnectionException
        {
            add { _hubConnection.Error += value; }
            remove { _hubConnection.Error -= value; }
        }

        public ClientStatsEx Stats { get; set; }
        public bool Connected { get; set; }

        private HubConnection _hubConnection;
        private IHubProxy _streamHubProxy;
        private Stopwatch _stopwatch = new Stopwatch();
        private HttpClient _httpClient = new HttpClient();
        private string _serverUrl;

        public ScaleTestClient()
        {
            Stats = new ClientStatsEx()
            {
                Name = Environment.MachineName + "-" + Process.GetCurrentProcess().Id,
                Id = Process.GetCurrentProcess().Id,
            };
            Connected = false;
        }
        
        public async Task ConnectAsync( string url, ConnectionRetryOptions retryOptions )
        {
            _serverUrl = url;
            if( _hubConnection == null )
            {
                _hubConnection = new HubConnection( url );
                _streamHubProxy = _hubConnection.CreateHubProxy( "streamHub" );

                _streamHubProxy.On<Payload>( "sendPayload", payload =>
                {
                    Stats.AddPayload( payload );
                    if( OnReceivedPayload != null ) OnReceivedPayload( payload );
                } );

                _hubConnection.Closed += () =>
                {
                    Connected = false;
                };

                _hubConnection.Error += ( exception ) =>
                {
                    if( exception is SocketException )
                    {
                        Connected = false;
                        Stats.Availability.DisconnectCount++;
                    }
                };
            }

            while( true )
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine( "Connecting to {0}, {1} retries left", url, retryOptions.Retries );
                    //await _hubConnection.Start();
                    await _hubConnection.Start( new LongPollingTransport() );
                    Stats.Transport = _hubConnection.Transport.Name;
                    Connected = true;
                    break;
                }
                catch( HttpRequestException ex )
                {
                    Stats.Availability.ConnectionEstablishmentFailCount++;
                    if( retryOptions.Retries == 0 )
                    {
                        throw ex;
                    }
                    else
                    {
                        retryOptions.Retries--;
                    }
                }
                await Task.Delay( retryOptions.RetryInterval );
            }
        }

        public async Task<Pong> PingAsync()
        {
            _stopwatch.Restart();
            var response = await _httpClient.GetAsync( _serverUrl + "api/ping" );
            _stopwatch.Stop();

            if( !response.IsSuccessStatusCode )
            {
                Stats.Availability.PingFailures++;
                return null;
            }
            
            Stats.Ping.AddSample( (double)_stopwatch.ElapsedMilliseconds );

            var contentString = await response.Content.ReadAsStringAsync();
            Pong pong = JsonConvert.DeserializeObject<Pong>( contentString );
            pong.DurationMilliseconds = (int)_stopwatch.ElapsedMilliseconds;

            return pong;
        }

        public void UpdateTimeSinceLastPayloadReceived()
        {
            Stats.UpdateTimeSinceLastPayloadReceived();
        }

        public void DisplayStats()
        {
            Console.Clear();
            Stats.ConnectionState = _hubConnection.State.ToString();
            Stats.Display();
        }

        public void ProcessPong( Pong pong )
        {
            Stats.DataIntegrity.LastServerPayloadIndex = pong.LastSentPayloadIndex;
            Stats.ServerSendPayloadInterval = pong.SendPayloadInterval;
            Stats.ServerLocation = pong.Location;
            Stats.ServerPayloadSizeKiloBytes = pong.PayloadSizeBytes / 1000;
        }

        public async Task SendStats()
        {
            Stats.Timestamp = DateTime.Now;
            var url = "http://localhost:5050/clientMetrics";

            HttpRequestMessage request = new HttpRequestMessage( HttpMethod.Post, url );
            request.Content = new JsonContent( Stats )
            {
                Headers = { ContentType = new MediaTypeHeaderValue( "application/json" ) }
            };

            var response = ( await _httpClient.SendAsync( request ) ).EnsureSuccessStatusCode();
        }
    }
}
