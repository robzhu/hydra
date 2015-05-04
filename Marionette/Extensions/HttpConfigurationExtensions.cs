using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Rz.Http;

namespace Marionette
{
    public static class HttpConfigurationExtensions
    {
        public static void UseJsonSerialization( this HttpConfiguration config )
        {
            var defaultSettings = JsonSettings.WebDefaults;

            JsonConvert.DefaultSettings = () => defaultSettings;

            config.Formatters.Clear();
            config.Formatters.Add( new JsonMediaTypeFormatter() );
            config.Formatters.JsonFormatter.SerializerSettings = defaultSettings;
        }
    }
}
