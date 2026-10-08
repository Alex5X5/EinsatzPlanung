namespace Einsatzplanung.GUI.ViewModels;

public class CardViewModel : ViewModelBase {

	public int Index { get; } = 0;

	public CardViewModel(int index) {
		Index = index;
	}

}
