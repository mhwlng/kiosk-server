using kiosk_server.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace kiosk_server.Pages
{
    public partial class Index
    {
        [CascadingParameter] public MainLayout MainLayout { get; set; } = default!;

        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        
        private List<string> Urls { get; set; } = new();

        private string RedirectUrl { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
#if !DEBUG
                if (!string.IsNullOrEmpty(RedirectUrl))
                {
                    NavigationManager.NavigateTo(RedirectUrl, true);
                }
#endif
                //StateHasChanged();
            }


        }


        protected override async Task OnInitializedAsync()
        {
            MainLayout.Title = "Kiosk Server";

            var port = Program.ConfigurationRoot.GetValue<int>("Port");

            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (!item.Description.Contains("virtual", StringComparison.CurrentCultureIgnoreCase) &&
                    item.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            Urls.Add($"http://{ip.Address}:{port}");
                        }
                    }
                }
            }

            RedirectUrl = Program.ConfigurationRoot.GetValue<string>("RedirectUrl");

#if !DEBUG
                var localhost = NavigationManager.Uri.Contains("127.0.0.1");

                if (!localhost)
                {
                    RedirectUrl = "/setup";
                }

#endif
            await base.OnInitializedAsync();


        }
    }
}
