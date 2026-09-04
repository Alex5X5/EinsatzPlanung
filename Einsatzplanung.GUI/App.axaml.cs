namespace EinsatzPlanung.GUI;

using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;

using EinsatzPlanung.ViewModels;
using EinsatzPlanung.GUI.Views;
using Einsatzplanung.Excel.Services;

public partial class App : Application {

	public static new App Current => (App)Application.Current;

    public IServiceProvider Services { private set; get; }

	public override void Initialize() {
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted() {
		IServiceCollection serviceCollection = new ServiceCollection();
		AddAppServices(serviceCollection);
		Services = serviceCollection.BuildServiceProvider();

		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
			desktop.MainWindow = new MainWindow {
				DataContext = Services.GetService<MainWindowViewModel>(),
			};
		}

		base.OnFrameworkInitializationCompleted();
	}

	public static void AddAppServices(IServiceCollection collection) {
		// Transient services are created each time they are requested.
		// Singleton services are created once and then reused.
		//Request services by calling App.Current.Services.GetService<AServiceType>()
		// or just as an argument in a custructor
		collection.AddTransient<ExcelImportService>();
		collection.AddTransient<MainWindowViewModel>();
		
	}

}