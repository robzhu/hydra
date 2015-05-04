using System;
using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace Hydra.CommandCenter
{
    public class LogEventSink : ILogEventSink
    {
        public event Action<LogEvent> LogEventAdded;
        public List<LogEvent> Events { get; private set; }

        public LogEventSink()
        {
            Events = new List<LogEvent>();
        }

        public void Emit( LogEvent logEvent )
        {
            Events.Add( logEvent );

            var evt = LogEventAdded;
            if( evt != null ) evt( logEvent );
        }
    }
}
