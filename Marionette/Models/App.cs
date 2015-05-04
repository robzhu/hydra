using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Marionette
{
    public enum AppState
    {
        /// <summary>
        /// this app is ready for new instances to be launched.
        /// </summary>
        Ready,      

        /// <summary>
        /// this app has one or more instances running.
        /// </summary>
        Running,    
    }

    public class AppModel
    {
        public string Id { get; set; }
        public string ExeFileName { get; set; }
        public bool RunOnDeploy { get; set; }
        public AppState State { get; set; }
    }
}
