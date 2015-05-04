using System.IO;

namespace Hydra.Deploy
{
    public static class DirectoryExtensions
    {
        public static void DeleteZipFiles( this DirectoryInfo dir )
        {
            foreach( FileInfo file in dir.GetFiles() )
            {
                if( file.Extension == ".zip" )
                {
                    file.Delete();
                }
            }
        }
    }
}
