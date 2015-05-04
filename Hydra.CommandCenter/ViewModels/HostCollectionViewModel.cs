using System.Linq;
using System.Collections.Generic;
using RzAspects;
using RzWpf;

namespace Hydra.CommandCenter
{
    public class HostCollectionViewModel : ViewModelBase
    {
        public DelegatedCommand AddHostCommand { get; private set; }
        public DelegatedCommand RemoveHostCommand { get; private set; }
        public ObservableCollectionEx<HostViewModel> Hosts { get; private set; }

        public string PropertySelectedHost { get { return "SelectedHost"; } }
        private HostViewModel _SelectedHost;
        public HostViewModel SelectedHost
        {
            get { return _SelectedHost; }
            set { SetProperty( PropertySelectedHost, ref _SelectedHost, value ); }
        }

        public HostCollectionViewModel()
        {
            Hosts = new ObservableCollectionEx<HostViewModel>();
            Hosts.ItemAdded += Hosts_ItemAdded;
            AddHostCommand = new DelegatedCommand( OnExecuteAddHost );
            RemoveHostCommand = new DelegatedCommand( OnExecuteRemoveHost, () => { return SelectedHost != null; } );
        }

        private void Hosts_ItemAdded( HostViewModel newHost )
        {
            if( Hosts.Count == 1 )
            {
                SelectedHost = newHost;
            }
        }

        private async void OnExecuteRemoveHost()
        {
            SelectedHost.DisconnectCommand.Execute( null );
            Hosts.Remove( SelectedHost );
            SelectedHost = null;
        }

        private async void OnExecuteAddHost()
        {
            var hostName = await Navigation.ShowInputAsync( "Add Host", "enter host name:" );

            if( !string.IsNullOrEmpty( hostName ) )
            {
                var showProgressController = await Navigation.ShowProgressAsync( "Verifying host", "attempting to connect to the host" );
                var host = new Host( hostName );
                var isReachable = await host.IsReachableAsync();
                await showProgressController.CloseAsync();

                if( isReachable )
                {
                    var newHostViewModel = new HostViewModel( host );
                    newHostViewModel.ConnectCommand.Execute( null );

                    Hosts.Add( newHostViewModel );
                }
                else
                {
                    await Navigation.ShowMessageAsync( 
                        "host not reachable", 
                        string.Format( "'{0}' could not be reached. Ensure the marionette service is running.", hostName ) );
                }
            }
        }

        internal List<string> GetHosts()
        {
            return Hosts.Select( h => h.Model.Name ).ToList();
        }
    }
}
