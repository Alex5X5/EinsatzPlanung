namespace Einsatzplanung.GUI.ViewModels
{
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Platform.Storage;

    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;

    using Einsatzplanung.Input.Interfaces;
	using Einsatzplanung.Types.Models;

	using System;
	using System.IO;
	using System.Linq;
    using System.Threading.Tasks;

    public partial class SaveViewModel : ViewModelBase
{
    [ObservableProperty]
    private string selectedFolderPath = GetDefaultDownloadsFolder();

    [RelayCommand]
    private async Task SavePath()
    {
        var window = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (window is null)
        {
            Console.WriteLine("Kein Fenster gefunden");
            return;
        }

        var folders = await window.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Ordner auswählen",
            AllowMultiple = false
        });

        var folder = folders.FirstOrDefault();
        SelectedFolderPath = folder?.Path.LocalPath ?? SelectedFolderPath;
        Console.WriteLine(SelectedFolderPath);
    }

    [RelayCommand]
    private void Export()
    {
        // Hier später deine Export-Datei erzeugen und
        // z. B. unter Path.Combine(SelectedFolderPath, "export.xlsx") speichern
    }

    private static string GetDefaultDownloadsFolder()
    {
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return Path.Combine(userProfile, "Downloads");
    }
}
}