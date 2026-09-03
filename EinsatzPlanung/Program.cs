namespace EinsatzPlanung;

using EinsatzPlanung.GUI;
using Einsatzplanung.Util.Services;

using Avalonia;
using System;
using System.IO;
using System.Diagnostics;

internal sealed class Program {
	/// <summary>
	///  The main entry point for the application.
	/// </summary>
	[STAThread]
	public static void Main(string[] args) {
		PathService.ExtractFiles("EinsatzPlanung");
#if !DEBUG
		try {
#endif
			BuildAvaloniaApp()
				.StartWithClassicDesktopLifetime(args);
#if !DEBUG
		} catch (Exception ex) {
			string path = PathService.CrashesPath($"crash-{DateTimeService.ToDayAndMonthAndYearString(DateTime.Now)}.log");
			File.WriteAllText(path, ex.ToString());
		}
#endif
	}

	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder.Configure<App>()
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace();
}
