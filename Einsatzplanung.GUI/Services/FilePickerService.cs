namespace Einsatzplanung.GUI.Services;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

public class FilePickerService {

	public static async Task<string?> PickFileName(string title, bool allowMultiple) =>
		await PickFileName(title, allowMultiple, []);

	public static async Task<string?> PickFileName(string title, bool allowMultiple, IReadOnlyList<FilePickerFileType> fileTypeFilter) {
		var window = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
		
		if (window is null)
			return await Task.FromResult<string?>(null);

		var options = new FilePickerOpenOptions {
			Title = title,
			AllowMultiple = allowMultiple,
			FileTypeFilter = fileTypeFilter
		};

		return await PickFileName(options);
	}

	public static async Task<string?> PickFileName(FilePickerOpenOptions options) {
		var window = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

		if (window is null)
			return await Task.FromResult<string?>(null);

		var files = await window.StorageProvider.OpenFilePickerAsync(options);

		var file = files.FirstOrDefault();

		return file?.Path.LocalPath ?? "";
	}

	public static async Task<string?> PickSaveLocation(FilePickerSaveOptions options) {
		var window = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

		if (window is null)
			return await Task.FromResult<string?>(null);

		var file = await window.StorageProvider.SaveFilePickerAsync(options);

		return file?.Path.LocalPath ?? "";
	}
}
