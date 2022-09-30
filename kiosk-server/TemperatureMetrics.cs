using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace kiosk_server
{
    public class TemperatureMetrics
    {
        public float CpuTemperature;
      
    }

    // copied from https://gunnarpeipman.com/dotnet-core-system-Temperature/

    public class TemperatureMetricsClient
    {
        public TemperatureMetrics GetMetrics()
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

        private TemperatureMetrics GetWindowsMetrics()
        {
            var metrics = new TemperatureMetrics();

            // todo

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

        private TemperatureMetrics GetLinuxMetrics()
        {
            var output = "";

            var info = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"/opt/vc/bin/vcgencmd measure_temp\"",
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
                        output.IndexOf("'", StringComparison.Ordinal) - (output.IndexOf('=') + 1));
                    
                var ok = float.TryParse(temperatureOutput, NumberStyles.Number ,CultureInfo.CreateSpecificCulture("en-US"), out metrics.CpuTemperature);
            }

            return metrics;


        }
    }
}
