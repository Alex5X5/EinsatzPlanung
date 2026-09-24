namespace Einsatzplanung.GUI.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

using System;

public partial class ExtendingCard : ContentControl {
	
	protected override Type StyleKeyOverride => typeof(ExtendingCard);

	public static readonly StyledProperty<object?> HeaderContentProperty =
	AvaloniaProperty.Register<ExtendingCard, object?>(nameof(HeaderContent));

	public static readonly StyledProperty<IDataTemplate?> HeaderContentTemplateProperty =
		AvaloniaProperty.Register<ExtendingCard, IDataTemplate?>(nameof(HeaderContentTemplate));

	public object? HeaderContent {
		get => GetValue(HeaderContentProperty);
		set => SetValue(HeaderContentProperty, value);
	}

	public IDataTemplate? HeaderContentTemplate {
		get => GetValue(HeaderContentTemplateProperty);
		set => SetValue(HeaderContentTemplateProperty, value);
	}

	public static readonly StyledProperty<bool> IsActiveProperty =
		AvaloniaProperty.Register<ExtendingCard, bool>(nameof(IsActive));

	public bool IsActive {
		get => GetValue(IsActiveProperty);
		set => SetValue(IsActiveProperty, value);
	}

	static ExtendingCard() {
		IsActiveProperty.Changed.AddClassHandler<ExtendingCard>(
			(c, e) => c.PseudoClasses.Set(":active", (bool)e.NewValue!));
	}
}
