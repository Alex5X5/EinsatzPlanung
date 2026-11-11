namespace Einsatzplanung.GUI.Views.Edit;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.VisualTree;

using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.CodeGenerators.Attributes;
using Einsatzplanung.GUI.Controls;
using Einsatzplanung.GUI.ViewModels.Edit;

public partial class TopicView : ViewBase<TopicViewModel> {

	public TopicView() : base() {
		InitializeComponent();
	}
}
public sealed class CardMovedEventArgs(object model, ObservableCollection<TopicCardViewModel> targetGroup, int targetCardIndex) : EventArgs {

	public object Model {set; get;} = model;
	public ObservableCollection<TopicCardViewModel> TargetGroup {set; get;} = targetGroup;
	public int TargetCardIndex {set; get;} = targetCardIndex;
	
}