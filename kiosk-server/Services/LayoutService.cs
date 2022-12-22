// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace kiosk_server.Services
{
    public class LayoutService
    {
        public bool IsDarkMode => Program.ConfigurationRoot.GetValue<bool>("DarkMode");

        public event EventHandler MajorUpdateOccured = null!;

        private void OnMajorUpdateOccured() => MajorUpdateOccured?.Invoke(this, EventArgs.Empty);

        private async Task UpdateAppSettings(bool darkMode)
        {
#if DEBUG
            var path = System.IO.Path.Combine(Environment.CurrentDirectory, "appsettings.json");
#else
            var path = System.IO.Path.Combine(System.AppContext.BaseDirectory, "appsettings.json");
#endif

            var configJson = await File.ReadAllTextAsync(path);
            var config = JsonSerializer.Deserialize<Dictionary<string, object>>(configJson);

            if (config != null)
            {
                config["DarkMode"] = (bool)darkMode;

                var updatedConfigJson =
                    JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(path, updatedConfigJson);

                Program.ConfigurationRoot.Reload();
            }
        }

        public async Task ToggleDarkMode()
        {
            await UpdateAppSettings(!IsDarkMode);
    
            OnMajorUpdateOccured();
        }


    }


}
