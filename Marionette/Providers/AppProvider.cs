using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Marionette
{
    public class AppProvider
    {
        private const string AppsRoot = @"c:\marionette\apps";
        private const string FilePath = @"c:\marionette\data.json";
        private const string DefaultArchiveName = "package.zip";
        public static AppProvider Instance { get; private set; }

        static AppProvider()
        {
            if( File.Exists( FilePath ) )
            {
                var serializedData = File.ReadAllText( FilePath );
                Instance = JsonConvert.DeserializeObject<AppProvider>( serializedData );
            }
            if( Instance == null )
            {
                Instance = new AppProvider();
            }
            else
            {
                Instance.Refresh();
            }
        }

        public List<AppModel> Apps { get; set; }
        public List<ProcessModel> Processes { get; set; }

        private AppProvider() 
        {
            Apps = new List<AppModel>();
            Processes = new List<ProcessModel>();
        }

        public AppModel GetAppById( string id )
        {
            Refresh();
            return Apps.FirstOrDefault( app => app.Id == id );
        }

        public IEnumerable<AppModel> GetAll()
        {
            return Apps;
        }

        public void DeleteAppById( string id )
        {
            var app = GetAppById( id );
            if( app == null )
            {
                throw new Exception( string.Format( "app with ID {0} could not be found", id ) );
            }
            if( app.State == AppState.Running )
            {
                throw new Exception( string.Format( "Cannot remove app with ID {0} because it is still running", id ) );
            }

            var appDir = GetAppPackagePath( app );
            Directory.Delete( appDir, true );

            Apps.Remove( app );
            Save();
        }

        public async Task CreateAppAsync( string appId, FileHelper packageFileHelper )
        {
            if( GetAppById( appId ) != null )
            {
                throw new Exception( string.Format( "The app with Id {0} already exists.", appId ) );
            }

            var app = await AppBuilder.BuildAsync( AppsRoot, appId, packageFileHelper );
            Apps.Add( app );

            if( app.RunOnDeploy )
            {
                LaunchInstance( app.Id, "" );
            }
            Save();
        }

        public void Refresh()
        {
            //Remove any processes for which we cannot find a process id.
            List<ProcessModel> removeList = new List<ProcessModel>();
            foreach( var processModel in Processes )
            {
                try
                {
                    var process = Process.GetProcessById( processModel.ProcessId );
                    if( process == null || process.MainModule.FileName.ToLowerInvariant() != processModel.ExeName.ToLowerInvariant() )
                    {
                        removeList.Add( processModel );
                    }
                }
                catch
                {
                    removeList.Add( processModel );
                    continue;
                }
            }

            foreach( var toRemove in removeList )
            {
                Processes.Remove( toRemove );
            }
        }

        private bool IsDirectoryEmpty( string path )
        {
            return !Directory.EnumerateFileSystemEntries( path ).Any();
        }

        private string GetAppPackagePath( string appId )
        {
            return string.Format( "{0}\\{1}", AppsRoot, appId );
        }

        private string GetAppPackagePath( AppModel app )
        {
            return GetAppPackagePath( app.Id );
        }

        private string GetAppExeFullPath( AppModel app )
        {
            var dir = GetAppPackagePath( app );
            return string.Format( "{0}\\{1}", dir, app.ExeFileName );
        }

        private void Save()
        {
            var serializedData = JsonConvert.SerializeObject( this );
            var directory = Path.GetDirectoryName( FilePath );

            if( !Directory.Exists( directory ) )
            {
                Directory.CreateDirectory( directory );
            }
            File.WriteAllText( FilePath, serializedData );
        }

        public ProcessModel LaunchInstance( string appId, string parameters )
        {
            var app = GetAppById( appId );
            if( app == null )
            {
                throw new Exception( string.Format( "The app with Id {0} was not found.", appId ) );
            }

            var exe = GetAppExeFullPath( app );
            var process = Process.Start( exe, parameters );

            var appInstance = new ProcessModel
            {
                ProcessId = process.Id,
                AppId = app.Id,
                RunParameters = parameters,
                ExeName = exe,
            };

            Processes.Add( appInstance );
            Save();
            return appInstance;
        }

        public ProcessModel GetProcessById( int id )
        {
            Refresh();
            return Processes.FirstOrDefault( instance => instance.ProcessId == id );
        }

        public void DeleteProcessById( int id )
        {
            var process = GetProcessById( id );
            if( process == null ) return;

            process.Kill();
            Refresh();
            Save();
        }

        public IEnumerable<ProcessModel> GetProcessesByAppId( string appId )
        {
            return Processes.Where( p => p.AppId == appId );
        }

        public IEnumerable<ProcessModel> GetAllProcesses()
        {
            Refresh();
            return Processes;
        }
    }
}
