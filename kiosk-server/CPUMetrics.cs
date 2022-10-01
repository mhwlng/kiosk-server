using kiosk_server.Model;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace kiosk_server
{
    public class CpuMetrics
    {

        public string OsDescription { get; set; } = default!;
        public string CpuModel { get; set; } = default!;
        public string CpuModelName { get; set; } = default!;
        public string CpuHardware { get; set; } = default!;
    }

    public class CpuMetricsClient
    {
        public CpuMetrics GetMetrics()
        {
            if (IsLinux())
            {
                return GetLinuxMetrics();
            }

            return GetWindowsMetrics();
        }

        private bool IsLinux()
        {
            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                         RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            return isLinux;
        }

        private CpuMetrics GetWindowsMetrics()
        {
            var metrics = new CpuMetrics();

            metrics.OsDescription = RuntimeInformation.OSDescription;

            return metrics;
        }

        public class CpuInfoMatch
        {
            public Regex regex;
            public Action<string> updateValue;

            public CpuInfoMatch(string pattern, Action<string> update)
            {
                this.regex = new Regex(pattern, RegexOptions.Compiled);
                this.updateValue = update;
            }
        }
        private CpuMetrics GetLinuxMetrics()
        {
            var metrics = new CpuMetrics();

            metrics.OsDescription = RuntimeInformation.OSDescription;

            string[] cpuInfoLines = File.ReadAllLines(@"/proc/cpuinfo");

            CpuInfoMatch[] cpuInfoMatches =
            {
                 new CpuInfoMatch(@"^Model\s+:\s+(.+)", value => metrics.CpuModel = value),
                 new CpuInfoMatch(@"^model name\s+:\s+(.+)", value => metrics.CpuModelName = value),
                 new CpuInfoMatch(@"^Hardware\s+:\s+(.+)", value => metrics.CpuHardware = value),
            };

            foreach (string cpuInfoLine in cpuInfoLines)
            {
                foreach (CpuInfoMatch cpuInfoMatch in cpuInfoMatches)
                {
                    var match = cpuInfoMatch.regex.Match(cpuInfoLine);
                    if (match.Groups[0].Success)
                    {
                        string value = match.Groups[1].Value;
                        cpuInfoMatch.updateValue(value);
                    }
                }
            }

            return metrics;
        }
    }
}
