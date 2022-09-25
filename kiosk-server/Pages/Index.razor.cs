using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace kiosk_server.Pages
{
    public partial class Index
    {


        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        [Inject] private ProtectedLocalStorage ProtectedLocalStorage { get; set; } = default!;


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                var localhost = NavigationManager.Uri.Contains("127.0.0.1");

                var redirectUrl = (await ProtectedLocalStorage.GetAsync<string>("RedirectUrl")).Value;

                if (string.IsNullOrEmpty(redirectUrl))
                {
                    redirectUrl = "https://www.google.com";

                    await ProtectedLocalStorage.SetAsync("RedirectUrl", redirectUrl);
                }

                NavigationManager.NavigateTo(redirectUrl, true);

                // StateHasChanged();
            }


        }


        protected override async Task OnInitializedAsync()
        {

     
            await base.OnInitializedAsync();
        }
    }
}
