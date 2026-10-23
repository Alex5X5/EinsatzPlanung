using System.Windows.Input;

namespace Einsatzplanung.GUI.Controls;

using Avalonia;
using Avalonia.Controls;


public class YearTabControl : TabControl
{
	public static readonly StyledProperty<ICommand?> AddTabCommandProperty =
		AvaloniaProperty.Register<YearTabControl, ICommand?>(nameof(AddTabCommand));

	public static readonly StyledProperty<ICommand?> RemoveTabCommandProperty =
		AvaloniaProperty.Register<YearTabControl, ICommand?>(nameof(RemoveTabCommand));

	public ICommand? AddTabCommand
	{
		get => GetValue(AddTabCommandProperty);
		set => SetValue(AddTabCommandProperty, value);
	}

	public ICommand? RemoveTabCommand
	{
		get => GetValue(RemoveTabCommandProperty);
		set => SetValue(RemoveTabCommandProperty, value);
	}
}}