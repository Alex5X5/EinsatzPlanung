namespace Einsatzplanung.GUI.Helpers;

using Avalonia.Media;

public static class ColorExtensions {
	public static string ToRgbHex(this Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
}
