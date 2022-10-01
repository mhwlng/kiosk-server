using System.Diagnostics;
using System.Runtime.InteropServices;

namespace kiosk_server
{
    public class MemoryMetrics
    {
        public double TotalMemory { get; set; }
        public double UsedMemory { get; set; }
        public double FreeMemory { get; set; }
    }

    // copied from https://gunnarpeipman.com/dotnet-core-system-memory/

    public class MemoryMetricsClient
    {
        public MemoryMetrics GetMetrics()
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

        private MemoryMetrics GetWindowsMetrics()
        {
            var output = "";

            var info = new ProcessStartInfo
            {
                FileName = "wmic",
                Arguments = "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value",
                RedirectStandardOutput = true
            };

            using (var process = Process.Start(info))
            {
                output = process?.StandardOutput.ReadToEnd();
            }

            var metrics = new MemoryMetrics();

            var lines = output?.Trim().Split("\n");
            if (lines != null)
            {
                var freeMemoryParts = lines[0].Split("=", StringSplitOptions.RemoveEmptyEntries);
                var totalMemoryParts = lines[1].Split("=", StringSplitOptions.RemoveEmptyEntries);

                metrics.TotalMemory = Math.Round(double.Parse(totalMemoryParts[1]) / 1024, 0);
                metrics.FreeMemory = Math.Round(double.Parse(freeMemoryParts[1]) / 1024, 0);
                metrics.UsedMemory = metrics.TotalMemory - metrics.FreeMemory;
            }

            return metrics;
        }

        private MemoryMetrics GetLinuxMetrics()
        {
            var output = "";

            var info = new ProcessStartInfo("free -m")
            {
                FileName = "/bin/bash",
                Arguments = "-c \"free -m\"",
                RedirectStandardOutput = true
            };

            using (var process = Process.Start(info))
            {
                output = process?.StandardOutput.ReadToEnd();
            }

            var metrics = new MemoryMetrics();

            var lines = output?.Split("\n");
            if (lines != null)
            {
                var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

                metrics.TotalMemory = double.Parse(memory[1]);
                metrics.UsedMemory = double.Parse(memory[2]);
                metrics.FreeMemory = double.Parse(memory[3]);

            }

            return metrics;
        }
    }
}
