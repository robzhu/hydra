using System.ComponentModel;
using Marionette.Driver;
using RzAspects;
using RzWpf;

namespace Hydra.CommandCenter
{
    /// <summary>
    /// ViewModel for a page that visualizes where the hosts are.
    /// </summary>
    public class MapPageViewModel : ViewModelBase
    {
        public HostCollectionViewModel HostCollection { get; private set; }
        public ObservableCollectionEx<LocationResource> GeoPoints {get; private set;}

        public MapPageViewModel( HostCollectionViewModel hostCollection )
        {
            HostCollection = hostCollection;
            HostCollection.Hosts.ItemPropertyChanged += Hosts_ItemPropertyChanged;
            GeoPoints = new ObservableCollectionEx<LocationResource>();
        }

        private void Hosts_ItemPropertyChanged( HostViewModel subject, PropertyChangedEventArgs args )
        {
            if( ( args.PropertyName == "Location" ) && 
                ( subject.Model.Location != null ) )
            {
                GeoPoints.Add( subject.Model.Location );
            }
        }
    }
}