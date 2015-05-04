using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Rz.Http;

namespace Hydra.Server
{
    public static class HttpConfigurationExtensions
    {
        public static void UseJsonSerialization( this HttpConfiguration config )
        {
            var defaultJsonSettings = JsonSettings.WebDefaults;

            JsonConvert.DefaultSettings = ( () =>
            {
                return defaultJsonSettings;
            } );

            config.Formatters.Clear();
            config.Formatters.Add( new JsonMediaTypeFormatter() );
            config.Formatters.JsonFormatter.SerializerSettings = defaultJsonSettings;
        }
    }
}
