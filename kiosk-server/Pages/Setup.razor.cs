using kiosk_server.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace kiosk_server.Pages
{
    public partial class Setup
    {


        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        [Inject] private ProtectedLocalStorage ProtectedLocalStorage { get; set; } = default!;

        private SetupModel setupModel = new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                setupModel.Url = (await ProtectedLocalStorage.GetAsync<string>("RedirectUrl")).Value;

                StateHasChanged();
            }


        }


        protected override async Task OnInitializedAsync()
        {

     
            await base.OnInitializedAsync();
        }



        async Task HandleValidSubmit()
        {


            await ProtectedLocalStorage.SetAsync("RedirectUrl", setupModel.Url ?? "");

        }

        private void HandleReboot()
        {

            System.Diagnostics.Process.Start(new ProcessStartInfo() { FileName = "sudo", Arguments = "reboot now" });

           
        }

        private void HandleShutdown()
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo() { FileName = "sudo", Arguments = "shutdown now" });
        }
    }
}
