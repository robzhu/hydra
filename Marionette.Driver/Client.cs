using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Rz.Http;

namespace Marionette.Driver
{
    public interface IClient
    {
    }

    public class Client
    {
        public const int DefaultPort = 5050;
        public string Address { get; private set; }

        private HttpClient _httpClient;

        public Client( string address, int port = DefaultPort )
        {
            Address = string.Format( "http://{0}:{1}", address, port );

            Uri result;
            if( !Uri.TryCreate( Address, UriKind.Absolute, out result ) )
            {
                throw new Exception( "Invalid address" );
            }

            _httpClient = new HttpClient();
        }

        //http post server/app/{id}     (body is multipart form content)
        public async Task<bool> CreateAppAsync( string appPackagePath )
        {
            var message = new HttpRequestMessage();
            var content = new MultipartFormDataContent();

            var fileStream = new FileStream( appPackagePath, FileMode.Open );
            var fileName = Path.GetFileName( appPackagePath );
            content.Add( new StreamContent( fileStream ), "file", fileName );

            message.Method = HttpMethod.Post;
            message.Content = content;
            var packageFileName = Path.GetFileNameWithoutExtension( appPackagePath );
            message.RequestUri = new Uri( Address + "/app/" + packageFileName );

            var result = await _httpClient.SendAsync( message );
            if( result.StatusCode == HttpStatusCode.BadRequest )
            {
                var badRequestMessage = await result.Content.ReadAsStringAsync();
            }
            return result.StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> LaunchAppAsync( string appName, string parameters = null )
        {
            var url = string.Format( "{0}/Process?appId={1}", Address, appName );
            if( !string.IsNullOrEmpty( parameters ) )
            {
                url += ( "parameters=" + parameters );
            }
            var response = await _httpClient.PostAsync( url, null );
            return response.IsSuccessStatusCode;
        }

        public async Task EnsureServiceAvailableAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync( Address + "/health" );
            response.EnsureSuccessStatusCode();
        }

        public async Task<LocationResource> GetHostLocationAsync()
        {
            HttpResponseMessage response = ( await _httpClient.GetAsync( Address + "/location" ) ).EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LocationResource>( content );
        }

        public async Task<string> GetAppWithNameAsync( string name )
        {
            HttpResponseMessage response = await _httpClient.GetAsync( Address + "/app/" + name );
            if( response.IsSuccessStatusCode )
            {
                var contentString = await response.Content.ReadAsStringAsync();
                return contentString;
            }
            return null;
        }

        public async Task<bool> DeleteAppWithNameAsync( string name )
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync( Address + "/app/" + name );
            return response.IsSuccessStatusCode;
        }

        public async Task<ResourceUsageSample> GetResourceUsageAsync()
        {
            var response = await _httpClient.GetAsync( Address + "/metrics" );
            if( response.IsSuccessStatusCode )
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ResourceUsageSample>( content );
            }
            return null;
        }

        public async Task<ResourceCollection<AppResource>> GetAppsAsync( string expandQuery = null )
        {
            var route = "/app";
            if( !string.IsNullOrWhiteSpace( expandQuery ) )
            {
                route += "?expand=" + expandQuery;
            }
            var response = await _httpClient.GetAsync( Address + route );
            if( response.IsSuccessStatusCode )
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ResourceCollection<AppResource>>( content );
            }
            return null;
        }

        public async Task<ResourceCollection<ClientStatsResource>> GetAllClientStatsAsync()
        {
            var route = "/ClientMetrics";
            var response = ( await _httpClient.GetAsync( Address + route ) ).EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResourceCollection<ClientStatsResource>>( content );
        }
    }
}
