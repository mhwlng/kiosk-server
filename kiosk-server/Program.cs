using System.Linq.Expressions;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.PortableExecutable;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.FileProviders;
using MudBlazor.Services;



class Program
{
    public static IConfigurationRoot ConfigurationRoot { get; set; } = default!;

    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        ConfigurationRoot = builder.Configuration;

        builder.Host.UseSystemd();

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

//builder.WebHost.UseStaticWebAssets(); // needs absolute path ??????????


// Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddMudServices();

        builder.Services.AddCors();

        //builder.Services.AddSingleton<WeatherForecastService>();


        builder.Services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                new[] { "application/octet-stream" });
        });

        var app = builder.Build();

        app.UseResponseCompression();

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

        app.UseStaticFiles();

        app.UseRouting();

        // global cors policy
        app.UseCors(x => x
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true) // allow any origin
            .AllowCredentials()); // allow credentials

        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

/*
app.UseEndpoints(endpoints =>
{
    endpoints.MapBlazorHub();
    //endpoints.MapHub<MyHub>("/myhub");
    endpoints.MapFallbackToPage("/_Host");
});*/

        app.Run();
    }
}
