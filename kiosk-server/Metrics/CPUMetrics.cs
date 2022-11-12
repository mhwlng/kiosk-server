using kiosk_server.Model;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace kiosk_server.Metrics
{
    public class CpuMetrics
    {
        public string OsDescription { get; set; } = default!;
        public string OsName { get; set; } = default!;

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

        public class RegExMatch
        {
            public Regex regex;
            public Action<string> updateValue;

            public RegExMatch(string pattern, Action<string> update)
            {
                regex = new Regex(pattern, RegexOptions.Compiled);
                updateValue = update;
            }
        }


        private void HandleRegExMatches(ref string[] lines,ref RegExMatch[] matches)
        {

            foreach (var line in lines)
            {
                foreach (var matchItem in matches)
                {
                    var match = matchItem.regex.Match(line);
                    if (match.Groups[0].Success)
                    {
                        var value = match.Groups[1].Value;
                        matchItem.updateValue(value);
                    }
                }
            }
        }

        private string GetLinuxOsName()
        {
            var prettyName = "";

            var releaseLines = File.ReadAllLines(@"/etc/os-release");

            RegExMatch[] releaseMatches =
            {
                new RegExMatch("^PRETTY_NAME+=\"(.+)\"", value => prettyName = value),
            };

            HandleRegExMatches(ref releaseLines, ref releaseMatches);
            
            return prettyName;
        }

        private CpuMetrics GetLinuxMetrics()
        {
            var metrics = new CpuMetrics();

            metrics.OsDescription = RuntimeInformation.OSDescription;

            metrics.OsName = GetLinuxOsName();

            var cpuInfoLines = File.ReadAllLines(@"/proc/cpuinfo");

            RegExMatch[] cpuInfoMatches =
            {
                 new RegExMatch(@"^Model\s+:\s+(.+)", value => metrics.CpuModel = value),
                 new RegExMatch(@"^model name\s+:\s+(.+)", value => metrics.CpuModelName = value),
                 new RegExMatch(@"^Hardware\s+:\s+(.+)", value => metrics.CpuHardware = value),
            };

            HandleRegExMatches(ref cpuInfoLines, ref cpuInfoMatches);


            return metrics;
        }
    }
}
