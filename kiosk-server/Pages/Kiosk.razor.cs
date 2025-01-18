using kiosk_server.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using static MudBlazor.Colors;
using System.Text.Json;
using kiosk_server.Shared;
using Microsoft.AspNetCore.Components.Web;
using System.Runtime.InteropServices;
using System.IO;
using kiosk_server.Metrics;
using kiosk_server.Services;

namespace kiosk_server.Pages
{
    public partial class Kiosk
    {
        [Inject] private LayoutService LayoutService { get; set; } = null!;
        [Inject] private MyEventService EventService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        
        private List<RedirectItem> RedirectUrlList { get; set; } = default!;
        
        private string? CurrentIframeUrl;

        private string? TabHeaderClass;

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

            RedirectUrlList = Program.ConfigurationRoot.GetSection("RedirectUrl").Get<List<RedirectItem>>() ?? new List<RedirectItem>();
            
            await base.OnInitializedAsync();

        }

        public void Dispose()
        {
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
            System.Diagnostics.Process.Start(new ProcessStartInfo() { FileName = "sudo", Arguments = "shutdown now" });
        }

        private static void HandleStopChromium()
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo() { FileName = "/usr/bin/bash", Arguments = "-c \"ps aux | awk '/chromium/ { print $2 } ' | xargs kill  \"" })?.WaitForExit();
        }

        private static void HandleFullScreen()
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo() { FileName = "/usr/bin/bash", Arguments = "-c \"export WAYLAND_DISPLAY=wayland-1 ; export XDG_RUNTIME_DIR=/run/user/1000 ; wtype -P F11 \"" })?.WaitForExit();
        }
    }
}
