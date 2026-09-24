namespace Einsatzplanung.GUI.ViewModels
{
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Platform.Storage;

    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;

	using Einsatzplanung.Input.Interfaces;
	using Einsatzplanung.Util.Interfaces;

	using System;
	using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class ImportCardViewModel : ObservableObject {
		
		private readonly IEntityService entityService;
		private readonly ILastSelectionService selectionService;

		[ObservableProperty]
		private string excelFileStatus = "Keine Excel-Datei ausgewählt";
		[ObservableProperty]
        private string pdfFileStatus = "Keine PDF-Datei ausgewählt";

		partial void OnExcelFileStatusChanged(string value) {
			string key = Header switch {
				"Ausbilder & Spezialisierungen" => "ImportCard.Trainer",
				"Ausbildungsgruppen & Themen" => "ImportCard.Groups",
				"Urlaub & Feiertage" => "ImportCard.Holiday",
				_ => ""
			};
			selectionService.SetSelection(key, value);
			entityService.SetSource(value);
		}

        public ImportCardViewModel(string header, IEntityService entityService, ILastSelectionService selectionService) {
            Header = header;
			this.entityService = entityService;
			this.selectionService = selectionService;
			ExcelFileStatus = Header switch {
				"Ausbilder & Spezialisierungen" => selectionService.GetSelection("ImportCard.Trainer") ?? "Keine Excel-Datei ausgewählt",
				"Ausbildungsgruppen & Themen" => selectionService.GetSelection("ImportCard.Groups") ?? "Keine Excel-Datei ausgewählt",
				"Urlaub & Feiertage" => selectionService.GetSelection("ImportCard.Holiday") ?? "Keine Excel-Datei ausgewählt",
				_ => "Keine Excel-Datei ausgewählt"
			};
        }

        public string Header { get; }

        public string HelpText => Header switch
        {
            "Ausbilder & Spezialisierungen" => "Hier werden Ausbilder und ihre Spezialisierungen importiert. Diese Daten werden für die Zuordnung der Einsatzzeiten verwendet.",
            "Ausbildungsgruppen & Themen" => "Hier werden Ausbildungsgruppen und zugehörige Themen geladen. Damit können Klassen und Fächer korrekt in die Planung übernommen werden.",
            "Urlaub & Feiertage" => "Hier werden Urlaubstage und Feiertage importiert. Diese Tage werden bei der Planung berücksichtigt und nicht als normale Arbeitstage behandelt.",
            _ => "Hier können relevante Daten für diesen Bereich importiert werden."
        };

        //public string ExcelFileStatus
        //{
        //    get => _excelFileStatus;
        //    set => SetProperty(ref _excelFileStatus, value);
        //}

        //public string PdfFileStatus
        //{
        //    get => _pdfFileStatus;
        //    set => SetProperty(ref _pdfFileStatus, value);
        //}

        [RelayCommand]
        private async Task ImportExcel(Window window)
        {
            string? status = await PickFileName(window, "Excel-Datei auswählen", [
                new FilePickerFileType("Excel Files")
                {
                    Patterns = ["*.xlsx", "*.xlsm", "*.xltx", "*.xltm"]
                },
                FilePickerFileTypes.All
            ]);
			await Task.Run(() => {
				ExcelFileStatus = status ?? "Keine Datei ausgewählt";
				System.Console.WriteLine(ExcelFileStatus);
				entityService.SetSource(ExcelFileStatus);
			});
        }

        [RelayCommand]
        private async Task ImportPdf(Window window)
        {
			string? status = await PickFileName(window, "PDF-Datei auswählen", [
                new FilePickerFileType("PDF Files")
                {
                    Patterns = ["*.pdf"]
                },
                FilePickerFileTypes.All
            ]);
			await Task.Run(() => {
                PdfFileStatus = status ?? "Keine Datei ausgewählt";
                System.Console.WriteLine(PdfFileStatus);
                entityService.SetSource(PdfFileStatus);
			});
		}

        private static async Task<string> PickFileName(
            Window window,
            string title,
            IReadOnlyList<FilePickerFileType> fileTypeFilter)
        {

            var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = title,
                AllowMultiple = false,
                FileTypeFilter = fileTypeFilter
            });

            var file = files.FirstOrDefault();

            return file?.Path.LocalPath ?? "Keine Datei ausgewählt";
        }
    }
}