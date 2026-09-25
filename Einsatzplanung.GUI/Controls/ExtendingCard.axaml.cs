namespace Einsatzplanung.GUI.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;

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

	private Border? _partBorder;

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
		base.OnApplyTemplate(e);

		if (_partBorder is not null) {
			_partBorder.PointerEntered -= OnBorderPointerEntered;
			_partBorder.PointerExited -= OnBorderPointerExited;
		}

		_partBorder = e.NameScope.Find<Border>("PART_Border");

		if (_partBorder is not null) {
			_partBorder.PointerEntered += OnBorderPointerEntered;
			_partBorder.PointerExited += OnBorderPointerExited;
		}
	}

	private void OnBorderPointerEntered(object? sender, PointerEventArgs e) {
		IsActive = true;
	}

	private void OnBorderPointerExited(object? sender, PointerEventArgs e) {
		if (_partBorder is null)
			return;

		var position = e.GetPosition(_partBorder);
		var bounds = new Rect(_partBorder.Bounds.Size);

		if (bounds.Contains(position))
			return;

		IsActive = false;
	}

	static ExtendingCard() {

		IsActiveProperty.Changed.AddClassHandler<ExtendingCard>(
			(c, e) => {
				bool newValue = (c.AllowExtend) ? (bool)e.NewValue! : false;
				if (newValue) {
					c.PseudoClasses.Set(":active", true);
				} else {
					c.PseudoClasses.Remove(":active");
				}
			});
	}
}
