using kiosk_server.Services;
using Microsoft.AspNetCore.ResponseCompression;
using MudBlazor.Services;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;



class Program
{
    public static IConfigurationRoot ConfigurationRoot { get; set; } = null!;

    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        ConfigurationRoot = builder.Configuration;

        builder.Host.UseSystemd();

        builder.WebHost.UseUrls();

        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            var port = ConfigurationRoot.GetValue<int>("Port");

            serverOptions.Listen(IPAddress.Loopback, port);

            foreach (var item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (!item.Description.Contains("virtual", StringComparison.CurrentCultureIgnoreCase) &&
                    item.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (var ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            serverOptions.Listen(ip.Address, port);
                        }
                    }
                }
            }

        });

        builder.WebHost.UseStaticWebAssets();


        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddMudServices();

        // Add services to manage API controller
        builder.Services.AddControllers();

        builder.Services.AddCors();

        //builder.Services.AddSingleton<WeatherForecastService>();

        builder.Services.AddScoped<LayoutService>();

        builder.Services.AddSingleton<MyEventService>();

        builder.Services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                ["application/octet-stream"]);
        });

        var app = builder.Build();

        if (!app.Environment.IsDevelopment()) // response compression currently conflicts with dotnet watch browser reload
        {
            app.UseResponseCompression();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

/*
var strExeFilePath = Assembly.GetEntryAssembly().Location;
var exePath = Path.GetDirectoryName(strExeFilePath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(exePath, @"wwwroot")),
});*/

        app.MapStaticAssets();

        app.UseRouting();

        // global cors policy
        app.UseCors(x => x
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true) // allow any origin
            .AllowCredentials()); // allow credentials

        app.MapControllers();

        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        /*
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapBlazorHub();
            //endpoints.MapHub<MyHub>("/myhub");
            endpoints.MapFallbackToPage("/_Host");
        });*/
        /*
        app.MapGet("/aaa/bbb", () =>
        {
            string[] data = new string[] {
                "Hello World!",
                "Hello Galaxy!",
                "Hello Universe!"
            };
            return Results.Ok(data);
        });*/

        app.Run();
    }
}
