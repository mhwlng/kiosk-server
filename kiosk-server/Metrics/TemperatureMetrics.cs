﻿using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
#pragma warning disable IDE0057

namespace kiosk_server.Metrics
{
    // from https://github.com/VincentPestana/RpiStats

    public class TemperatureMetrics
    {
        public float CpuTemperature { get; set; }

        public string ThrottledState { get; set; } = null!;

    }

    public class TemperatureMetricsClient
    {
        public static TemperatureMetrics GetMetrics()
        {
            return IsLinux() ? GetLinuxMetrics() : GetWindowsMetrics();
        }

        private static bool IsLinux()
        {
            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                         RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            return isLinux;
        }

        private static TemperatureMetrics GetWindowsMetrics()
        {
            var metrics = new TemperatureMetrics();

            // todo ???

            /*
            Double CPUtprt = 0;
            System.Management.ManagementObjectSearcher mos = new System.Management.ManagementObjectSearcher(@"root\WMI", "Select * From MSAcpi_ThermalZoneTemperature");
            foreach (System.Management.ManagementObject mo in mos.Get())
            {
                CPUtprt = Convert.ToDouble(Convert.ToDouble(mo.GetPropertyValue("CurrentTemperature").ToString()) - 2732) / 10;
                Console.WriteLine("CPU temp : " + CPUtprt.ToString() + " °C");
            }*/

            return metrics;
        }

        private enum ThrottledState : long
        {
            UnderVoltageDetected = 0x1L,
            FrequencyCapped = 0x2L,
            CurrentlyThrottled = 0x4L,
            SoftTemperatureLimit = 0x8L,

            UnderVoltHasOccuredSinceBoot = 0x10000L,
            FrequencyCapHasOccured = 0x20000L,
            ThrottlingHasOccured = 0x40000L,
            SoftTemperatureLimitHasOccured = 0x80000L
        }
        /*
        private static string FormatThrottledState(string throttledStateCallOutput)
        {
            var enumState = Enum.Parse<ThrottledState>(throttledStateCallOutput);

            return enumState.ToString();
        }*/

        private static TemperatureMetrics GetLinuxMetrics()
        {
            var output = "";

            var info = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = "-c \"/usr/bin/vcgencmd measure_temp\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using (var process = Process.Start(info))
            {
                output = process?.StandardOutput.ReadToEnd();
            }

            var metrics = new TemperatureMetrics();

            if (!string.IsNullOrEmpty(output))
            {
                var temperatureOutput = output
                    .Substring(output.IndexOf('=') + 1,
                        output.IndexOf('\'', StringComparison.Ordinal) - (output.IndexOf('=') + 1));

                if (float.TryParse(temperatureOutput, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out var cpuTemp))
                {
                    metrics.CpuTemperature = cpuTemp;
                }
            }

            var info2 = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = "-c \"/usr/bin/vcgencmd get_throttled\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using (var process2 = Process.Start(info2))
            {
                output = process2?.StandardOutput.ReadToEnd();
            }

            if (!string.IsNullOrEmpty(output))
            {
                var throttledStateList = new List<string>();

                output = output.Trim();
                var throttledOutput = Convert.ToInt64(output.Substring(output.IndexOf("=0x", StringComparison.Ordinal) + 1), 16);

                if ((throttledOutput & (long)ThrottledState.UnderVoltageDetected) > 0)
                {
                    throttledStateList.Add("Undervoltage");
                }
                if ((throttledOutput & (long)ThrottledState.FrequencyCapped) > 0)
                {
                    throttledStateList.Add("FrequencyCapped");
                }
                if ((throttledOutput & (long)ThrottledState.CurrentlyThrottled) > 0)
                {
                    throttledStateList.Add("CurrentlyThrottled");
                }
                if ((throttledOutput & (long)ThrottledState.SoftTemperatureLimit) > 0)
                {
                    throttledStateList.Add("SoftTemperatureLimit");
                }
                if ((throttledOutput & (long)ThrottledState.UnderVoltHasOccuredSinceBoot) > 0)
                {
                    throttledStateList.Add("UnderVoltHasOccuredSinceBoot");
                }
                if ((throttledOutput & (long)ThrottledState.FrequencyCapHasOccured) > 0)
                {
                    throttledStateList.Add("FrequencyCapHasOccured");
                }
                if ((throttledOutput & (long)ThrottledState.ThrottlingHasOccured) > 0)
                {
                    throttledStateList.Add("ThrottlingHasOccured");
                }
                if ((throttledOutput & (long)ThrottledState.SoftTemperatureLimitHasOccured) > 0)
                {
                    throttledStateList.Add("SoftTemperatureLimitHasOccured");
                }

                metrics.ThrottledState = string.Join(", ", throttledStateList);
            }

            return metrics;


        }
    }
}
