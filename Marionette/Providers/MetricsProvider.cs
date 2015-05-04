using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace Marionette
{
    public class MetricsProvider
    {
        static ManualResetEvent HasNetworkUsage = new ManualResetEvent( false );
        static NetworkUsage NetworkUsage { get; set; }
        static float CpuUsage { get; set; }
        static MemoryStatus MemoryUsage { get; set; }

        const int PollInterval = 1000;

        static MetricsProvider()
        {
            Task.Run( () => PollForMetricsAsync() );
        }

        private static async Task PollForMetricsAsync()
        {
            Counters counters = new Counters();
            while( true )
            {
                NetworkUsage = counters.GetNetworkUtilization();
                CpuUsage = (float)counters.GetCpuUsage();
                MemoryUsage = counters.GetMemoryUsage();
                HasNetworkUsage.Set();

                await Task.Delay( PollInterval );
            }
        }

        public MetricsResource GetUsage()
        {
            HasNetworkUsage.WaitOne();
            return new MetricsResource
            {
                Network = NetworkUsage,
                CpuUsagePercent = CpuUsage, 
                RamTotal = MemoryUsage.TotalPhysicalMB,
                RamUsed = MemoryUsage.UsedPhysicalMB,
            };
        }
    }
}
