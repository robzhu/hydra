using RzWpf;
using Serilog.Events;

namespace Hydra.CommandCenter
{
    public class LogEventViewModel : ViewModelBase<LogEvent>
    {
        public string Message { get; private set; }

        public LogEventViewModel( LogEvent model )
        {
            Model = model;
            Message = Model.RenderMessage();
        }
    }
}
