namespace Einsatzplanung.GUI.Views.Edit;

using System;
using System.Collections.Generic;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

using Einsatzplanung.GUI.Controls;
using Einsatzplanung.GUI.ViewModels.Edit;

public partial class TopicView : ViewBase<TopicViewModel> {

	public TopicView() : base() {
		InitializeComponent();
	}
}
