namespace EinsatzPlanung.GUI;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;

using Einsatzplanung.Excel.Services;
using Einsatzplanung.GUI;
using Einsatzplanung.GUI.ViewModels;
using Einsatzplanung.GUI.Views;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Linq;

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
			desktop.MainWindow = new MainWindow() {
				DataContext = Services.GetRequiredService<MainViewModel>(),
				Title = "Einsatz Planung",
			};
		} else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform) {
			singleViewPlatform.MainView = new MainView() {
				DataContext = Services.GetRequiredService<MainViewModel>()
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
		collection.AddTransient<ExcelExportService>();

		collection.AddTransient<MainViewModel>();
		collection.AddTransient<ImportViewModel>();
		collection.AddTransient<ImportCardViewModel>();
		collection.AddTransient<WelcomeViewModel>();

	}

}