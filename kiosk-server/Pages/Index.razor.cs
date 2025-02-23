using kiosk_server.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.Json.Serialization;

namespace kiosk_server.Pages
{
    public class RedirectItem
    {
        [JsonIgnore] public int Id { get; set; }


        public string? Name { get; set; } 
        public string? Url { get; set; } 

    }

    public partial class Index
    {
        [CascadingParameter] public MainLayout MainLayout { get; set; } = null!;

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        
        private List<string> WebServerUrls { get; set; } = [];

        private List<RedirectItem> RedirectUrlList { get; set; } = null!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
#if !DEBUG
                if (RedirectUrlList.Count(x => !string.IsNullOrEmpty(x.Url)) > 1)
                {
                    NavigationManager.NavigateTo("/kiosk", true);
                } 
                else if (!string.IsNullOrEmpty(RedirectUrlList.FirstOrDefault()?.Url))
                {
                    NavigationManager.NavigateTo(RedirectUrlList.FirstOrDefault()?.Url ?? "?", true);
                }
#else
               NavigationManager.NavigateTo("/setup", true);
#endif
                //StateHasChanged();
            }
        }
        

        protected override async Task OnInitializedAsync()
        {
            MainLayout.Title = "Kiosk Server";

            var port = Program.ConfigurationRoot.GetValue<int>("Port");

            foreach (var item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (!item.Description.Contains("virtual", StringComparison.CurrentCultureIgnoreCase) &&
                    item.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (var ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            WebServerUrls.Add($"http://{ip.Address}:{port}");
                        }
                    }
                }
            }

            RedirectUrlList = Program.ConfigurationRoot.GetSection("RedirectUrl").Get<List<RedirectItem>>() ?? [];

#if !DEBUG
            var localhost = NavigationManager.Uri.Contains("127.0.0.1");

            if (!localhost)
            {
                RedirectUrlList.Clear();
                RedirectUrlList.Add(new RedirectItem
                {
                    Name = "setup",
                    Url = "/setup"
                });
            }
#endif
            await base.OnInitializedAsync();


        }
    }
}
