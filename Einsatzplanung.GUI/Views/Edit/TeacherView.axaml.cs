using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

using Einsatzplanung.GUI.ViewModels.Edit;
using Einsatzplanung.Types.Models;

using Microsoft.Extensions.DependencyInjection;

namespace Einsatzplanung.GUI.Views.Edit;

	public partial class TeacherView : UserControl {

	public TeacherView() {
		InitializeComponent();
		DataContext = App.Current.Services.GetRequiredService<TeacherPageViewModel>();
	}
}
