using kiosk_server.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MudBlazor;

namespace kiosk_server.Shared
{
    public partial class MainLayout
    {
        [Inject] private LayoutService LayoutService { get; set; } = null!;

        private string? _title;

        public string Title
        {
            get => _title ?? "";
            set
            {
                _title = value ?? "";
                InvokeAsync(StateHasChanged);
            }
        }
        protected override void OnInitialized()
        {
            LayoutService.MajorUpdateOccured += LayoutServiceOnMajorUpdateOccured;
            base.OnInitialized();
        }

        public void Dispose()
        {
            LayoutService.MajorUpdateOccured -= LayoutServiceOnMajorUpdateOccured;
        }
        
        private void LayoutServiceOnMajorUpdateOccured(object? sender, EventArgs e) => StateHasChanged();
    }
}
