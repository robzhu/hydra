
namespace Marionette.Driver
{
    public struct NetworkUsage
    {
        public double SendMbps { get; set; }
        public double ReceiveMbps { get; set; }
        public double PercentUtilization { get; set; }
    }

    public class ResourceUsageSample
    {
        public NetworkUsage Network { get; set; }
        public float CpuUsagePercent { get; set; }
        public float RamTotal { get; set; }
        public float RamUsed { get; set; }

        public float RamUsedPercent
        {
            get
            {
                return RamUsed / RamTotal * 100;
            }
        }
    }
}
