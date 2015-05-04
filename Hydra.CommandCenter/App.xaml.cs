using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Rz.Http;
using Serilog;
using Serilog.Core;

namespace Hydra.CommandCenter
{
    public partial class App : Application
    {
        public static ILogger Log;
        public static LogEventSink LogSink;

        public App()
        {
            LogSink = new LogEventSink();
            Log = new LoggerConfiguration().WriteTo.Sink( LogSink ).CreateLogger();
            Log.Information( "Application started" );
            ConfiguratJsonSettings();
        }

        private static void ConfiguratJsonSettings()
        {
            JsonConvert.DefaultSettings = () => JsonSettings.WebDefaults;
        }
    }
}
