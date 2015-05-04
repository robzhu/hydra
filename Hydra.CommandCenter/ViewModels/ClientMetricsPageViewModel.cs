using RzWpf;

namespace Hydra.CommandCenter
{
    /// <summary>
    /// ViewModel for a page that lists the detailed client performance on a selected host.
    /// </summary>
    public class ClientMetricsPageViewModel : ViewModelBase
    {
        public HostCollectionViewModel HostCollection { get; private set; }

        public string PropertySelectedClient { get { return "SelectedClient"; } }
        private ClientMetricsViewModel _SelectedClient;
        public ClientMetricsViewModel SelectedClient
        {
            get { return _SelectedClient; }
            set { SetProperty( PropertySelectedClient, ref _SelectedClient, value ); }
        }

        public ClientMetricsPageViewModel( HostCollectionViewModel hostCollection )
        {
            HostCollection = hostCollection;
        }
    }
}