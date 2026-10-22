namespace Einsatzplanung.GUI.Converters;

using Avalonia;
using Avalonia.Data.Converters;

using System;
using System.Collections.Generic;
using System.Globalization;

public sealed class WidthAndHeightToMarginConverter : IMultiValueConverter {
	
	public static readonly WidthAndHeightToMarginConverter Instance = new();

	public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture) {
		if (values.Count != 2)
			return new Thickness(0);
		if (values[0] is not double width)
			return new Thickness(0);
		if (values[1] is not double height)
			return new Thickness(0);
		double size = Math.Max(width, height);
		return new Thickness(0, size / 2, size / 2, 0);
	}
}