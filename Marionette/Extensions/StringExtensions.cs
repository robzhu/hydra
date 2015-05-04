using System.IO;
using System.Linq;

namespace Marionette
{
    public static class StringExtensions
    {
        public static bool IsDirectoryEmpty( this string path )
        {
            return !Directory.EnumerateFileSystemEntries( path ).Any();
        }
    }
}
