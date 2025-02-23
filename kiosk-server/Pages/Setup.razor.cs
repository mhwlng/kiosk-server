using kiosk_server.Model;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using System.Text.Json;
using kiosk_server.Shared;
using kiosk_server.Metrics;
using kiosk_server.Services;

namespace kiosk_server.Pages
{
    public partial class Setup
    {
        [Inject] private LayoutService LayoutService { get; set; } = null!;

        [CascadingParameter] public MainLayout Layout { get; set; } = null!;

        private readonly SetupModel SetupModel = new();

        private List<RedirectItem> RedirectUrlList { get; set; } = null!;

        
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

            // called twice in case server mode = serverprerendered

            var memoryMetricsClient = new MemoryMetricsClient();
            SetupModel.Memory = MemoryMetricsClient.GetMetrics();

            var temperatureMetricsClient = new TemperatureMetricsClient();
            SetupModel.Temperature = TemperatureMetricsClient.GetMetrics();

            var diskMetricsClient = new DiskMetricsClient();
            SetupModel.Disk = DiskMetricsClient.GetMetrics();

            var cpuMetricsClient = new CpuMetricsClient();
            SetupModel.Cpu = CpuMetricsClient.GetMetrics();

            RedirectUrlList = Program.ConfigurationRoot.GetSection("RedirectUrl").Get<List<RedirectItem>>() ?? [];

            RenumberRedirectUrlListIndexes();

            RedirectUrlList.Add(new RedirectItem
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
            var path = Path.Combine(Environment.CurrentDirectory, "appsettings.json");
#else
            var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
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
                    RedirectUrlList.Add(new RedirectItem
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
            Process.Start(new ProcessStartInfo { FileName = "sudo", Arguments = "reboot now" });
        }

        private static void HandleShutdown()
        {
            Process.Start(new ProcessStartInfo { FileName = "sudo", Arguments = "shutdown now" });
        }
    }
}
