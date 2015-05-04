using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Marionette.Driver;
using Rz.Http;
using RzAspects;

namespace Hydra.CommandCenter
{
    /// <summary>
    /// Model for a remote machine
    /// </summary>
    public class Host : ModelBase
    {
        const int SamplePeriod = 1000;
        public string Name { get; private set; }

        public string PropertyConnected { get { return "Connected"; } }
        private bool _Connected;
        public bool Connected
        {
            get { return _Connected; }
            set { SetProperty( PropertyConnected, ref _Connected, value ); }
        }

        public string PropertyLocation { get { return "Location"; } }
        private LocationResource _Location;
        public LocationResource Location
        {
            get { return _Location; }
            private set { SetProperty( PropertyLocation, ref _Location, value ); }
        }
        
        public ResourceUsageSampleCollection ResourceUsage { get; private set; }
        public ObservableCollectionEx<AppViewModel> Apps { get; private set; }
        public ObservableCollectionEx<ClientMetricsViewModel> Clients { get; private set; }

        private bool _monitorUsage = false;
        private Client Client;

        public Host( string name )
        {
            Name = name;
            Client = new Client( name );
            ResourceUsage = new ResourceUsageSampleCollection();
            ResourceUsage.Initialize( 60 );
            Apps = new ObservableCollectionEx<AppViewModel>();
            Clients = new ObservableCollectionEx<ClientMetricsViewModel>();
        }

        public async Task<bool> IsReachableAsync()
        {
            try
            {
                await Client.EnsureServiceAvailableAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void StartMonitoring()
        {
            _monitorUsage = true;

            Task.Run( () => { return MonitorUsageAsync(); } );
        }

        public async Task RefreshLocationAsync()
        {
            var location = await Client.GetHostLocationAsync();

            UIThread.Marshall( () =>
            {
                Location = location;
            } );
        }

        private async Task MonitorUsageAsync()
        {
            await RefreshLocationAsync();
            Stopwatch sw = new Stopwatch();
            while( _monitorUsage )
            {
                sw.Reset();
                sw.Start();
                try
                {
                    ResourceUsageSample sample = await Client.GetResourceUsageAsync();
                    ResourceCollection<AppResource> apps = await Client.GetAppsAsync( "processes" );
                    ResourceCollection<ClientStatsResource> clientMetrics = await Client.GetAllClientStatsAsync();

                    UIThread.Marshall( () => 
                    {
                        ResourceUsage.AddDataPointButKeepSetSize( sample );
                        SynchronizeApps( apps.Items.ToList() );
                        SynchronizeClientMetrics( clientMetrics.Items.ToList() );
                    } );
                }
                catch( Exception ex )
                {
                }

                sw.Stop();
                if( sw.ElapsedMilliseconds > SamplePeriod )
                {
                    continue;
                }
                else
                {
                    await Task.Delay( SamplePeriod - (int)sw.ElapsedMilliseconds );
                }
            }
        }

        private void SynchronizeApps( List<AppResource> apps )
        {
            List<AppViewModel> toRemove = new List<AppViewModel>();
            foreach( var appModel in Apps )
            {
                var match = apps.FirstOrDefault( app => app.Id == appModel.Data.Id );
                if( match == null )
                {
                    toRemove.Add( appModel );
                    continue;
                }
                //copy state of match to currentApp
                appModel.CopyFrom( match );
                apps.Remove( match );
            }

            //any entries remaining in apps are new
            foreach( var newApp in apps )
            {
                Apps.Add( new AppViewModel { Data = newApp } );
            }

            foreach( var item in toRemove )
            {
                Apps.Remove( item );
            }
        }

        private void SynchronizeClientMetrics( List<ClientStatsResource> stats )
        {
            List<ClientMetricsViewModel> toRemove = new List<ClientMetricsViewModel>();
            foreach( var clientViewModel in Clients )
            {
                var match = stats.FirstOrDefault( stat => stat.Name == clientViewModel.Data.Name );
                if( match == null )
                {
                    toRemove.Add( clientViewModel );
                    continue;
                }
                
                clientViewModel.CopyFrom( match );
                stats.Remove( match );
            }

            foreach( var newClientMetrics in stats )
            {
                Clients.Add( new ClientMetricsViewModel { Data = newClientMetrics } );
            }

            foreach( var item in toRemove )
            {
                Clients.Remove( item );
            }
        }

        public void StopMonitoring()
        {
            _monitorUsage = false;
        }
    }
}
