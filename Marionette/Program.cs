using System;
using Microsoft.Owin.Hosting;

namespace Marionette
{
    /// <summary>
    /// Marionette is an HTTP API that allows for remote control of the machine it runs on. It allows:
    /// 
    /// -Copying entire app folders
    /// -Starting/stopping/monitoring those apps processes
    /// -Removing apps
    /// 
    /// Think of it as an HTTP API for PsExec
    /// </summary>

    class Program
    {
        static void Main( string[] args )
        {
            LocationProvider.SetOrPrompt( args );
            Console.Clear();

            var url = "http://*:5050";
            using( WebApp.Start( url ) )
            {
                Console.WriteLine( "Marionette started on port 5050" );
                Console.ReadLine();
            }
        }
    }
}
