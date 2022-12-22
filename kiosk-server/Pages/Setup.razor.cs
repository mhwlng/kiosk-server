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
using static kiosk_server.Pages.Index;
using static MudBlazor.CategoryTypes;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using MudBlazor;
using kiosk_server.Services;

namespace kiosk_server.Pages
{
    public partial class Setup
    {
        [Inject] private LayoutService LayoutService { get; set; } = null!;

        [CascadingParameter] public MainLayout Layout { get; set; } = default!;

        private SetupModel SetupModel = new();

        private List<RedirectItem> RedirectUrlList { get; set; } = default!;

        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {

                //StateHasChanged();
            }
        }

        private void RenumberRedirectUrlListIndexes()
        {
            for (var index = 0; index < RedirectUrlList.Count; index++)
            {
                RedirectUrlList[index].Id = index + 1;
            }
        }
        
        protected override async Task OnInitializedAsync()
        {
            Layout.Title = "Kiosk Server Setup";

            var memoryMetricsClient = new MemoryMetricsClient();
            SetupModel.MemoryMetrics = memoryMetricsClient.GetMetrics();

            var temperatureMetricsClient = new TemperatureMetricsClient();
            SetupModel.TemperatureMetrics = temperatureMetricsClient.GetMetrics();

            var diskMetricsClient = new DiskMetricsClient();
            SetupModel.DiskMetrics = diskMetricsClient.GetMetrics();

            var cpuMetricsClient = new CpuMetricsClient();
            SetupModel.CpuMetrics = cpuMetricsClient.GetMetrics();

            RedirectUrlList = Program.ConfigurationRoot.GetSection("RedirectUrl").Get<List<RedirectItem>>() ?? new List<RedirectItem>();

            RenumberRedirectUrlListIndexes();

            RedirectUrlList.Add(new RedirectItem()
            {
                Id = RedirectUrlList.Count + 1,
                Name = "",
                Url = ""
            });

            await base.OnInitializedAsync();

        }

        private async Task UpdateAppSettings()
        {
#if DEBUG
            var path = System.IO.Path.Combine(Environment.CurrentDirectory, "appsettings.json");
#else
            var path = System.IO.Path.Combine(System.AppContext.BaseDirectory, "appsettings.json");
#endif

            var configJson = await File.ReadAllTextAsync(path);
            var config = JsonSerializer.Deserialize<Dictionary<string, object>>(configJson);

            if (config != null)
            {
                config["RedirectUrl"] = RedirectUrlList
                    .Where(x => !string.IsNullOrEmpty(x.Name) && !string.IsNullOrEmpty(x.Url)).ToArray();

                var updatedConfigJson =
                    JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(path, updatedConfigJson);

                Program.ConfigurationRoot.Reload();
            }
        }

        private async Task CommittedItemChanges(RedirectItem item)
        {
            if (!string.IsNullOrEmpty(item.Name) && !string.IsNullOrEmpty(item.Url))
            {
                RedirectUrlList[item.Id - 1].Name = item.Name;
                RedirectUrlList[item.Id - 1].Url = item.Url;

                if (item.Id == RedirectUrlList.Count)
                {
                    RedirectUrlList.Add(new RedirectItem()
                    {
                        Id = RedirectUrlList.Count + 1,
                        Name = "",
                        Url = ""
                    });
                }

                await UpdateAppSettings();

                StateHasChanged();
            }
        }

        private async Task DeleteUrl(RedirectItem item)
        {

            RedirectUrlList.RemoveAt(item.Id - 1);

            RenumberRedirectUrlListIndexes();

            await UpdateAppSettings();

            StateHasChanged();

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
