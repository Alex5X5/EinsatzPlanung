namespace Einsatzplanung.GUI.Controls;

using System;

using CommunityToolkit.Mvvm.Input;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Metadata;

using Einsatzplanung.GUI.CodeGenerators.Attributes;


public partial class ExtendingCard : UserControl {

	protected override Type StyleKeyOverride => typeof(ExtendingCard);

	[BasicStyledProperty<ExtendingCard>]
	private Orientation headerOrientation = Avalonia.Layout.Orientation.Vertical;

	[BasicStyledProperty<ExtendingCard>]
	private Orientation bodyOrientation = Avalonia.Layout.Orientation.Vertical;

	[BasicStyledProperty<ExtendingCard>]
	private bool allowExtend = true;

	[BasicStyledProperty<ExtendingCard>]
	private bool showAddButton = true;

	[BasicStyledProperty<ExtendingCard>]
	private bool isActive = false;

	[BasicStyledProperty<ExtendingCard>]
	private RelayCommand? addButtonCommand;

	private readonly Controls _children = new();
	private readonly Controls _headerContent = new();

	private Border? _partBorder;
	private StackPanel? _headerPanel;
	private StackPanel? _bodyPanel;


	[Content]
	public Controls Children => _children;

	public Controls HeaderContent {
		get => _headerContent;
	}

	public ExtendingCard() {
		_children.CollectionChanged += (_, _) => SyncPanel(_bodyPanel, _children);
		_headerContent.CollectionChanged += (_, _) => SyncPanel(_headerPanel, _headerContent);
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

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
		base.OnApplyTemplate(e);

		_headerPanel?.Children.Clear();
		_bodyPanel?.Children.Clear();

		_headerPanel = e.NameScope.Find<StackPanel>("PART_HeaderPanel");
		_bodyPanel = e.NameScope.Find<StackPanel>("PART_BodyPanel");

		SyncPanel(_headerPanel, _headerContent);
		SyncPanel(_bodyPanel, _children);

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

	private static void SyncPanel(Panel? panel, Controls source) {
		if (panel is null)
			return;
		panel.Children.Clear();
		panel.Children.AddRange(source);
	}

	private void OnBorderPointerEntered(object? sender, PointerEventArgs e) {
		IsActive = true;
	}

	private void OnBorderPointerExited(object? sender, PointerEventArgs e) {
		if (_partBorder is null)
			return;

		var position = e.GetPosition(_partBorder);
		var bounds = new Rect(10, 10, _partBorder.Bounds.Width - 20, _partBorder.Bounds.Height - 20);

		if (bounds.Contains(position))
			return;

		IsActive = false;
	}

	protected override Size MeasureOverride(Size availableSize) {
		_partBorder?.Measure(availableSize);
		return _partBorder?.DesiredSize ?? new Size(0.0, 0.0);
	}

	protected override Size ArrangeOverride(Size finalSize) {
		_partBorder?.Arrange(new Rect(finalSize));
		return finalSize;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
		base.OnPropertyChanged(change);

		if (change.Property == HeaderOrientationProperty)
			if (_headerPanel is not null)
				_headerPanel.Orientation = change.GetNewValue<Orientation>();

		if (change.Property == BodyOrientationProperty)
			if (_bodyPanel is not null)
				_bodyPanel.Orientation = change.GetNewValue<Orientation>();
	}
}
