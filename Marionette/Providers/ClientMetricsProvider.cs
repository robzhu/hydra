using System.Collections.Generic;
using Hydra.Shared;
using Marionette.Driver;

namespace Marionette
{
    public class ClientMetricsProvider
    {
        public static ClientMetricsProvider Instance { get; private set; }

        static ClientMetricsProvider()
        {
            Instance = new ClientMetricsProvider();
        }

        private Dictionary<int, ClientStatsResource> _stats = new Dictionary<int, ClientStatsResource>();
        private AppProvider _appProvider;

        private ClientMetricsProvider() 
        {
            _appProvider = AppProvider.Instance;
        }

        internal ClientStatsResource GetById( int id )
        {
            Refresh();
            if( _stats.ContainsKey( id ) )
            {
                return _stats[ id ];
            }
            return null;
        }

        internal void CreateOrUpdateById( ClientStatsResource metrics )
        {
            _stats[ metrics.Id ] = metrics;
        }

        internal IEnumerable<ClientStatsResource> GetAll()
        {
            Refresh();
            return _stats.Values;
        }

        List<int> _pidsToRemove = new List<int>();
        private void Refresh()
        {
            _pidsToRemove.Clear();
            foreach( var pid in _stats.Keys )
            {
                var process = _appProvider.GetProcessById( pid );
                if( process == null )
                {
                    _pidsToRemove.Add( pid );
                }
            }

            foreach( int toRemove in _pidsToRemove )
            {
                _stats.Remove( toRemove );
            }
        }
    }
}
