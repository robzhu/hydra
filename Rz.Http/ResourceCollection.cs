using System.Collections.Generic;
using Newtonsoft.Json;

namespace Rz.Http
{
    public interface IResourceCollection<T> : IResource where T : IResource
    {
        //int Offset { get; }
        //int Limit { get; }
        //int Size { get; }
        List<T> Items { get; }
    }

    public class ResourceCollection<T> : Resource, IResourceCollection<T> where T : IResource
    {
        //[JsonProperty( Order = -90 )]
        //public int Size { get; set; }

        //[JsonProperty( Order = -89 )]
        //public int Offset { get; set; }

        //[JsonProperty( Order = -88 )]
        //public int Limit { get; set; }

        [JsonProperty( Order = 0 )]
        public List<T> Items { get; set; }

        public ResourceCollection()
        {
            Items = new List<T>();
        }
    }
}