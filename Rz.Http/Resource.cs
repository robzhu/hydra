using System.Diagnostics;
using Newtonsoft.Json;

namespace Rz.Http
{
    public interface IResource
    {
        string Href { get; set; }
    }

    [DebuggerDisplay( "{Href}" )]
    public abstract class Resource : IResource
    {
        /// <summary>
        /// The "self" hypermedia link.
        /// </summary>
        [JsonProperty( Order = -100 )]
        public string Href { get; set; }
    }
}