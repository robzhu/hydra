using Rz.Http;

namespace Marionette
{
    public class MetricsResource : Resource
    {
        public float CpuUsagePercent { get; set; }
        public float RamTotal { get; set; }
        public float RamUsed { get; set; }
        public NetworkUsage Network { get; set; }
    }
}
