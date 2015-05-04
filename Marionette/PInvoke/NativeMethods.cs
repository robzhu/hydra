using System.Runtime.InteropServices;

namespace Marionette
{
    public class MemoryStatus
    {
        private const int BytesPerMB = 1024 * 1024;

        public float AvailablePhysicalMB { get; set; }
        public float TotalPhysicalMB { get; set; }
        public float UsedPhysicalMB { get; set; }

        public static implicit operator MemoryStatus( MEMORYSTATUSEX status )
        {
            return new MemoryStatus()
            {
                AvailablePhysicalMB = status.ullAvailPhys / BytesPerMB,
                TotalPhysicalMB = status.ullTotalPhys / BytesPerMB,
                UsedPhysicalMB = ( status.ullTotalPhys - status.ullAvailPhys ) / BytesPerMB,
            };
        }
    }

    [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto )]
    public class MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;
        public MEMORYSTATUSEX()
        {
            this.dwLength = (uint)Marshal.SizeOf( typeof( MEMORYSTATUSEX ) );
        }
    }

    public static class NativeMethods
    {
        [return: MarshalAs( UnmanagedType.Bool )]
        [DllImport( "kernel32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        public static extern bool GlobalMemoryStatusEx( [In, Out] MEMORYSTATUSEX lpBuffer );
    }
}
