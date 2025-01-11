using kiosk_server.Metrics;
using kiosk_server.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Xml.Linq;
using kiosk_server.Services;
using System.Collections.Generic;

namespace kiosk_server.Api
{
    [ApiController]
    [AllowAnonymous]
    public class ApiController(MyEventService myEventService) : ControllerBase
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


        [Route("api/screenon")]
        [HttpPost]
        public IActionResult ScreenOn()
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo() { FileName = "sudo", Arguments = "vcgencmd display_power 1" });

            return Ok();
        }

        [Route("api/screenoff")]

        [HttpPost]
        public IActionResult ScreenOff()
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo() { FileName = "sudo", Arguments = "vcgencmd display_power 0" });

            return Ok();
        }

        [Route("api/screenon2")]
        [HttpPost]
        public IActionResult ScreenOn2()
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo() { FileName = "/usr/bin/bash", Arguments = "-c \"export WAYLAND_DISPLAY=wayland-0 ; export XDG_RUNTIME_DIR=/run/user/1000 ; /usr/bin/wlr-randr --output HDMI-A-1 --on \"" })?.WaitForExit();

            return Ok();
        }

        [Route("api/screenoff2")]

        [HttpPost]
        public IActionResult ScreenOff2()
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo() { FileName = "/usr/bin/bash", Arguments = "-c \"export WAYLAND_DISPLAY=wayland-0 ; export XDG_RUNTIME_DIR=/run/user/1000 ; /usr/bin/wlr-randr --output HDMI-A-1 --off \"" })?.WaitForExit(); 

            return Ok();
        }
        [Route("api/navigatetourl")]
        [HttpPost]
        public IActionResult NavigateToUrl([FromQuery]string? url = null)
        {
            myEventService.NavigateToUrl(url);

            return Ok();
        }
    }
}
