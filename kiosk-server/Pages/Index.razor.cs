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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                var redirectUrl = Program.ConfigurationRoot.GetValue<string>("RedirectUrl");

#if !DEBUG
                var localhost = NavigationManager.Uri.Contains("127.0.0.1");

                if (!localhost)
                {
                    redirectUrl = "/setup";
                }

#else
                redirectUrl = "";
#endif

                if (!string.IsNullOrEmpty(redirectUrl))
                {
                    NavigationManager.NavigateTo(redirectUrl, true);
                }

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
            await base.OnInitializedAsync();


        }
    }
}
