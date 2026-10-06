namespace Einsatzplanung.GUI.Views.Edit;

using Avalonia.Markup.Xaml;

using Microsoft.Extensions.DependencyInjection;

using Einsatzplanung.GUI.ViewModels.Edit;

public partial class HolidayView : ViewBase {

	public HolidayView() : base(App.Current.Services.GetRequiredService<HolidayViewModel>()) {
		InitializeComponent();
	}
}
