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
			DisableAvaloniaDataAnnotationValidation();
			desktop.MainWindow = new MainWindow {
				DataContext = Services.GetService<MainWindowViewModel>(),
			};
		}

		base.OnFrameworkInitializationCompleted();
	}

	public static void AddAppServices(IServiceCollection collection) {
		collection.AddTransient<MainWindowViewModel>();
	}

	private void DisableAvaloniaDataAnnotationValidation() {
		// Get an array of plugins to remove
		var dataValidationPluginsToRemove =
			BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

		// remove each entry found
		foreach (var plugin in dataValidationPluginsToRemove) {
			BindingPlugins.DataValidators.Remove(plugin);
		}
	}
}