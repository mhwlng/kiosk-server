using kiosk_server.Model;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace kiosk_server
{
    public class DiskMetrics
    {
        public double TotalDiskSpace { get; set; }
        public double AvailableDiskSpace { get; set; }


    }

    public class DiskMetricsClient
    {
        public DiskMetrics GetMetrics()
        {
            if (IsLinux())
            {
                return GetLinuxMetrics();
            }

            return GetWindowsMetrics();
        }

        private bool IsLinux()
        {
            var isUnix = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                         RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            return isUnix;
        }

        private DiskMetrics GetWindowsMetrics()
        {
            var metrics = new DiskMetrics();

            var f = new FileInfo(System.AppContext.BaseDirectory);
            var drive = Path.GetPathRoot(f.FullName);

            var driveInfo = new DriveInfo(drive ?? "c:\\");
            metrics.AvailableDiskSpace = driveInfo.AvailableFreeSpace / Math.Pow(1024, 3);
            metrics.TotalDiskSpace = driveInfo.TotalSize / Math.Pow(1024, 3);

            return metrics;
        }

        private DiskMetrics GetLinuxMetrics()
        {
            var metrics = new DiskMetrics();

            var driveInfo = new DriveInfo("/");
            metrics.AvailableDiskSpace = driveInfo.AvailableFreeSpace / Math.Pow(1024, 3);
            metrics.TotalDiskSpace = driveInfo.TotalSize / Math.Pow(1024, 3);

            return metrics;


        }
    }
}
