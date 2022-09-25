using kiosk_server.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using static MudBlazor.Colors;
using System.Text.Json;
using kiosk_server.Shared;
using Microsoft.AspNetCore.Components.Web;

namespace kiosk_server.Pages
{
    public partial class Setup
    {
        [CascadingParameter] public MainLayout Layout { get; set; } = default!;

        private readonly SetupModel setupModel = new();

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
            Layout.Title = "Kiosk Server Setup";

            await base.OnInitializedAsync();

 
        }


        private static async Task HandleOnChange(string  url)
        {
            var path = System.IO.Path.Combine(System.AppContext.BaseDirectory, "appsettings.json");

           var configJson = await File.ReadAllTextAsync(path);
           var config = JsonSerializer.Deserialize<Dictionary<string, object>>(configJson);

           if (config != null)
           {
               config["RedirectUrl"] = url;

               Program.ConfigurationRoot["RedirectUrl"] = url;

               var updatedConfigJson =
                   JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
               await File.WriteAllTextAsync(path, updatedConfigJson);
           }
        }


        private static void HandleReboot()
        {

            System.Diagnostics.Process.Start(new ProcessStartInfo() { FileName = "sudo", Arguments = "reboot now" });

           
        }

        private static void HandleShutdown()
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo() { FileName = "sudo", Arguments = "shutdown now" });
        }
    }
}
