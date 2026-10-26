namespace Einsatzplanung.GUI.Services;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

public class FilePickerService {

	public static class Filters {

		public static readonly List<FilePickerFileType> Excel = [
			new("Excel-Datei") {
				Patterns = ["*.xlsx", "*.xlsm", "*.xltx", "*.xltm"]
			},
			FilePickerFileTypes.All
		];
	}

	public static async Task<string?> PickLoadFileExcelAsync() {
		return await PickLoadFileAsync("Excel Datei auswählen", Filters.Excel, false);
	}

	public static async Task<string?> PickSafeFileExcelAsync() {
		return await PickSafeFileAsync("Excel Datei auswählen", Filters.Excel, false);
	}

	public static async Task<string?> PickLoadFileAsync(string title, bool allowMultiple) =>
		await PickLoadFileAsync(title, [], allowMultiple);

	public static async Task<string?> PickLoadFileAsync(string title, List<FilePickerFileType> patterns, bool allowMultiple) =>
		await PickLoadFileAsync(
			new FilePickerOpenOptions() {
				Title = title,
				AllowMultiple = allowMultiple,
				FileTypeFilter = patterns
			});

	public static async Task<string?> PickLoadFileAsync(FilePickerOpenOptions options) {
		var window = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

		if (window is null)
			return await Task.FromResult<string?>(null);

		var files = await window.StorageProvider.OpenFilePickerAsync(options);

		var file = files.FirstOrDefault();

		return file?.Path.LocalPath ?? "";
	}

	public static async Task<string?> PickSafeFileAsync(string title, List<FilePickerFileType> patterns, bool allowMultiple) =>
		await PickLoadFileAsync(
			new FilePickerOpenOptions() {
				Title = title,
				AllowMultiple = allowMultiple,
				FileTypeFilter = patterns
			});

	public static async Task<string?> PickSafeFileAsync(FilePickerSaveOptions options) {
		var window = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

		if (window is null)
			return await Task.FromResult<string?>(null);

		var file = await window.StorageProvider.SaveFilePickerAsync(options);

		return file?.Path.LocalPath ?? "";
	}

	public static async Task<string?> PickFolder(string title) =>
		await PickFolder(new FolderPickerOpenOptions() { Title = title });

	public static async Task<string?> PickFolder(FolderPickerOpenOptions options) {
		var window = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

		if (window is null)
			return await Task.FromResult<string?>(null);

		var files = await window.StorageProvider.OpenFolderPickerAsync(options);

		var file = files.FirstOrDefault();

		return file?.Path.LocalPath ?? "";
	}
}
