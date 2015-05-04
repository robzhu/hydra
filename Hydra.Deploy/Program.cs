using System;
using System.IO;
using Serilog;

namespace Hydra.Deploy
{
    public class Program
    {
        public const bool Debug = false;
        public static ILogger Log = CreateLogger();

        //Deploy.exe <solution> <server>
        //Deploy.exe D:\VSScratch\signalr_server01\signalr_server01\signalr_server01.csproj serverName.cloudapp.net
        static void Main( string[] args )
        {
            AllowDebugger();

            try
            {
                ValidateArgs( args );
                Build.CleanAndBuild( args[ 0 ], Log );
                var package = Package.Create( args[ 0 ], Log );
                Deployment.Deploy( package, args[ 1 ], Log );
            }
            catch( Exception ex )
            {
                Log.Error( "Unhandled Exception: " + ex.Message );
                Console.ReadLine();
            }
        }

        private static ILogger CreateLogger()
        {
            return new LoggerConfiguration().WriteTo.ColoredConsole().CreateLogger();
        }

        private static void AllowDebugger()
        {
            if( Debug )
            {
                Log.Information( "Attach Debugger..." );
                Console.ReadLine();
            }
        }

        private static void ValidateArgs( string[] args )
        {
            if( args.Length != 2 ) throw new Exception( "Usage: Deploy.exe <solution> <server>" );
            else if( !File.Exists( args[ 0 ] ) )
            {
                throw new Exception( "solution file does not exist." );
            }
        }
    }
}
