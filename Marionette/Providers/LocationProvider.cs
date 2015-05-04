using System;

namespace Marionette
{
    public static class LocationProvider
    {
        public static LocationModel Location { get; private set; }

        static LocationProvider()
        {
            Location = Locations.Data[ 1 ];
        }

        internal static void SetOrPrompt( string[] args )
        {
            if( ( args == null ) || ( args.Length == 0 ) )
            {
                PromptLocation();
                return;
            }

            int locationCode;
            if( int.TryParse( args[ 0 ], out locationCode ) )
            {
                if( Locations.Data.ContainsKey( locationCode ) )
                {
                    Location = Locations.Data[ locationCode ];
                }
            }
            PromptLocation();
        }

        private static void PromptLocation()
        {
            Console.Clear();
            foreach( var location in Locations.Data.Values )
            {
                Console.WriteLine( "{0}- {1}", location.Id, location.AzureRegion );
            }
            Console.Write( "Choose a Location: " );
            var input = Console.ReadLine();

            int selectedCode;
            if( int.TryParse( input, out selectedCode ) )
            {
                if( Locations.Data.ContainsKey( selectedCode ) )
                {
                    Location = Locations.Data[ selectedCode ];
                }
                else
                {
                    PromptLocation();
                }
            }
            else
            {
                PromptLocation();
            }
        }
    }
}
