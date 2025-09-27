using System.Text.Json;

namespace kiosk_server.Services
{
    public class LayoutService
    {
        public static bool IsDarkMode => Program.ConfigurationRoot.GetValue<bool>("DarkMode");

        public event EventHandler MajorUpdateOccured = null!;

        private void OnMajorUpdateOccured() => MajorUpdateOccured?.Invoke(this, EventArgs.Empty);

        private static async Task UpdateAppSettings(bool darkMode)
        {
#if DEBUG
            var path = Path.Combine(Environment.CurrentDirectory, "appsettings.json");
#else
            var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
#endif

            var configJson = await File.ReadAllTextAsync(path);
            var config = JsonSerializer.Deserialize<Dictionary<string, object>>(configJson);

            if (config != null)
            {
                config["DarkMode"] = darkMode;

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
