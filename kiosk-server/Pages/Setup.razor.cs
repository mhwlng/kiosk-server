using kiosk_server.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using static MudBlazor.Colors;
using System.Text.Json;

namespace kiosk_server.Pages
{
    public partial class Setup
    {


        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        private SetupModel setupModel = new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                var redirectUrl = Program.ConfigurationRoot.GetValue<string>("RedirectUrl");

                setupModel.Url = redirectUrl;

                StateHasChanged();
            }
        }


        protected override async Task OnInitializedAsync()
        {

     
            await base.OnInitializedAsync();
        }

     

        async Task HandleValidSubmit()
        {

            var path = System.IO.Path.Combine(System.AppContext.BaseDirectory, "appsettings.json");

            var configJson = await File.ReadAllTextAsync(path);
            var config = JsonSerializer.Deserialize<Dictionary<string, object>>(configJson);

            config["RedirectUrl"] = setupModel.Url ?? "";
            Program.ConfigurationRoot["RedirectUrl"] = setupModel.Url ?? "";

            var updatedConfigJson = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(path, updatedConfigJson);
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
