namespace Einsatzplanung.GUI.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

using System;
using System.Windows.Input;

public partial class ExtendingCard : ContentControl {
	
	protected override Type StyleKeyOverride => typeof(ExtendingCard);

	public static readonly StyledProperty<object?> HeaderContentProperty =
	AvaloniaProperty.Register<ExtendingCard, object?>(nameof(HeaderContent));

	public static readonly StyledProperty<IDataTemplate?> HeaderContentTemplateProperty =
		AvaloniaProperty.Register<ExtendingCard, IDataTemplate?>(nameof(HeaderContentTemplate));

	public static readonly StyledProperty<bool> IsActiveProperty =
		AvaloniaProperty.Register<ExtendingCard, bool>(nameof(IsActive));

	public static readonly StyledProperty<ICommand?> AddButtonCommandProperty =
		AvaloniaProperty.Register<ExtendingCard, ICommand?>(nameof(AddButtonCommand));

	public static readonly StyledProperty<bool> AllowExtendProperty =
		AvaloniaProperty.Register<ExtendingCard, bool>(nameof(AllowExtend), defaultValue:true);

	public object? HeaderContent {
		get => GetValue(HeaderContentProperty);
		set => SetValue(HeaderContentProperty, value);
	}

	public IDataTemplate? HeaderContentTemplate {
		get => GetValue(HeaderContentTemplateProperty);
		set => SetValue(HeaderContentTemplateProperty, value);
	}

	public bool IsActive {
		get => GetValue(IsActiveProperty);
		set => SetValue(IsActiveProperty, value);
	}

	public ICommand? AddButtonCommand {
		get => GetValue(AddButtonCommandProperty);
		set => SetValue(AddButtonCommandProperty, value);
	}

	public bool AllowExtend {
		get => GetValue(AllowExtendProperty);
		set => SetValue(AllowExtendProperty, value);
	}

	static ExtendingCard() {
		IsPointerOverProperty.Changed.AddClassHandler<ExtendingCard>(
			(c, e) => {
				bool newValue = (c.AllowExtend) ? (bool)e.NewValue! : false;
				if (newValue) {
					Console.WriteLine("setting active");
					c.PseudoClasses.Set(":active", true);
				} else {
					Console.WriteLine("removing active");
					c.PseudoClasses.Remove(":active");
				}
			});
		//IsActiveProperty.Changed.AddClassHandler<ExtendingCard>(
		//	(c, e) => c.PseudoClasses.Set(":active", c.AllowExtend ? (bool)e.NewValue! : false ));
	}
}
