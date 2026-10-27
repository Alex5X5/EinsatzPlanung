namespace Einsatzplanung.GUI.Views.Edit;

using System;
using System.Linq;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

using Einsatzplanung.GUI.ViewModels.Edit;


public partial class AgeGroupPageView : ViewBase<AgeGroupPageViewModel> {

	public AgeGroupPageView() : base() {
		InitializeComponent();
	}
}
