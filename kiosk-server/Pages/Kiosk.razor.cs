using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using kiosk_server.Services;

namespace kiosk_server.Pages
{
    public partial class Kiosk
    {
        [Inject] private LayoutService LayoutService { get; set; } = null!;
        [Inject] private MyEventService EventService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        private readonly System.Timers.Timer _timer = new System.Timers.Timer();

        private List<RedirectItem> RedirectUrlList { get; set; } = null!;
        
        private string? CurrentIframeUrl;

        private string? TabHeaderClass;

        private string? CurrentDateTime;

        private async Task UpdateClock()
        {
            //currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            CurrentDateTime = DateTime.Now.ToString("HH:mm");
            await InvokeAsync(StateHasChanged);
        }

        private void OnTimerTick(object? sender, System.Timers.ElapsedEventArgs e)
        {
            InvokeAsync(UpdateClock);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                CurrentIframeUrl = RedirectUrlList.FirstOrDefault()?.Url ?? "";

                StateHasChanged();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            EventService.OnUrlChange += NavigateToUrl;

            RedirectUrlList = Program.ConfigurationRoot.GetSection("RedirectUrl").Get<List<RedirectItem>>() ?? [];

            _timer.Interval = 60000;
            _timer.Elapsed += OnTimerTick;
            _timer.Start();
            await UpdateClock();

            await base.OnInitializedAsync();

        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Dispose();

            EventService.OnUrlChange -= NavigateToUrl;

        }

        private void NavigateToUrl(string? url)
        {
            InvokeAsync(() =>
            {
                if (string.IsNullOrEmpty(url))
                {
                    NavigationManager.NavigateTo(NavigationManager.Uri, true);
                }
                else
                {
                    CurrentIframeUrl = url;
                    TabHeaderClass = "hideme";
                    StateHasChanged();
                }

            });
        }

        private void ActivePanelIndexChanged(int index)
        {
            CurrentIframeUrl = RedirectUrlList[index].Url;
            
            StateHasChanged();
        }

        private static void HandleShutdown()
        {
            Process.Start(new ProcessStartInfo { FileName = "sudo", Arguments = "shutdown now" });
        }

        private static void HandleStopChromium()
        {
            Process.Start(new ProcessStartInfo { FileName = "/usr/bin/bash", Arguments = "-c \"ps aux | awk '/chromium/ { print $2 } ' | xargs kill  \"" })?.WaitForExit();
        }

        private static void HandleFullScreen() 
        {
            //Process.Start(new ProcessStartInfo { FileName = "/usr/bin/bash", Arguments = "-c \"export WAYLAND_DISPLAY=wayland-1 ; export XDG_RUNTIME_DIR=/run/user/1000 ; wtype -P F11 \"" })?.WaitForExit(); // wayfire

            Process.Start(new ProcessStartInfo { FileName = "/usr/bin/bash", Arguments = "-c \"export WAYLAND_DISPLAY=wayland-0 ; export XDG_RUNTIME_DIR=/run/user/1000 ; wtype -P F11 \"" })?.WaitForExit(); // labwc
        }
    }
}
