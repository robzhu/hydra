using Rz.Http;

namespace Marionette.Driver
{
    //public class ProcessResource : Resource
    //{
    //    public AppResource App { get; set; }
    //    public int ProcessId { get; set; }
    //    public string RunParameters { get; set; }
    //}

    public class ProcessResource : Resource
    {
        /// <summary>
        /// The unique identity of the process.
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// The executable file name for this app.
        /// </summary>
        public string RunParameters { get; set; }

        public Hyperlink<AppResource> App { get; set; }
    }
}
