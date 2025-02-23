﻿using kiosk_server.Services;
using Microsoft.AspNetCore.Components;

namespace kiosk_server.Shared
{
    public partial class EmptyLayout
    {
        [Inject] private LayoutService LayoutService { get; set; } = null!;

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
