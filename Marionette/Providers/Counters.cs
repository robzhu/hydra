using System.Diagnostics;
using System.Linq;

namespace Marionette
{
    public struct NetworkUsage
    {
        public double SendMbps { get; set; }
        public double ReceiveMbps { get; set; }
        public double PercentUtilization { get; set; }
    }

    internal class Counters
    {
        private static MEMORYSTATUSEX GetMemoryStatus()
        {
            var memStatus = new MEMORYSTATUSEX();
            NativeMethods.GlobalMemoryStatusEx( memStatus );
            return memStatus;
        }

        public static string[] GetNetworkInterfaceNames()
        {
            PerformanceCounterCategory category = new PerformanceCounterCategory( "Network Interface" );
            return category.GetInstanceNames();
        }

        PerformanceCounter bandwidthCounter;
        PerformanceCounter dataSentCounter;
        PerformanceCounter dataReceivedCounter;
        PerformanceCounter cpuCounter;

        public Counters() : this ( GetNetworkInterfaceNames().FirstOrDefault() ) { }
        public Counters( string nicName )
        {
            bandwidthCounter = new PerformanceCounter( "Network Interface", "Current Bandwidth", nicName );
            dataSentCounter = new PerformanceCounter( "Network Interface", "Bytes Sent/sec", nicName );
            dataReceivedCounter = new PerformanceCounter( "Network Interface", "Bytes Received/sec", nicName );

            cpuCounter = new PerformanceCounter( "Processor", "% Processor Time", "_Total" );
        }

        public NetworkUsage GetNetworkUtilization()
        {
            const int numberOfIterations = 1;

            float bandwidth = bandwidthCounter.NextValue();
            float sendSum = 0;
            float receiveSum = 0;

            for( int index = 0; index < numberOfIterations; index++ )
            {
                sendSum += dataSentCounter.NextValue();
                receiveSum += dataReceivedCounter.NextValue();
            }

            float dataSent = sendSum;
            float dataReceived = receiveSum;

            double sentRate = dataSent / numberOfIterations;
            double utilization = ( 8 * ( dataSent + dataReceived ) ) / ( bandwidth * numberOfIterations ) * 100;
            //return utilization;

            double denom = numberOfIterations * 1000 * 1000;

            return new NetworkUsage
            {
                SendMbps = dataSent * 8 / denom,
                ReceiveMbps = dataReceived * 8 / denom,
                PercentUtilization = utilization,
            };
        }

        public double GetCpuUsage()
        {
            return cpuCounter.NextValue();
        }

        public MEMORYSTATUSEX GetMemoryUsage()
        {
            return GetMemoryStatus();
        }
    }
}
