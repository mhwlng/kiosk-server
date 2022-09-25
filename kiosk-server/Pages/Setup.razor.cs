using kiosk_server.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;

namespace kiosk_server.Pages
{
    public partial class Setup
    {


        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        [Inject] private ProtectedLocalStorage ProtectedLocalStorage { get; set; } = default!;


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
              

                // StateHasChanged();
            }


        }


        protected override async Task OnInitializedAsync()
        {

     
            await base.OnInitializedAsync();
        }


        private SetupModel setupModel = new();

        async Task HandleValidSubmit()
        {


            await ProtectedLocalStorage.SetAsync("RedirectUrl", setupModel.Url ?? "");

        }
    }
}
