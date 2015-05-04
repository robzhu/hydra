using System.Linq;
using RzWpf;

namespace Hydra.CommandCenter
{
    public class MainWindowViewModel : ViewModelBase
    {
        public DeploymentPageViewModel Deployment { get; private set; }
        public ClientMetricsPageViewModel ClientMetrics { get; private set; }
        public MapPageViewModel Map { get; private set; }
        public LogViewModel LogVM { get; private set; }
        public HostCollectionViewModel HostCollection { get; private set; }

        public MainWindowViewModel()
        {
            HostCollection = new HostCollectionViewModel();
            Deployment = new DeploymentPageViewModel( HostCollection );
            ClientMetrics = new ClientMetricsPageViewModel( HostCollection );
            Map = new MapPageViewModel( HostCollection );
            LogVM = new LogViewModel( App.LogSink );
            LoadDataFromSettings();
        }

        private void LoadDataFromSettings()
        {
            foreach( var host in Settings.Instance.Hosts )
            {
                var hostVM = new HostViewModel( new Host( host ) );
                hostVM.ConnectCommand.Execute( null );
                HostCollection.Hosts.Add( hostVM );
            }
            foreach( var package in Settings.Instance.Packages )
            {
                Deployment.Packages.Add( new PackageViewModel
                {
                    Location = package
                } );
            }
            foreach( var project in Settings.Instance.Projects )
            {
                Deployment.Projects.Add( ProjectViewModel.Create( project ) );
            }
        }

        internal void OnClosing()
        {
            Settings.Instance.Hosts = HostCollection.GetHosts();
            Settings.Instance.Packages = Deployment.Packages.Select( p => p.Location ).ToList();
            Settings.Instance.Projects = Deployment.Projects.Select( p => p.FolderPath ).ToList();
            Settings.Save();
        }
    }
}
