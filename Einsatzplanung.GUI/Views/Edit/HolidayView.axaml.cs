using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

using Einsatzplanung.GUI.ViewModels.Edit;
using Einsatzplanung.Types.Models;

using Microsoft.Extensions.DependencyInjection;

namespace Einsatzplanung.GUI.Views.Edit;

	public partial class HolidayView : UserControl {

	public HolidayView() {
		InitializeComponent();
		DataContext = App.Current.Services.GetRequiredService<HolidayViewModel>();
	}
}
