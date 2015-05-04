using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Rz.Http
{
    public static class JsonSettings
    {
        public static JsonSerializerSettings WebDefaults
        {
            get
            {
                return new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = new List<JsonConverter>
                    {
                        new HyperlinkConverter(),
                        new StringEnumConverter { CamelCaseText = true },
                    }
                };
            }
        }
    }
}
