using Core.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Wpf.ApplicationForms;
using Wpf.DependencyInjection;


namespace Wpf;

public partial class App : System.Windows.Application
{
    #region Fields

    private IServiceProvider _serviceProvider = null!;

    #endregion Fields

    #region Methods

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        ServiceCollection services = new();

        services.AddSingleton<IConfiguration>(configuration);

        services.AddCoreFusion();
        services.AddCoreFusionAssembly(typeof(App).Assembly);
        services.AddWpfServices(configuration);

        _serviceProvider = services.BuildServiceProvider();

        MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();

        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        base.OnExit(e);
    }

    #endregion Methods
}