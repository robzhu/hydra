using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Hydra.Shared;
using Microsoft.Owin.Hosting;

namespace Hydra.Server
{
    class Program
    {
        static void Main( string[] args )
        {
            var url = "http://*:15050";
            using( WebApp.Start( url ) )
            {
                Console.WriteLine( "Service started at {0}", url );
                Console.ReadLine();
            }
        }

        private static bool Running = false;
        public static int count = 0;
        public static int LastSentPayloadIndex = 0;
        public static int SendPayloadInterval = 500;
        public static int PayloadSizeBytes = 10000;

        internal static void Register( StreamHub hub )
        {
            if( !Running )
            {
                Running = true;
                Stopwatch sw = new Stopwatch();
                Task.Run( async () =>
                {
                    while( Running )
                    {
                        sw.Reset();
                        sw.Start();

                        Console.WriteLine( "Sending payload: {0}", count );
                        hub.SendPayload( new Payload( count, PayloadSizeBytes ) );
                        LastSentPayloadIndex = count;
                        count++;
                        sw.Stop();

                        //Console.WriteLine( "Time spent in SendPayload: {0:0}", sw.ElapsedMilliseconds );
                        await Task.Delay( SendPayloadInterval - (int)sw.ElapsedMilliseconds );
                    }
                } );
            }
        }
    }
}
