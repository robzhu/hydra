using System.Windows.Input;
using RzWpf;

namespace Hydra.CommandCenter
{
    public class HostViewModel : ViewModelBase<Host>
    {
        public ICommand ConnectCommand { get; private set; }
        public ICommand DisconnectCommand { get; private set; }

        public string PropertyIsExpanded { get { return "IsExpanded"; } }
        private bool _IsExpanded = false;
        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set { SetProperty( PropertyIsExpanded, ref _IsExpanded, value ); }
        }

        public HostViewModel( Host model )
        {
            Model = model;
            Model.Apps.ItemAdded += OnAppAdded;
            ConnectCommand = new DelegatedCommand( OnExecuteConnect );
            DisconnectCommand = new DelegatedCommand( OnExecuteDisconnect );
        }

        private void OnAppAdded( AppViewModel app )
        {
            if( Model.Apps.Count == 1 )
            {
                IsExpanded = true;
            }
        }

        private void OnExecuteConnect()
        {
            Model.StartMonitoring();
        }

        private void OnExecuteDisconnect()
        {
            Model.StopMonitoring();
        }
    }
}
