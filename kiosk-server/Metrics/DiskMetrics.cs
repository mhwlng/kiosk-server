using System.Runtime.InteropServices;

namespace kiosk_server.Metrics
{
    public class DiskMetrics
    {
        public double TotalDiskSpace { get; set; }
        public double AvailableDiskSpace { get; set; }


    }

    public class DiskMetricsClient
    {
        public static DiskMetrics GetMetrics()
        {
            return IsLinux() ? GetLinuxMetrics() : GetWindowsMetrics();
        }

        private static bool IsLinux()
        {
            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                         RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            return isLinux;
        }

        private static DiskMetrics GetWindowsMetrics()
        {
            var metrics = new DiskMetrics();

            var f = new FileInfo(AppContext.BaseDirectory);
            var drive = Path.GetPathRoot(f.FullName);

            var driveInfo = new DriveInfo(drive ?? "c:\\");
            metrics.AvailableDiskSpace = driveInfo.AvailableFreeSpace / Math.Pow(1024, 3);
            metrics.TotalDiskSpace = driveInfo.TotalSize / Math.Pow(1024, 3);

            return metrics;
        }

        private static DiskMetrics GetLinuxMetrics()
        {
            var metrics = new DiskMetrics();

            var driveInfo = new DriveInfo("/");
            metrics.AvailableDiskSpace = driveInfo.AvailableFreeSpace / Math.Pow(1024, 3);
            metrics.TotalDiskSpace = driveInfo.TotalSize / Math.Pow(1024, 3);

            return metrics;


        }
    }
}
