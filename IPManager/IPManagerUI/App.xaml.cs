using IPManagerUI.Properties;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace IPManagerUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }

        public App()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            Log.Information("Application startup");

            try
            {
                AppHost = Host.CreateDefaultBuilder()
                    .UseSerilog()
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.Configure<AppSettingsSection>(configuration);

                        //Add dependencies

                        //Add resources from WPF
                        services.AddSingleton<MainWindow>();
                    })
                    .Build();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occured while building the application.");
                throw;
            }
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                await AppHost!.StartAsync();

                CheckUserSettings();

                var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
                startupForm.Show();

                base.OnStartup(e);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occured while application was running.");
                throw;
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();

            Log.Information("Application stopped");

            await Log.CloseAndFlushAsync();

            base.OnExit(e);
        }

        private void CheckUserSettings()
        {
            SetBaseTheme();
        }

        private void SetBaseTheme()
        {
            var paletteHelper = new PaletteHelper();
            //Retrieve the app's existing theme
            var theme = paletteHelper.GetTheme();

            if (UserSettings.Default.DarkTheme)
            {
                theme.SetBaseTheme(BaseTheme.Dark);
            }
            else
            {
                theme.SetBaseTheme(BaseTheme.Light);
            }

            //Change the app's current theme
            paletteHelper.SetTheme(theme);
        }
    }
}
