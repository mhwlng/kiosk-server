﻿using kiosk_server.Model;
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

namespace kiosk_server.Pages
{
    public partial class Kiosk
    {

        private List<RedirectItem> RedirectUrlList { get; set; } = default!;
        
        private string? CurrentIframeUrl;

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
            RedirectUrlList = Program.ConfigurationRoot.GetSection("RedirectUrl").Get<List<RedirectItem>>();

            await base.OnInitializedAsync();

        }


        private void ActivePanelIndexChanged(int index)
        {
            CurrentIframeUrl = RedirectUrlList[index].Url;
            
            StateHasChanged();
        }



    }
}