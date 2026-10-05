namespace Einsatzplanung.GUI.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

using System;
using System.Windows.Input;

public partial class RemovableCard : ContentControl {
	
	protected override Type StyleKeyOverride => typeof(RemovableCard);

	public static readonly StyledProperty<bool> IsActiveProperty =
		AvaloniaProperty.Register<RemovableCard, bool>(nameof(IsActive));

	public static readonly StyledProperty<ICommand?> RemoveButtonCommandProperty =
		AvaloniaProperty.Register<RemovableCard, ICommand?>(nameof(RemoveButtonCommand));

	public bool IsActive {
		get => GetValue(IsActiveProperty);
		set => SetValue(IsActiveProperty, value);
	}

	public ICommand? RemoveButtonCommand {
		get => GetValue(RemoveButtonCommandProperty);
		set => SetValue(RemoveButtonCommandProperty, value);
	}

	private Panel? _partRoot;

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
		base.OnApplyTemplate(e);

		if (_partRoot is not null) {
			_partRoot.PointerEntered -= OnBorderPointerEntered;
			_partRoot.PointerExited -= OnBorderPointerExited;
		}

		_partRoot = e.NameScope.Find<Panel>("PART_Root");

		if (_partRoot is not null) {
			_partRoot.PointerEntered += OnBorderPointerEntered;
			_partRoot.PointerExited += OnBorderPointerExited;
		}
	}

	private void OnBorderPointerEntered(object? sender, PointerEventArgs e) {
		IsActive = true;
	}

	private void OnBorderPointerExited(object? sender, PointerEventArgs e) {
		if (_partRoot is null)
			return;

		var position = e.GetPosition(_partRoot);
		var bounds = new Rect(10, 10, _partRoot.Bounds.Width - 20, _partRoot.Bounds.Height - 20);

		if (bounds.Contains(position))
			return;

		IsActive = false;
	}

	static RemovableCard() {

		IsActiveProperty.Changed.AddClassHandler<RemovableCard>(
			(c, e) => c.PseudoClasses.Set(":active", (bool)e.NewValue!));

	}
}
