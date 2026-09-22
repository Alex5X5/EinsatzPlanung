namespace Einsatzplanung.GUI;

using System;

using Microsoft.Extensions.DependencyInjection;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Einsatzplanung.Excel.Services;
using Einsatzplanung.Excel.Interfaces;

using Einsatzplanung.Generation.Interfaces;
using Einsatzplanung.Generation.Services;

using Einsatzplanung.GUI.Views;
using Einsatzplanung.GUI.ViewModels;
using Einsatzplanung.GUI.ViewModels.Edit;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Input.Services;
using Einsatzplanung.Input.Services.Config;

using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Configuration;

using Einsatzplanung.Util.Services;
using Einsatzplanung.Util.Interfaces;

public partial class App : Application {

	public static new App Current => (Application.Current as App)!;

    public IServiceProvider Services { private set; get; }

	public override void Initialize() {
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted() {
		IServiceCollection serviceCollection = new ServiceCollection();
		AddAppServices(serviceCollection);
		Services = serviceCollection.BuildServiceProvider();


		PathService.ExtractFiles("Einsatzplanung");

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
		// Request services by calling App.Current.Services.GetService<AServiceType>()
		// or just as an argument in a custructor

		collection.AddSingleton<ILastSelectionService, LastSelectionService>();

		collection.AddSingleton<GeneralConfigService>();

		collection.AddTransient<ExcelImportService>();
		collection.AddTransient<ExcelExportService>();

		collection.AddSingleton<IConfigService<Topic>, TopicService>();

		collection.AddSingleton<IConfigService<TeacherConfig>, TeacherConfigService>();
		collection.AddTransient<IEntityService<Teacher>, TeacherService>();

		collection.AddSingleton<IConfigService<HolidayConfig>, HolidayConfigService>();
		collection.AddTransient<IEntityService<Holiday>, HolidayService>();

		collection.AddSingleton<IConfigService<AgeGroupConfig>, AgeGroupConfigService>();
		collection.AddSingleton<IConfigService<GroupConfig>, GroupConfigService>();
		collection.AddSingleton<IMappedConfigService<GroupConfig, BlockConfig>, BlockConfigService>();

		collection.AddTransient<IEntityService<AgeGroup>, AgeGroupService>();
		collection.AddTransient<IEntityService<Group>, GroupService>();
		collection.AddTransient<IMappedEntityService<Group, Block>, BlockService>();

		collection.AddTransient<IGeneratorService, GeneratorService>();
		collection.AddTransient<IPlanExportService, PlanExportService>();

		collection.AddSingleton<MainViewModel>();
		collection.AddTransient<ImportViewModel>();
		collection.AddTransient<EditViewModel>();
		collection.AddTransient<SaveViewModel>();
		collection.AddTransient<ImportCardViewModel>();
		collection.AddTransient<WelcomeViewModel>();

		collection.AddTransient<ClassesPageViewModel>();
		collection.AddTransient<ClassCardViewModel>();
		collection.AddTransient<TeacherPageViewModel>();
		collection.AddTransient<TeacherCardViewModel>();
		collection.AddTransient<TeacherTopicCardViewModel>();

	}

}