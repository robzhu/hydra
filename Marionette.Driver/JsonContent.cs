using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace Marionette.Driver
{
    public class JsonContent : StringContent
    {
        public const string JsonMediaType = "application/json";
        public JsonContent( object obj ) : base( JsonConvert.SerializeObject( obj ) ) { }
        public JsonContent( string json ) : base( json, Encoding.UTF8, JsonMediaType ) 
        {
        }
    }
}
