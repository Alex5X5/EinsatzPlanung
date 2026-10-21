namespace Einsatzplanung.GUI.Converters;

using System;
using System.Globalization;

using Avalonia;
using Avalonia.Data.Converters;

public sealed class HalfBoundsToMarginConverter : IValueConverter {
	public static readonly HalfBoundsToMarginConverter Instance = new();

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		=> value is Rect r ? new Thickness(0, r.Height / 2, r.Width / 2, 0) : new Thickness(0);

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		=> throw new NotSupportedException();
}