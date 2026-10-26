namespace Einsatzplanung.GUI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

using Einsatzplanung.Types.Models;

public partial class ClassSelectionItem : ViewModelBase {

    [ObservableProperty] 
	private string name = "";

    [ObservableProperty] 
	private bool isSelected = true;
    
    public Group Config { set; get; }
}