using kiosk_server.Metrics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using kiosk_server.Services;

namespace kiosk_server.Api
{
    [ApiController]
    [AllowAnonymous]
    public class ApiController(MyEventService myEventService) : ControllerBase
    {
        private class StatusData
        {
            public DiskMetrics Disk { get; set; } = null!;
            public TemperatureMetrics Temperature { get; set; } = null!;
            public MemoryMetrics Memory { get; set; } = null!;
            public CpuMetrics Cpu { get; set; } = null!;
        }

        [Route("api/status")]
        [HttpGet]
        public IActionResult Get()
        {
            var statusData = new StatusData
            {
                Memory = MemoryMetricsClient.GetMetrics(),
                Temperature = TemperatureMetricsClient.GetMetrics(),
                Disk = DiskMetricsClient.GetMetrics(),
                Cpu = CpuMetricsClient.GetMetrics()
            };
            return Ok(statusData);
        }

        [Route("api/shutdown")]
        [HttpPost]
        public IActionResult Shutdown()
        {
            Process.Start(new ProcessStartInfo { FileName = "sudo", Arguments = "shutdown now" });

            return Ok();
        }

        [Route("api/reboot")]

        [HttpPost]
        public IActionResult Reboot()
        {
            Process.Start(new ProcessStartInfo { FileName = "sudo", Arguments = "reboot now" });

            return Ok();
        }


        [Route("api/screenon")] // Pi 4 X11
        [HttpPost]
        public IActionResult ScreenOn()
        {
            Process.Start(new ProcessStartInfo { FileName = "sudo", Arguments = "vcgencmd display_power 1" });

            return Ok();
        }

        [Route("api/screenoff")] // Pi 4 X11

        [HttpPost]
        public IActionResult ScreenOff()
        {
            Process.Start(new ProcessStartInfo { FileName = "sudo", Arguments = "vcgencmd display_power 0" });

            return Ok();
        }

        [Route("api/screenon2")] // Pi 5 labwc
        [HttpPost]
        public IActionResult ScreenOn2()
        {
            Process.Start(new ProcessStartInfo { FileName = "/usr/bin/bash", Arguments = "-c \"export WAYLAND_DISPLAY=wayland-0 ; export XDG_RUNTIME_DIR=/run/user/1000 ; /usr/bin/wlr-randr --output HDMI-A-1 --on \"" })?.WaitForExit();

            return Ok();
        }

        [Route("api/screenoff2")] // Pi 5 labwc

        [HttpPost]
        public IActionResult ScreenOff2()
        {
            Process.Start(new ProcessStartInfo { FileName = "/usr/bin/bash", Arguments = "-c \"export WAYLAND_DISPLAY=wayland-0 ; export XDG_RUNTIME_DIR=/run/user/1000 ; /usr/bin/wlr-randr --output HDMI-A-1 --off \"" })?.WaitForExit(); 

            return Ok();
        }

        [Route("api/screenon3")] // Pi 5 wayfire
        [HttpPost]
        public IActionResult ScreenOn3()
        {
            Process.Start(new ProcessStartInfo { FileName = "/usr/bin/bash", Arguments = "-c \"export WAYLAND_DISPLAY=wayland-1 ; export XDG_RUNTIME_DIR=/run/user/1000 ; /usr/bin/wlr-randr --output HDMI-A-1 --on; sleep 5; wtype -P F11 \"" })?.WaitForExit();

            return Ok();
        }

        [Route("api/screenoff3")] // Pi 5 wayfire

        [HttpPost]
        public IActionResult ScreenOff3()
        {
            Process.Start(new ProcessStartInfo { FileName = "/usr/bin/bash", Arguments = "-c \"export WAYLAND_DISPLAY=wayland-1 ; export XDG_RUNTIME_DIR=/run/user/1000 ; /usr/bin/wlr-randr --output HDMI-A-1 --off \"" })?.WaitForExit();

            return Ok();
        }

        [Route("api/stopchromium")] 

        [HttpPost]
        public IActionResult StopChromium()
        {
            Process.Start(new ProcessStartInfo { FileName = "/usr/bin/bash", Arguments = "-c \"ps aux | awk '/chromium/ { print $2 } ' | xargs kill  \"" })?.WaitForExit();

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
