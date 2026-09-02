namespace EinsatzPlanung.ViewModels {
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Threading.Tasks;

	using Avalonia.Controls;
	using Avalonia.Platform.Storage;

	using CommunityToolkit.Mvvm.ComponentModel;
	using CommunityToolkit.Mvvm.Input;

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

		[ObservableProperty]
		private string _importExcelFileStatus = "Keine Datei ausgewählt";

		[ObservableProperty]
		private string _importPdfFileStatus = "Keine Datei ausgewählt";
		

		[RelayCommand]
		private async Task AddFirstCardFile(Window window) {
			_importExcelFileStatus = await PickFileName(window);
		}

		[RelayCommand]
		private async Task AddSecondCardFile(Window window) {
			_importPdfFileStatus = await PickFileName(window);
		}
		

		private static async Task<string> PickFileName(Window window) {
			var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions {
				Title = "Datei auswählen",
				AllowMultiple = false,
			});

			var file = files.FirstOrDefault();

			return file?.Name ?? "Keine Datei ausgewählt";
		}
	}

	public partial class ImportCardViewModel : ObservableObject {
		public ImportCardViewModel(string header) {
			Header = header;
		}

		public string Header { get; }

		[ObservableProperty]
		private string _excelFileStatus = "Keine Excel-Datei ausgewählt";

		[ObservableProperty]
		private string _pdfFileStatus = "Keine PDF-Datei ausgewählt";

		[RelayCommand]
		private async Task ImportExcel(Window window) {
			_excelFileStatus = await PickFileName(window, "Excel-Datei auswählen");
		}

		[RelayCommand]
		private async Task ImportPdf(Window window) {
			_pdfFileStatus = await PickFileName(window, "PDF-Datei auswählen");
		}

		private static async Task<string> PickFileName(Window window, string title) {
			if (window is null) {
				return "Fenster nicht gefunden";
			}

			var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions {
				Title = title,
				AllowMultiple = false,
				FileTypeFilter = new[] {
					new FilePickerFileType("Excel Files") {
						Patterns = new[] { "*.xlsx", "*.xlsm", "*.xlsb", "*.csv" }
					},
					new FilePickerFileType("PDF Files") {
						Patterns = new[] { "*.pdf" }
					},
					FilePickerFileTypes.All
				}
			});

			var file = files.FirstOrDefault();

			return file?.Name ?? "Keine Datei ausgewählt";
		}
	}
}
