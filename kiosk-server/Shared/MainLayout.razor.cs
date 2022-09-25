using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace kiosk_server.Shared
{
    public partial class MainLayout
    {
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

    }
}
