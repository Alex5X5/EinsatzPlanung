namespace Einsatzplanung.GUI.ViewModels
{
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Platform.Storage;

    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;

	using EinsatzPlanung.Input.Interfaces;

    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class ImportCardViewModel : ObservableObject
    {
		private IEntityService entityService;

		[ObservableProperty]
		private string excelFileStatus = "Keine Excel-Datei ausgewählt";
		[ObservableProperty]
        private string pdfFileStatus = "Keine PDF-Datei ausgewählt";

        public ImportCardViewModel(string header, IEntityService entityService)
        {
            Header = header;
            this.entityService = entityService;
        }

        public string Header { get; }

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
				ExcelFileStatus = status ?? "Keine Datei ausgewählt";
				System.Console.WriteLine(ExcelFileStatus);
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