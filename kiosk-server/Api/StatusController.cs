using kiosk_server.Metrics;
using kiosk_server.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace kiosk_server.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class StatusController : ControllerBase
    {
        private class StatusData
        {
            public DiskMetrics Disk { get; set; } = default!;
            public TemperatureMetrics Temperature { get; set; } = default!;
            public MemoryMetrics Memory { get; set; } = default!;
            public CpuMetrics Cpu { get; set; } = default!;
        }

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
    }
}
