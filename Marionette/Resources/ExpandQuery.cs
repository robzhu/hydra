using System.Linq;

namespace Marionette
{
    public class ExpandQuery
    {
        public const char Delimeter = ',';
        private string[] Values;

        public ExpandQuery( string query )
        {
            if( query != null )
            {
                Values = query.Split( Delimeter );
            }
        }

        public bool Contains( string field )
        {
            if( Values == null ) return false;

            return Values.Any( value => value.ToLowerInvariant() == field.ToLowerInvariant() );
        }
    }
}
