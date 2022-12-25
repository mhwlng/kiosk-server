using kiosk_server.Metrics;
using kiosk_server.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Xml.Linq;

namespace kiosk_server.Api
{
    [ApiController]
    [AllowAnonymous]
    public class ApiController : ControllerBase
    {
        private class StatusData
        {
            public DiskMetrics Disk { get; set; } = default!;
            public TemperatureMetrics Temperature { get; set; } = default!;
            public MemoryMetrics Memory { get; set; } = default!;
            public CpuMetrics Cpu { get; set; } = default!;
        }

        [Route("api/status")]
        [HttpGet]
        public IActionResult Get()
        {
            var statusData = new StatusData
            {
                Memory = new MemoryMetricsClient().GetMetrics(),
                Temperature = new TemperatureMetricsClient().GetMetrics(),
                Disk = new DiskMetricsClient().GetMetrics(),
                Cpu = new CpuMetricsClient().GetMetrics()
            };
            return Ok(statusData);
        }

        [Route("api/shutdown")]
        [HttpPost]
        public IActionResult Shutdown()
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo() { FileName = "sudo", Arguments = "shutdown now" });

            return Ok();
        }

        [Route("api/reboot")]

        [HttpPost]
        public IActionResult Reboot()
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo() { FileName = "sudo", Arguments = "reboot now" });

            return Ok();
        }
    }
}
