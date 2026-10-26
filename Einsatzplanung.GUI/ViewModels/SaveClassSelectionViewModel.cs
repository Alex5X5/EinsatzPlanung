namespace Einsatzplanung.GUI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;


public partial class ClassSelectionItem : ViewModelBase {

    [ObservableProperty] 
	private string name = "";

    [ObservableProperty] 
	private bool isSelected = true;
}