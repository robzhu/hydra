using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Marionette
{
    public class ProcessModel
    {
        public int ProcessId { get; set; }
        public string AppId { get; set; }
        public string RunParameters { get; set; }
        public string ExeName { get; set; }

        internal void Kill()
        {
            Process.GetProcessById( ProcessId ).Kill();
        }
    }
}
