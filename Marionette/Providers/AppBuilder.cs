using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Marionette
{
    public class AppManifest
    {
        public string Exe { get; set; }
        public bool RunOnDeploy { get; set; }
    }

    public static class AppBuilder
    {
        const string DefaultArchiveName = "package.zip";
        const string ManifestFileName = "manifest.json";

        public static async Task<AppModel> BuildAsync( string appsRootDir, string appId, FileHelper packageFileHelper )
        {
            try
            {
                var appDir = InitializeDirectory( appsRootDir, appId );
                await UnpackAsync( packageFileHelper, appDir );
                var manifest = ParseManifest( appDir );

                return new AppModel
                {
                    Id = appId,
                    ExeFileName = manifest.Exe,
                    RunOnDeploy = manifest.RunOnDeploy,
                    State = AppState.Ready,
                };
            }
            catch( Exception ex )
            {
                Cleanup( appsRootDir, appId );
                throw ex;
            }
        }

        private static string InitializeDirectory( string appsRootDir, string appId )
        {
            var appDir = string.Format( "{0}\\{1}", appsRootDir, appId );

            if( Directory.Exists( appDir ) && !appDir.IsDirectoryEmpty() )
            {
                throw new Exception( string.Format( "Application directory {0} is not empty", appDir ) );
            }

            if( !Directory.Exists( appDir ) )
            {
                Directory.CreateDirectory( appDir );
            }

            return appDir;
        }

        private static async Task UnpackAsync( FileHelper packageFileHelper, string appDir )
        {
            await packageFileHelper.SaveAsync( appDir, DefaultArchiveName );
            var archiveFileName = string.Format( "{0}\\{1}", appDir, DefaultArchiveName );

            try
            {
                ZipFile.ExtractToDirectory( archiveFileName, appDir );
            }
            catch
            {
                throw new Exception( "The uploaded file was not a valid zip package. Make sure the contents are contained in a zip file." );
            }
        }

        private static AppManifest ParseManifest( string appDir )
        {
            //look for a file
            string manifestFile = string.Format( "{0}\\{1}", appDir, ManifestFileName );
            if( !File.Exists( manifestFile ) )
            {
                var exeFiles = new List<string>( Directory.GetFiles( appDir, "*.exe" ) );
                exeFiles.RemoveAll( file => file.Contains( "vshost" ) );
                if( exeFiles.Count == 1 )
                {
                    return new AppManifest
                    {
                        Exe = Path.GetFileName( exeFiles[ 0 ] ),
                    };
                }

                var sb = new StringBuilder();
                sb.AppendFormat( "Package contains multiple EXEs but does not contain a manifest file: {0}\n", ManifestFileName );
                sb.AppendFormat( "Include a file called {0} in your output folder (bin/debug) with the following format:\n", ManifestFileName );

                var sampleManifestJson = JsonConvert.SerializeObject( new AppManifest
                {
                    Exe = "{exeFileName}"
                }, Formatting.Indented );

                sb.AppendLine( sampleManifestJson );

                throw new Exception( sb.ToString() );
            }
            else
            {
                var data = File.ReadAllText( manifestFile );
                var manifest = JsonConvert.DeserializeObject<AppManifest>( data );

                var fullExePath = string.Format( "{0}\\{1}", appDir, manifest.Exe );

                if( !File.Exists( fullExePath ) )
                {
                    throw new Exception( string.Format( "Manifest executable file not found: {0}", fullExePath ) );
                }

                return manifest;
            }
        }

        private static void Cleanup( string appsRootDir, string appId )
        {
            string appDir = string.Format( "{0}\\{1}", appsRootDir, appId );
            if( Directory.Exists( appDir ) )
            {
                Directory.Delete( appDir, true );
            }
        }
    }
}
