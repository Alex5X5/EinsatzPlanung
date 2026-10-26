namespace Einsatzplanung.GUI.ViewModels
{
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Platform.Storage;

    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;

	using Einsatzplanung.GUI.Services;
	using Einsatzplanung.Input.Interfaces;
	using Einsatzplanung.Util.Interfaces;

	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
    using System.Threading.Tasks;

    public partial class ImportCardViewModel : ObservableObject {
		
		private readonly IEntityService entityService;
		private readonly ILastSelectionService selectionService;

        public string Header { get; }

		[ObservableProperty]
		private string excelFileStatus = "Keine Excel-Datei ausgewählt";
        [ObservableProperty]
        private string excelShortendPath = "Keine Excel-Datei ausgewählt";
		[ObservableProperty]
        private string pdfFileStatus = "Keine PDF-Datei ausgewählt";

		partial void OnExcelFileStatusChanged(string value) {
			string key = Header switch {
				"Ausbildungsinhalte" => "ImportCard.Topics",
				"Ausbilder*innen & Spezialisierungen" => "ImportCard.Trainer",
				"Ausbildungsgruppen & Themen" => "ImportCard.Groups",
				"Urlaub & Feiertage" => "ImportCard.Holiday",
				_ => ""
			};
			selectionService.SetSelection(key, value);
			entityService.SetSource(value);
		}

		public string HelpText => Header switch {
			"Ausbildungsinhalte" => "Hier werden die Themen der Ausbildung importiert. Die Spezialisierungen der Ausbilder*innen und die Ausblidungsinhalte der Klassen werden von diesen Themen ausgewählt.",
			"Ausbilder*innen & Spezialisierungen" => "Hier werden Ausbilder*innen und ihre Spezialisierungen importiert. Diese Daten werden für die Zuordnung der Einsatzzeiten verwendet.",
			"Ausbildungsgruppen & Themen" => "Hier werden Ausbildungsgruppen und zugehörige Themen geladen. Damit können Klassen und Fächer korrekt in die Planung übernommen werden.",
			"Urlaub & Feiertage" => "Hier werden Urlaubstage und Feiertage importiert. Diese Tage werden bei der Planung berücksichtigt und nicht als normale Arbeitstage behandelt.",
			_ => "Hier können relevante Daten für diesen Bereich importiert werden."
		};

		public ImportCardViewModel(string header, IEntityService entityService, ILastSelectionService selectionService) {
            Header = header;
			this.entityService = entityService;
			this.selectionService = selectionService;
			string path = Header switch {
				"Ausbildungsinhalte" => selectionService.GetSelection("ImportCard.Topics") ?? "Keine Excel-Datei ausgewählt",
				"Ausbilder*innen & Spezialisierungen" => selectionService.GetSelection("ImportCard.Trainer") ?? "Keine Excel-Datei ausgewählt",
				"Ausbildungsgruppen & Themen" => selectionService.GetSelection("ImportCard.Groups") ?? "Keine Excel-Datei ausgewählt",
				"Urlaub & Feiertage" => selectionService.GetSelection("ImportCard.Holiday") ?? "Keine Excel-Datei ausgewählt",
				_ => "Keine Excel-Datei ausgewählt"
			};
            ExcelFileStatus = path;
            ExcelShortendPath = path;
            if (ExcelShortendPath.Contains(Path.DirectorySeparatorChar) || ExcelShortendPath.Contains(Path.AltDirectorySeparatorChar))
                {
                    var p = ExcelShortendPath.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (p.Length > 4)
					    ExcelShortendPath = $"{p[0]}\\{p[1]}\\...\\{p[^2]}\\{p[^1]}";
                }
            
        }        

        [RelayCommand]
        private async Task ImportExcel(Window window) {
			string? status = await FilePickerService.PickLoadFileExcelAsync();

			await Task.Run(() => {
				ExcelFileStatus = status ?? "Keine Datei ausgewählt";
                ExcelShortendPath = ExcelFileStatus;
                if (ExcelShortendPath.Contains(Path.DirectorySeparatorChar) || ExcelShortendPath.Contains(Path.AltDirectorySeparatorChar))
                {
                    var p = ExcelShortendPath.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries);
                    if (p.Length > 4)
					    ExcelShortendPath = $"{p[0]}\\{p[1]}\\...\\{p[^2]}\\{p[^1]}";
                }
				entityService.SetSource(ExcelFileStatus);
			});
        }

  //      [RelayCommand]
  //      private async Task ImportPdf(Window window)
  //      {
		//	string? status = await PickFileName(window, "PDF-Datei auswählen", [
  //              new FilePickerFileType("PDF Files")
  //              {
  //                  Patterns = ["*.pdf"]
  //              },
  //              FilePickerFileTypes.All
  //          ]);
		//	await Task.Run(() => {
  //              PdfFileStatus = status ?? "Keine Datei ausgewählt";
  //              System.Console.WriteLine(PdfFileStatus);
  //              entityService.SetSource(PdfFileStatus);
		//	});
		//}

  //      private static async Task<string> PickFileName(
  //          Window window,
  //          string title,
  //          IReadOnlyList<FilePickerFileType> fileTypeFilter)
  //      {
			
  //          var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
  //          {
  //              Title = title,
  //              AllowMultiple = false,
  //              FileTypeFilter = fileTypeFilter
  //          });

  //          var file = files.FirstOrDefault();

  //          return file?.Path.LocalPath ?? "Keine Datei ausgewählt";
  //      }
    }
}