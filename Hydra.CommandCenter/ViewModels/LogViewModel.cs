using RzAspects;
using Serilog.Events;

namespace Hydra.CommandCenter
{
    public class LogViewModel : ModelBase
    {
        private LogEventSink _logEventSink;
        public ObservableCollectionEx<LogEventViewModel> Events { get; private set; }

        public LogViewModel( LogEventSink logEventSink )
        {
            _logEventSink = logEventSink;
            Events = new ObservableCollectionEx<LogEventViewModel>();

            foreach( var e in logEventSink.Events )
            {
                Events.Add( new LogEventViewModel( e ) );
            }

            _logEventSink.LogEventAdded += ( e ) =>
                {
                    UIThread.Marshall( () => Events.Add( new LogEventViewModel( e ) ) );
                };
        }
    }
}
