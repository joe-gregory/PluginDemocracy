using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PluginDemocracy.WebApp.Components;
using PluginDemocracy.UIComponents;
using MudBlazor.Services;
using MudBlazor;

namespace PluginDemocracy.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                .AddJsonFile("appsettings.WebApp.json", optional: false, reloadOnChange: true);

            // Add services to the container.
            builder.Services.AddRazorComponents().AddInteractiveServerComponents();

            builder.Services.AddHttpClient("MyHttpClient").ConfigureHttpClient(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
                client.MaxResponseContentBufferSize = 629145600; // 600MB
            });
            builder.Services.AddScoped(sp =>
            {
                // Create a named HttpClient using the HttpClientFactory
                var factory = sp.GetRequiredService<IHttpClientFactory>();
                return factory.CreateClient("MyHttpClient");
            });

            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
                config.SnackbarConfiguration.PreventDuplicates = true;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 10000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
                config.SnackbarConfiguration.ClearAfterNavigation = false;
            });

            builder.Services.AddCascadingAuthenticationState();

            builder.Services.AddSingleton<BaseAppState, WebAppState>();
            builder.Services.AddScoped<Services>();

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddIdentityCookies();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

#if DEBUG
            // Configure logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            builder.Logging.AddFilter("Microsoft", LogLevel.Debug);
            builder.Logging.AddFilter("System", LogLevel.Debug);
            builder.Logging.AddFilter("Default", LogLevel.Debug);
#endif

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            // Add additional endpoints required by the Identity /Account Razor components.

            app.Run();
        }
    }
}
