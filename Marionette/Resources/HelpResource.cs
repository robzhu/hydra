using Rz.Http;

namespace Marionette
{
    public class HelpEntry
    {
        public string Href { get; set; }
        public string Curl { get; set; }
        public string Httpie { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
    }

    public class HelpResource : Resource
    {
        public HelpEntry CreateApp { get; set; }
    }
}
