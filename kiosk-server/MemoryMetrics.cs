using System.Diagnostics;
using System.Runtime.InteropServices;

namespace kiosk_server
{
    public class MemoryMetrics
    {
        public double Total;
        public double Used;
        public double Free;
    }

    // copied from https://gunnarpeipman.com/dotnet-core-system-memory/

    public class MemoryMetricsClient
    {
        public MemoryMetrics GetMetrics()
        {
            if (IsUnix())
            {
                return GetUnixMetrics();
            }

            return GetWindowsMetrics();
        }

        public bool IsUnix()
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

                metrics.Total = Math.Round(double.Parse(totalMemoryParts[1]) / 1024, 0);
                metrics.Free = Math.Round(double.Parse(freeMemoryParts[1]) / 1024, 0);
                metrics.Used = metrics.Total - metrics.Free;
            }

            return metrics;
        }

        private MemoryMetrics GetUnixMetrics()
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
                Console.WriteLine(output);
            }

            var metrics = new MemoryMetrics();

            var lines = output?.Split("\n");
            if (lines != null)
            {
                var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

                metrics.Total = double.Parse(memory[1]);
                metrics.Used = double.Parse(memory[2]);
                metrics.Free = double.Parse(memory[3]);

            }

            return metrics;
        }
    }
}
