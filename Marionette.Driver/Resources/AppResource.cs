using Rz.Http;

namespace Marionette.Driver
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

    public class AppResource : Resource
    {
        /// <summary>
        /// The unique identity of the app.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The executable file name for this app.
        /// </summary>
        public string ExeFileName { get; set; }

        public AppState State { get; set; }

        public Hyperlink<ResourceCollection<ProcessResource>> Processes { get; set; }

        /// <summary>
        /// The hyperlink to use for launching new process instances of this app.
        /// </summary>
        public Hyperlink LaunchInstance { get; set; }
    }
}
