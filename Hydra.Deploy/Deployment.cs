using System;
using System.IO;
using System.Threading.Tasks;
using Marionette.Driver;
using Serilog;

namespace Hydra.Deploy
{
    public class Deployment
    {
        public static void Deploy( string package, string target, ILogger log )
        {
            new Deployment( package, target, log ).DeployAsync().Wait();
        }

        public string Package { get; private set; }

        private Client _client;
        private ILogger _log;

        public Deployment( string package, string target, ILogger log )
        {
            Package = package;
            _client = new Client( target );
            _log = log;
        }

        public async Task DeployAsync()
        {
            try
            {
                await CheckServerHealthAsync();
                await RemoveExistingAppAsync();
                await UploadPackageAsync();
            }
            catch( Exception ex )
            {
                _log.Error( ex.Message );
            }
        }

        public async Task LaunchAppAsync( string parameters )
        {
            var appName = Path.GetFileNameWithoutExtension( Package );
            if( !await _client.LaunchAppAsync( appName, parameters ) )
            {
                throw new Exception( string.Format( "failed to launch process instance for app: {0}", appName ) );
            }
        }

        private async Task CheckServerHealthAsync()
        {
            _log.Information( "Checking for Marionette endpoint on {0}...", _client.Address );
            await _client.EnsureServiceAvailableAsync();
        }

        private async Task RemoveExistingAppAsync()
        {
            var appName = Path.GetFileNameWithoutExtension( Package );
            var existingApp = await _client.GetAppWithNameAsync( appName );
            if( existingApp != null )
            {
                _log.Information( "Removing existing application with the same name..." );
                await _client.DeleteAppWithNameAsync( appName );
            }
        }

        private async Task UploadPackageAsync()
        {
            _log.Information( "Uploading app package..." );
            if( !await _client.CreateAppAsync( Package ) )
            {
                throw new Exception( string.Format( "upload package failed. Aborting.", _client.Address ) );
            }
        }
    }
}
