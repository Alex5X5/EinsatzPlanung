namespace EinsatzPlanung.ViewModels
{
    using System.Linq;
    using System.Threading.Tasks;

    using Avalonia.Controls;
    using Avalonia.Platform.Storage;

    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;

    public partial class ImportCardViewModel : ObservableObject
    {
        public ImportCardViewModel(string header)
        {
            Header = header;
        }

        public string Header { get; }

        [ObservableProperty] private string _excelFileStatus = "Keine Excel-Datei ausgewählt";

        [ObservableProperty] private string _pdfFileStatus = "Keine PDF-Datei ausgewählt";

        [RelayCommand]
        private async Task ImportExcel(Window window)
        {
            _excelFileStatus = await PickFileName(window, "Excel-Datei auswählen");
        }

        [RelayCommand]
        private async Task ImportPdf(Window window)
        {
            _pdfFileStatus = await PickFileName(window, "PDF-Datei auswählen");
        }

        private static async Task<string> PickFileName(Window window, string title)
        {

            var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = title,
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Excel Files")
                    {
                        Patterns = ["*.xlsx", "*.xlsm", "*.xlsb", "*.csv"]
                    },
                    new FilePickerFileType("PDF Files")
                    {
                        Patterns = ["*.pdf"]
                    },
                    FilePickerFileTypes.All
                }
            });

            var file = files.FirstOrDefault();

            return file?.Name ?? "Keine Datei ausgewählt";
        }
    }
}