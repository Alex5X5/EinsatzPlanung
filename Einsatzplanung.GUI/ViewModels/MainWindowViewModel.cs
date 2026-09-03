using Einsatzplanung.Excel.Services;

namespace EinsatzPlanung.ViewModels {
	using System.Collections.ObjectModel;

	public partial class MainWindowViewModel : ViewModelBase {
		public ObservableCollection<ImportCardViewModel> Cards { get; } =
		[
			new("Ausbilder & Spezialisierungen"),
			new("Ausbildungsgruppe"),
			new("Ausbildungsinhalte"),
			new("Urlaubswochen & Feiertage"),
			new("Praktikumszeiten"),
			new("Schulwochen")
		];

		public MainWindowViewModel()
		{
			var excelImportService = new ExcelImportService();

			excelImportService.PrintTable();
		}

        
	}

}