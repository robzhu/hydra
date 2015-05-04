using Marionette.Driver;
using RzWpf;

namespace Hydra.CommandCenter
{
    public class ClientMetricsViewModel : ViewModelBase
    {
        public HostCollectionViewModel HostCollection { get; private set; }

        public string PropertyData { get { return "Data"; } }
        private ClientStatsResource _Data;
        public ClientStatsResource Data
        {
            get { return _Data; }
            set { SetProperty( PropertyData, ref _Data, value ); }
        }

        internal void CopyFrom( ClientStatsResource match )
        {
            Data = match;
        }
    }
}
