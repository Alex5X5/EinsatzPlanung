namespace EinsatzPlanung.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Platform.Storage;

    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;

	using DocumentFormat.OpenXml.EMMA;

	using Einsatzplanung.Excel.Services;
    using EinsatzPlanung.GUI;
    using Microsoft.Extensions.DependencyInjection;

    public partial class ImportCardViewModel : ObservableObject
    {
        private string _excelFileStatus = "Keine Excel-Datei ausgewählt";
        private string _pdfFileStatus = "Keine PDF-Datei ausgewählt";

        public ImportCardViewModel(string header)
        {
            Header = header;
        }

        public string Header { get; }

        public string ExcelFileStatus
        {
            get => _excelFileStatus;
            set => SetProperty(ref _excelFileStatus, value);
        }

        public string PdfFileStatus
        {
            get => _pdfFileStatus;
            set => SetProperty(ref _pdfFileStatus, value);
        }

        [RelayCommand]
        private async Task ImportExcel(Window window)
        {
            string? ExcelFileStatus = await PickFileName(window, "Excel-Datei auswählen", [
                new FilePickerFileType("Excel Files")
                {
                    Patterns = ["*.xlsx", "*.xlsm", "*.xltx", "*.xltm"]
                },
                FilePickerFileTypes.All
            ]);
            System.Console.WriteLine(ExcelFileStatus);
            ExcelImportService excelService = App.Current.Services.GetService<ExcelImportService>();
            Einsatzplanung.Excel.Models.Table table = excelService.CreateTableObj(ExcelFileStatus);
        }

        [RelayCommand]
        private async Task ImportPdf(Window window)
        {
            PdfFileStatus = await PickFileName(window, "PDF-Datei auswählen", [
                new FilePickerFileType("PDF Files")
                {
                    Patterns = ["*.pdf"]
                },
                FilePickerFileTypes.All
            ]);
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