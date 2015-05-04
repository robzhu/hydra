using System.Net.Http;
using Marionette.Driver;
using RzAspects;
using RzWpf;

namespace Hydra.CommandCenter
{
    public class AppViewModel : ModelBase
    {
        public string PropertyData { get { return "Data"; } }
        private AppResource _Data;
        public AppResource Data
        {
            get { return _Data; }
            set { SetProperty( PropertyData, ref _Data, value ); }
        }

        public string PropertyIsExpanded { get { return "IsExpanded"; } }
        private bool _IsExpanded = true;
        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set { SetProperty( PropertyIsExpanded, ref _IsExpanded, value ); }
        }

        private HttpClient Client = new HttpClient();

        public DelegatedCommand<ProcessResource> StopProcessCommand { get; private set; }
        public DelegatedCommand LaunchCommand { get; private set; }
        public DelegatedCommand LaunchWithParametersCommand { get; private set; }
        public DelegatedCommand RemoveCommand { get; private set; }

        public AppViewModel()
        {
            StopProcessCommand = new DelegatedCommand<ProcessResource>( OnExecuteStopProcess );
            LaunchCommand = new DelegatedCommand( () =>
                {
                    LaunchInstance( null );
                    IsExpanded = true;
                } );

            LaunchWithParametersCommand = new DelegatedCommand( OnExecuteLaunchWithParameters );

            RemoveCommand = new DelegatedCommand( () =>
                {
                    Client.DeleteAsync( Data.Href );
                } );
        }

        private async void OnExecuteLaunchWithParameters()
        {
            var parameters = await Navigation.ShowInputAsync( "launch parameters", "enter launch parameters", Settings.Instance.LastParameterInput );
            if( !string.IsNullOrEmpty( parameters ) )
            {
                LaunchInstance( parameters );
                Settings.Instance.LastParameterInput = parameters;
                IsExpanded = true;
            }
        }

        private async void LaunchInstance( string parameters )
        {
            var url = Data.LaunchInstance.Href;
            if( !string.IsNullOrEmpty( parameters ) )
            {
                url += ( "&parameters=" + parameters );
            }
            await Client.PostAsync( url, null );
        }

        private void OnExecuteStopProcess( ProcessResource process )
        {
            Client.DeleteAsync( process.Href );
        }

        internal void CopyFrom( AppResource newData )
        {
            Data = newData;
        }
    }
}
