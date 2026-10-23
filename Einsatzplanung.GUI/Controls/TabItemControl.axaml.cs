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


public partial class TabItemControl : UserControl {

	protected override Type StyleKeyOverride => typeof(ExtendingCard);

	[BasicStyledProperty<ExtendingCard>]
	private Orientation headerOrientation = Avalonia.Layout.Orientation.Horizontal;

	[BasicStyledProperty<ExtendingCard>]
	private Orientation bodyOrientation = Avalonia.Layout.Orientation.Vertical;

	[BasicStyledProperty<ExtendingCard>]
	private bool allowExtend = true;

	[BasicStyledProperty<ExtendingCard>]
	private double extendedHeight = 300.0;

	[BasicStyledProperty<ExtendingCard>]
	private bool showAddButton = true;

	[BasicStyledProperty<ExtendingCard>]
	private int activeCardIndex = -1;

	[BasicStyledProperty<ExtendingCard>]
	private int index = 0;

	[BasicStyledProperty<ExtendingCard>]
	private bool isActive = false;

	[BasicStyledProperty<ExtendingCard>]
	private IRelayCommand? addButtonCommand;

	[BasicStyledProperty<ExtendingCard>]
	private IRelayCommand? removeButtonCommand;

	[BasicStyledProperty<ExtendingCard>]
	private object? removeButtonCommandParameter;

	private readonly Controls _children = new();
	private readonly Controls _headerContent = new();

	private Panel? _partRoot;
	private Border? _partBorder;
	private DockPanel? _headerPanel;
	private StackPanel? _bodyPanel;
	private ScrollViewer? _bodyScrollViewer;
	private Button? _partRemoveButton;


	[Content]
	public Controls Children => _children;

	public Controls HeaderContent {
		get => _headerContent;
	}

	public TabItemControl() {
		_children.CollectionChanged += (_, _) => SyncPanel(_bodyPanel, _children);
		_headerContent.CollectionChanged += (_, _) => SyncHeader();
	}

	static TabItemControl() {

		IndexProperty.Changed.AddClassHandler<TabItemControl>(
			(c, e) => {
				c.IsActive = c.ActiveCardIndex == (int)e.NewValue!;
			});

		ActiveCardIndexProperty.Changed.AddClassHandler<TabItemControl>(
			(c, e) => {
				c.IsActive = c.Index == (int)e.NewValue!;
			});

		IsActiveProperty.Changed.AddClassHandler<TabItemControl>(
			(c, e) => {
				var newValue = (c.AllowExtend) ? (bool)e.NewValue! : false;
				c.PseudoClasses.Set(":active", newValue);
			});
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
		base.OnApplyTemplate(e);

		_headerPanel?.Children.Clear();
		_bodyPanel?.Children.Clear();

		_headerPanel = e.NameScope.Find<DockPanel>("PART_HeaderPanel");
		_bodyPanel = e.NameScope.Find<StackPanel>("PART_BodyPanel");

		SyncHeader();
		SyncPanel(_bodyPanel, _children);

		if (_partBorder is not null)
			_partBorder.PointerEntered -= OnBorderPointerEntered;

		_partBorder = e.NameScope.Find<Border>("PART_Border");

		if (_partBorder is not null)
			_partBorder.PointerEntered += OnBorderPointerEntered;

		_bodyScrollViewer?.PointerWheelChanged -= OnScroll;
		_bodyScrollViewer = e.NameScope.Find<ScrollViewer>("PART_Body");
		_bodyScrollViewer?.PointerWheelChanged += OnScroll;

		_partRoot = e.NameScope.Find<Panel>("PART_Root");

		_partRemoveButton = e.NameScope.Find<Button>("PART_RemoveButton");
	}

	private static void SyncPanel(Panel? panel, Controls source) {
		if (panel is null)
			return;
		panel.Children.Clear();
		panel.Children.AddRange(source);
	}
	
	private void SyncHeader() {
		SyncPanel(_headerPanel, _headerContent);
		ApplyHeaderOrientation();
	}
	
	private void ApplyHeaderOrientation() {
		if (_headerPanel is null)
			return;

		var side = HeaderOrientation == Orientation.Horizontal ? Dock.Left : Dock.Top;

		foreach (var child in _headerPanel.Children)
			DockPanel.SetDock(child, side);
	}

	public void OnScroll(object? sender, PointerWheelEventArgs e) {
		if (sender is not ScrollViewer sv)
			return;
		e.Handled = true;
	}

	private void OnBorderPointerEntered(object? sender, PointerEventArgs e) {
		ActiveCardIndex = Index;
	}

	protected override Size MeasureOverride(Size availableSize) {
		_partRoot?.Measure(availableSize);
		return _partRoot?.DesiredSize ?? new Size(0.0, 0.0);
	}

	protected override Size ArrangeOverride(Size finalSize) {
		_partRoot?.Arrange(new Rect(finalSize));
		return finalSize;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
		base.OnPropertyChanged(change);

		if (change.Property == HeaderOrientationProperty)
			ApplyHeaderOrientation();

		if (change.Property == BodyOrientationProperty)
			if (_bodyPanel is not null)
				_bodyPanel.Orientation = change.GetNewValue<Orientation>();

		if (change.Property == RemoveButtonCommandParameterProperty) {
			if(_partRemoveButton is not null)
				_partRemoveButton.CommandParameter = change.GetNewValue<object>();
		}
	}
}