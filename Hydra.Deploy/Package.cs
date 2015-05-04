using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Serilog;

namespace Hydra.Deploy
{
    public static class Package
    {
        public const string PackagePathName = "Package";
        public static string[] Projects = new string[]
        {
            "signalr_server01",
        };

        private static string GetPackagePath( string solutionPath )
        {
            return Path.GetFullPath( string.Format( "{0}\\{1}", Path.GetDirectoryName( solutionPath ), PackagePathName ) );
        }

        public static string Create( string solutionPath, ILogger log )
        {
            ClearPackageDirectory( solutionPath, log );
            return GenerateProjectArchive( solutionPath, log );
        }

        private static void ClearPackageDirectory( string solutionPath, ILogger log )
        {
            string packagePath = GetPackagePath( solutionPath );
            if( Directory.Exists( packagePath ) )
            {
                Directory.Delete( packagePath, true );
            }
            Directory.CreateDirectory( packagePath );
        }

        /// <summary>
        /// Creates a list of paths to the project archives.
        /// </summary>
        private static string GenerateProjectArchive( string solutionPath, ILogger log )
        {
            var sourcePath = Path.GetFullPath( string.Format( "{0}\\bin\\debug", Path.GetDirectoryName( solutionPath ) ) );
            new DirectoryInfo( sourcePath ).DeleteZipFiles();

            var targetPath = GetPackagePath( solutionPath );
            var projectName = Path.GetFileNameWithoutExtension( solutionPath );
            var archiveName = Path.GetFullPath( string.Format( "{0}\\{1}.zip", targetPath, projectName ) );

            log.Information( "Packing: " + sourcePath );

            if( File.Exists( archiveName ) )
            {
                File.Delete( archiveName );
            }
            ZipFile.CreateFromDirectory( sourcePath, archiveName );

            return archiveName;
        }
    }
}
