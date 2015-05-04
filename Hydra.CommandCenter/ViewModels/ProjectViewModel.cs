using System;
using System.Linq;
using System.IO;
using RzWpf;

namespace Hydra.CommandCenter
{
    public class ProjectViewModel : ViewModelBase
    {
        internal static ProjectViewModel Create( string projectFolderPath )
        {
            if( !Directory.Exists( projectFolderPath ) ) throw new ArgumentException( "{0} is not a valid directory", projectFolderPath );

            DirectoryInfo di = new DirectoryInfo( projectFolderPath );
            var files = di.GetFiles( "*.csproj", SearchOption.TopDirectoryOnly );
            if( !files.Any() )
            {
                throw new ArgumentException( string.Format( "\"{0}\" does not contain a CS project", projectFolderPath ) );
            }
            else if( files.Count() > 1 )
            {
                throw new ArgumentException( string.Format( "\"{0}\" contains multiple CS projects. This is not supported yet.", projectFolderPath ) );
            }

            var fileName = files[ 0 ].Name;
            return new ProjectViewModel()
            {
                FolderPath = projectFolderPath,
                ProjectFilePath = projectFolderPath + "\\" + fileName,
                Name = fileName,
            };
        }

        public string FolderPath { get; private set; }
        public string ProjectFilePath { get; private set; }
        public string Name { get; private set; }

        public ProjectViewModel()
        {

        }
    }
}
