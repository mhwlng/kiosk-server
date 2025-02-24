﻿using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using kiosk_server.Services;

namespace kiosk_server.Pages
{
    public partial class Kiosk
    {
        [Inject] private LayoutService LayoutService { get; set; } = null!;
        [Inject] private MyEventService EventService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        
        private List<RedirectItem> RedirectUrlList { get; set; } = null!;
        
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

            RedirectUrlList = Program.ConfigurationRoot.GetSection("RedirectUrl").Get<List<RedirectItem>>() ?? [];
            
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
            Process.Start(new ProcessStartInfo { FileName = "sudo", Arguments = "shutdown now" });
        }

        private static void HandleStopChromium()
        {
            Process.Start(new ProcessStartInfo { FileName = "/usr/bin/bash", Arguments = "-c \"ps aux | awk '/chromium/ { print $2 } ' | xargs kill  \"" })?.WaitForExit();
        }

        private static void HandleFullScreen()
        {
            Process.Start(new ProcessStartInfo { FileName = "/usr/bin/bash", Arguments = "-c \"export WAYLAND_DISPLAY=wayland-1 ; export XDG_RUNTIME_DIR=/run/user/1000 ; wtype -P F11 \"" })?.WaitForExit();
        }
    }
}
