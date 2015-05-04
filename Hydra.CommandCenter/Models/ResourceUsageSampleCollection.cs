using System;
using Marionette.Driver;
using RzAspects;

namespace Hydra.CommandCenter
{
    public class ResourceUsageSampleCollection : ObservableCollectionEx<ChartDataPoint<ResourceUsageSample>>
    {
        public void Initialize( int count )
        {
            for( int i = 0; i < count; i++ )
            {
                AddDataPoint( null );
            }
        }

        public void AddDataPoint( ResourceUsageSample sample )
        {
            Add( new ChartDataPoint<ResourceUsageSample>
            {
                Value = sample,
                Label = DateTime.Now.ToString( "mm:ss" ),
            } );
        }

        public void AddDataPointButKeepSetSize( ResourceUsageSample sample )
        {
            if( Count > 1 )
            {
                RemoveAt( 0 );
            }
            AddDataPoint( sample );
        }
    }
}
