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

        public double CpuUsage { get; set; } = default!;

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

        private static bool IsLinux()
        {
            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                         RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            return isLinux;
        }

        private static CpuMetrics GetWindowsMetrics()
        {
            var metrics = new CpuMetrics
            {
                OsDescription = RuntimeInformation.OSDescription
            };

            return metrics;
        }

        public class RegExMatch
        {
            public Regex regex;
            public Action<string> updateValue;

            private RegExMatch(string pattern, Action<string> update)
            {
                regex = new Regex(pattern, RegexOptions.Compiled);
                updateValue = update;
            }

            public static RegExMatch CreateInstance(string pattern, Action<string> update)
            {
                return new RegExMatch(pattern, update);
            }
        }


        private static void HandleRegExMatches(ref string[] lines,ref RegExMatch[] matches)
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

        /*private static string GetLinuxOsName()
        {
            var prettyName = "";

            var releaseLines = File.ReadAllLines(@"/etc/os-release");

            var releaseMatches = new[] {
                RegExMatch.CreateInstance("^PRETTY_NAME+=\"(.+)\"", value => prettyName = value),
            };

            HandleRegExMatches(ref releaseLines, ref releaseMatches);
            
            return prettyName;
        }*/

        // https://github.com/MhyrAskri/Linux-CPU-Usage/blob/master/CpuUsage.cs
        private static double GetLinuxCpuUsage()
        {
            var output = "";

            var info = new ProcessStartInfo("top -b -n 1")
            {
                FileName = "/bin/bash",
                Arguments = "-c \"top -b -n 1\"",
                RedirectStandardOutput = true
            };

            using (var process = Process.Start(info))
            {
                output = process?.StandardOutput.ReadToEnd();
            }

            var lines = output?.Split("\n");
            if (lines != null)
            {
                var cpuLine2 = lines[2].Split(",", StringSplitOptions.RemoveEmptyEntries);
                var firstPart = cpuLine2[0].Split(":", StringSplitOptions.RemoveEmptyEntries);
                var secondPart = cpuLine2[1].Split("s", StringSplitOptions.RemoveEmptyEntries);
                var thirdPart = cpuLine2[2].Split("n", StringSplitOptions.RemoveEmptyEntries);

                var cpuUsage = double.Parse(firstPart[1].Split("u", StringSplitOptions.RemoveEmptyEntries)[0]) +
                                  double.Parse(secondPart[0]) +
                                  double.Parse(thirdPart[0]);

                return cpuUsage;

            }

            return 0;
        }

        private static CpuMetrics GetLinuxMetrics()
        {
            var metrics = new CpuMetrics
            {
                OsDescription = RuntimeInformation.OSDescription
                //OsName = GetLinuxOsName()
            };

            var cpuInfoLines = File.ReadAllLines(@"/proc/cpuinfo");

            var cpuInfoMatches = new[] {
                RegExMatch.CreateInstance(@"^Model\s+:\s+(.+)", value => metrics.CpuModel = value),
                //RegExMatch.CreateInstance(@"^model name\s+:\s+(.+)", value => metrics.CpuModelName = value),
                RegExMatch.CreateInstance(@"^Hardware\s+:\s+(.+)", value => metrics.CpuHardware = value),
            };

            HandleRegExMatches(ref cpuInfoLines, ref cpuInfoMatches);

            var output = "";

            var info = new ProcessStartInfo("lscpu")
            {
                FileName = "/bin/bash",
                Arguments = "-c \"lscpu\"",
                RedirectStandardOutput = true
            };

            using (var process = Process.Start(info))
            {
                output = process?.StandardOutput.ReadToEnd();
            }

            var lscpuLines = output?.Split("\n") ;

            if (lscpuLines != null)
            {
                var lscpuMatches = new[]  {
                        RegExMatch.CreateInstance(@"^Model name:\s+(.+)", value => metrics.CpuModelName = value)
                    };

                HandleRegExMatches(ref lscpuLines, ref lscpuMatches);

            }

            metrics.CpuUsage = GetLinuxCpuUsage();
            
            return metrics;
        }
    }
}
