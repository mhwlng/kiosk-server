// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace kiosk_server.Services
{
    public class MyEventService
    {
        public event Action<string?> OnUrlChange = null!;
        
        public void NavigateToUrl(string? url)
        {
            OnUrlChange?.Invoke(url);

        }


    }


}
