namespace Einsatzplanung.GUI.Controls;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.CodeGenerators.Attributes;

[TemplatePart(PartRowsPanel, typeof(StackPanel))]
public partial class LevelItemsControl : TemplatedControl {
	/// <summary>Name of the vertical StackPanel in the control template that hosts the rows.</summary>
	public const string PartRowsPanel = "PART_RowsPanel";

	private const double DragThreshold = 4;
	
	/// <summary>A collection of rows; each row is an IList of items (e.g. ObservableCollection&lt;ObservableCollection&lt;T&gt;&gt;).</summary>
	[BasicStyledProperty]
	private IEnumerable? itemsSource = null;

	/// <summary>Template used to display a single item.</summary>
	[BasicStyledProperty]
	private IDataTemplate? itemTemplate = null;
	
	[BasicStyledProperty]
	private IBrush insertionIndicatorBrush = Avalonia.Media.Brushes.DodgerBlue;

	[BasicStyledProperty]
	private double draggedItemOpacity = 0.7;

	[BasicStyledProperty]
	private double horizontalSpacing = 20;
	[BasicStyledProperty]
	private double verticalSpacing = 20;

	/// <summary>
	/// Empty space reserved above the first and below the last row. Dropping an item
	/// there (or anywhere further above/below) creates a new row. Set to 0 to disable the extra space.
	/// </summary>
	[BasicStyledProperty]
	private double newRowDropZoneHeight = 20;

	/// <summary>
	/// Fraction (0..0.5) of a row's height, measured from its top and bottom edge, that counts as
	/// "between two rows" when dragging. Together with the space between the rows this is the drop area
	/// that inserts a new row there. Default 0.1 (10%).
	/// </summary>
	[BasicStyledProperty]
	private double newRowEdgeFraction = 0.1;
	
	/// <summary>
	/// Optional factory for new rows. If null, a row of the same type as the source row is created
	/// (works for ObservableCollection&lt;T&gt; and List&lt;T&gt;).
	/// </summary>
	public Func<IList>? NewRowFactory {
		get; set;
	}

	public event EventHandler<ItemMovedEventArgs>? ItemMoved;

	[BasicStyledProperty]
	private IRelayCommand<ItemMovedEventArgs> itemMovedCommand;

	// ------------------------------------------------------------------ fields

	private StackPanel? _rowsPanel; // from the control template (PART_RowsPanel)

	private readonly List<INotifyCollectionChanged> _subscribed = new();
	private bool _suppressRebuild;

	// drag state
	private Control? _dragged;
	private Panel? _draggedRow;
	private int _dragRow = -1, _dragCol = -1;
	private Point _pressPoint;
	private bool _dragging;
	private IPointer? _pointer;
	private DropKind _dropKind = DropKind.None;
	private int _dropRow = -1, _dropIndex = -1;
	private InsertionAdorner? _adorner;

	// ------------------------------------------------------------------ setup

	static LevelItemsControl() {
		// Default template: just the vertical panel that hosts the rows.
		// Replace the Template (or supply a ControlTheme) to restyle; keep a StackPanel named PART_RowsPanel.
		TemplateProperty.OverrideDefaultValue<LevelItemsControl>(
			new FuncControlTemplate<LevelItemsControl>((owner, scope) => {
				var panel = new StackPanel { Orientation = Orientation.Vertical, Spacing = 10 };
				scope.Register(PartRowsPanel, panel);
				return panel;
			}));
	}

	public LevelItemsControl() {
		AddHandler(PointerPressedEvent, OnPointerPressedTunnel, RoutingStrategies.Tunnel);
		AddHandler(PointerMovedEvent, OnPointerMovedHandler, RoutingStrategies.Bubble | RoutingStrategies.Tunnel, true);
		AddHandler(PointerReleasedEvent, OnPointerReleasedHandler, RoutingStrategies.Bubble | RoutingStrategies.Tunnel, true);
		AddHandler(PointerCaptureLostEvent, (_, _) => ResetDrag(), RoutingStrategies.Bubble, true);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
		base.OnPropertyChanged(change);

		if (change.Property == ItemsSourceProperty) {
			if (change.OldValue is INotifyCollectionChanged oldNcc)
				oldNcc.CollectionChanged -= OnCollectionChanged;
			if (change.NewValue is INotifyCollectionChanged newNcc)
				newNcc.CollectionChanged += OnCollectionChanged;
			Rebuild();
		} else if (change.Property == ItemTemplateProperty) {
			Rebuild();
		} else if (change.Property == NewRowDropZoneHeightProperty || change.Property == PaddingProperty) {
			UpdateZoneMargin();
		}
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
		base.OnApplyTemplate(e);

		ResetDrag();
		UnsubscribeRows();
		_rowsPanel?.Children.Clear(); // old template's panel

		_rowsPanel = e.NameScope.Find<StackPanel>(PartRowsPanel);
		UpdateZoneMargin();
		Rebuild();
	}

	public override void Render(DrawingContext context) {
		// TemplatedControl does not paint its own Background.
		if (Background != null)
			context.FillRectangle(Background, new Rect(Bounds.Size));
		base.Render(context);
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) {
		base.OnAttachedToVisualTree(e);
		if (ItemsSource is INotifyCollectionChanged ncc) {
			ncc.CollectionChanged -= OnCollectionChanged;
			ncc.CollectionChanged += OnCollectionChanged;
		}
		Rebuild();
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) {
		ResetDrag();
		if (ItemsSource is INotifyCollectionChanged ncc)
			ncc.CollectionChanged -= OnCollectionChanged;
		UnsubscribeRows();
		base.OnDetachedFromVisualTree(e);
	}

	private void UpdateZoneMargin() {
		if (_rowsPanel is null)
			return;

		// Padding plus the reserved "new row" drop zones above and below the rows.
		double z = Math.Max(0, NewRowDropZoneHeight);
		var pad = Padding;
		_rowsPanel.Margin = new Thickness(pad.Left, pad.Top + z, pad.Right, pad.Bottom + z);
	}

	private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
		if (!_suppressRebuild)
			Rebuild();
	}

	// ------------------------------------------------------------------ building the visual rows

	private void UnsubscribeRows() {
		foreach (var s in _subscribed)
			s.CollectionChanged -= OnCollectionChanged;
		_subscribed.Clear();
	}

	private void Rebuild() {
		ResetDrag();
		UnsubscribeRows();

		if (_rowsPanel is null)
			return;

		_rowsPanel.Children.Clear();

		if (ItemsSource is null)
			return;

		foreach (var row in ItemsSource) {
			// Always add a panel per row so panel index == row index in ItemsSource.
			var rowPanel = new StackPanel {
				Orientation = Orientation.Horizontal,
				Spacing = HorizontalSpacing,
			};

			if (row is INotifyCollectionChanged ncc) {
				ncc.CollectionChanged += OnCollectionChanged;
				_subscribed.Add(ncc);
			}

			if (row is IEnumerable items && row is not string) {
				foreach (var item in items) {
					rowPanel.Children.Add(new ContentPresenter {
						Content = item,
						ContentTemplate = ItemTemplate
					});
				}
			}

			_rowsPanel.Children.Add(rowPanel);
		}
	}

	// ------------------------------------------------------------------ geometry helpers
	// All rectangles are in the coordinate space of this control and are computed from the
	// untransformed Bounds, so the item currently being dragged still reports its original slot.

	// The rows panel lives inside the template, so find its origin relative to this control.
	private Point PanelOrigin() => _rowsPanel?.TranslatePoint(default, this) ?? default;

	private Rect RowRect(Panel row) =>
		new(PanelOrigin() + row.Bounds.Position, row.Bounds.Size);

	private Rect ItemRect(Panel row, Control item) =>
		new(PanelOrigin() + row.Bounds.Position + item.Bounds.Position, item.Bounds.Size);

	private List<Panel> RowPanels() =>
		_rowsPanel?.Children.OfType<Panel>().ToList() ?? new List<Panel>();

	// ------------------------------------------------------------------ pointer handling

	private void OnPointerPressedTunnel(object? sender, PointerPressedEventArgs e) {
		if (_dragged != null)
			return;
		if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
			return;

		var p = e.GetPosition(this);
		var rows = RowPanels();
		for (int r = 0; r < rows.Count; r++) {
			for (int c = 0; c < rows[r].Children.Count; c++) {
				if (ItemRect(rows[r], rows[r].Children[c]).Contains(p)) {
					_dragged = rows[r].Children[c];
					_draggedRow = rows[r];
					_dragRow = r;
					_dragCol = c;
					_pressPoint = p;
					_dragging = false;
					return;
				}
			}
		}
	}

	private void OnPointerMovedHandler(object? sender, PointerEventArgs e) {
		if (_dragged is null)
			return;

		if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) {
			ResetDrag();
			return;
		}

		var p = e.GetPosition(this);

		if (!_dragging) {
			var d = p - _pressPoint;
			if (Math.Sqrt(d.X * d.X + d.Y * d.Y) < DragThreshold)
				return;

			_dragging = true;
			_pointer = e.Pointer;
			e.Pointer.Capture(this);

			// Lift the item (and its row, so it draws above the following rows).
			_dragged.ZIndex = 1000;
			_dragged.Opacity = DraggedItemOpacity;
			if (_draggedRow != null)
				_draggedRow.ZIndex = 1000;
		}

		_dragged.RenderTransform = new TranslateTransform(p.X - _pressPoint.X, p.Y - _pressPoint.Y);
		UpdateDropTarget(p);
		e.Handled = true;
	}

	private void OnPointerReleasedHandler(object? sender, PointerReleasedEventArgs e) {
		if (_dragged is null)
			return;

		bool wasDragging = _dragging;
		int srcRow = _dragRow, srcCol = _dragCol;
		var kind = _dropKind;
		int dropRow = _dropRow, dropIndex = _dropIndex;
		ResetDrag();

		if (!wasDragging || kind == DropKind.None)
			return;

		e.Handled = true;
		MoveItem(srcRow, srcCol, kind, dropRow, dropIndex);
	}

	// ------------------------------------------------------------------ drop target

	private void UpdateDropTarget(Point p) {
		var rows = RowPanels();
		if (rows.Count == 0) {
			ClearTarget();
			return;
		}

		var first = RowRect(rows[0]);
		var last = RowRect(rows[^1]);
		int srcCount = rows[_dragRow].Children.Count;

		double zone = Math.Max(0, NewRowDropZoneHeight);

		// Above the top row -> new row at the top.
		if (p.Y < first.Top) {
			SetNewRowTarget(0, srcCount, new Rect(0, first.Top - zone / 2 - 1.5, Bounds.Width, 3));
			return;
		}

		// Below the bottom row -> new row at the bottom.
		if (p.Y > last.Bottom) {
			SetNewRowTarget(rows.Count, srcCount, new Rect(0, last.Bottom + zone / 2 - 1.5, Bounds.Width, 3));
			return;
		}

		// Between two rows: bottom part of the upper row, the gap, and top part of the lower row.
		double fraction = Math.Clamp(NewRowEdgeFraction, 0, 0.5);
		for (int k = 1; k < rows.Count; k++) {
			var upper = RowRect(rows[k - 1]);
			var lower = RowRect(rows[k]);
			double zoneTop = upper.Bottom - upper.Height * fraction;
			double zoneBottom = lower.Top + lower.Height * fraction;

			if (p.Y >= zoneTop && p.Y <= zoneBottom) {
				double y = (upper.Bottom + lower.Top) / 2;
				SetNewRowTarget(k, srcCount, new Rect(0, y - 1.5, Bounds.Width, 3));
				return;
			}
		}

		// Otherwise: pick the row whose vertical band contains the pointer (or the nearest one).
		int bestRow = 0;
		double bestDist = double.MaxValue;
		for (int r = 0; r < rows.Count; r++) {
			var rr = RowRect(rows[r]);
			double dy = p.Y < rr.Top ? rr.Top - p.Y : p.Y > rr.Bottom ? p.Y - rr.Bottom : 0;
			if (dy < bestDist) {
				bestDist = dy;
				bestRow = r;
			}
		}

		var rowPanel = rows[bestRow];
		var rowRect = RowRect(rowPanel);

		// Insert before the first item whose centre is right of the pointer, else at the end.
		int index = rowPanel.Children.Count;
		double x = rowPanel.Children.Count > 0
			? ItemRect(rowPanel, rowPanel.Children[^1]).Right
			: rowRect.Left;

		for (int c = 0; c < rowPanel.Children.Count; c++) {
			var ir = ItemRect(rowPanel, rowPanel.Children[c]);
			if (p.X < ir.Center.X) {
				index = c;
				x = ir.Left;
				break;
			}
		}

		if (bestRow == _dragRow && (index == _dragCol || index == _dragCol + 1)) {
			ClearTarget(); // dropping at the same place
			return;
		}

		_dropKind = DropKind.Row;
		_dropRow = bestRow;
		_dropIndex = index;
		ShowIndicator(new Rect(x - 1.5, rowRect.Top, 3, rowRect.Height));
	}

	/// <summary>Targets a new row inserted at <paramref name="insertAt"/> (index among the existing rows).</summary>
	private void SetNewRowTarget(int insertAt, int srcCount, Rect indicator) {
		// A lone item moved to a new row right next to its own row would end up where it is.
		if (srcCount == 1 && (insertAt == _dragRow || insertAt == _dragRow + 1)) {
			ClearTarget();
			return;
		}

		_dropKind = DropKind.NewRow;
		_dropRow = insertAt;
		_dropIndex = 0;
		ShowIndicator(indicator);
	}

	private void ClearTarget() {
		_dropKind = DropKind.None;
		_dropRow = _dropIndex = -1;
		HideIndicator();
	}

	// ------------------------------------------------------------------ modifying the data

	private bool IsValidList(IList src) {
		if (src.IsReadOnly)
			return false;
		if (src.IsFixedSize)
			return false;
		return true;
	}

	private bool IsIndexInList(IList list, int index) {
		if (index < 0)
			return false;
		if (index >= list.Count)
			return false;
		return true;
	}

	private void MoveItem(int oldRowIndex, int oldColIndex, DropKind kind, int dropRowIndex, int dropColIndex) {
		if (ItemsSource is not IList rows)
			return;
		if (!IsValidList(rows))
			return;
		if (!IsIndexInList(rows, oldRowIndex))
			return;
		if (rows[oldRowIndex] is not IList row)
			return;
		if (!IsValidList(row))
			return;
		if (!IsIndexInList(row, oldColIndex))
			return;

		var item = row[oldColIndex];
		int newRow, newCol;

		_suppressRebuild = true;
		try {
			if (kind == DropKind.Row) {
				if (!IsIndexInList(rows, dropRowIndex))
					return;
				if (rows[dropRowIndex] is not IList target)
					return;
				if (!IsValidList(target))
					return;

				if (ReferenceEquals(row, target)) {
					int ni = dropColIndex > oldColIndex ? dropColIndex - 1 : dropColIndex;
					if (ni == oldColIndex)
						return;
					MoveWithin(row, oldColIndex, ni);
					newRow = oldRowIndex;
					newCol = ni;
				} else {
					row.RemoveAt(oldColIndex);
					target.Insert(Math.Clamp(dropColIndex, 0, target.Count), item);
					if (row.Count == 0)
						RemoveRow(rows, row);
					newRow = IndexOfRef(rows, target);
					newCol = Math.Clamp(dropColIndex, 0, target.Count - 1);
				}
			} else { // DropKind.NewRow: a new row inserted at index dropRow
				var created = CreateRow(row);
				if (created is null)
					return;

				row.RemoveAt(oldColIndex);
				created.Add(item);
				rows.Insert(Math.Clamp(dropRowIndex, 0, rows.Count), created);

				if (row.Count == 0)
					RemoveRow(rows, row);

				newRow = IndexOfRef(rows, created);
				newCol = 0;
			}
		} finally {
			_suppressRebuild = false;
			Rebuild();
		}

		var movedArgs = new ItemMovedEventArgs(item, oldRowIndex, oldColIndex, newRow, newCol);
		ItemMoved?.Invoke(this, movedArgs);
		if (ItemMovedCommand?.CanExecute(movedArgs) ?? false)
			ItemMovedCommand.Execute(movedArgs);
	}

	private static void MoveWithin(IList list, int oldIndex, int newIndex) {
		// ObservableCollection<T>.Move raises one clean "Move" notification.
		var move = list.GetType().GetMethod("Move", new[] { typeof(int), typeof(int) });
		if (move != null) {
			move.Invoke(list, new object[] { oldIndex, newIndex });
			return;
		}
		var item = list[oldIndex];
		list.RemoveAt(oldIndex);
		list.Insert(newIndex, item);
	}

	private IList? CreateRow(IList template) {
		if (NewRowFactory != null)
			return NewRowFactory();
		try {
			return Activator.CreateInstance(template.GetType()) as IList;
		} catch {
			return null; // set NewRowFactory for row types without a parameterless constructor
		}
	}

	private static int IndexOfRef(IList list, object value) {
		for (int i = 0; i < list.Count; i++)
			if (ReferenceEquals(list[i], value))
				return i;
		return -1;
	}

	private static void RemoveRow(IList rows, object row) {
		int i = IndexOfRef(rows, row);
		if (i >= 0)
			rows.RemoveAt(i);
	}

	// ------------------------------------------------------------------ visuals / cleanup

	private void ResetDrag() {
		var dragged = _dragged;
		var row = _draggedRow;
		var pointer = _pointer;

		_dragged = null;
		_draggedRow = null;
		_pointer = null;
		_dragging = false;
		_dragRow = _dragCol = -1;
		_dropKind = DropKind.None;
		_dropRow = _dropIndex = -1;

		if (dragged != null) {
			dragged.RenderTransform = null;
			dragged.ZIndex = 0;
			dragged.Opacity = 1;
		}
		if (row != null)
			row.ZIndex = 0;

		HideIndicator();

		if (pointer?.Captured == this)
			pointer.Capture(null);
	}

	private void ShowIndicator(Rect rect) {
		if (_adorner is null) {
			var layer = AdornerLayer.GetAdornerLayer(this);
			if (layer is null)
				return;

			_adorner = new InsertionAdorner();
			AdornerLayer.SetAdornedElement(_adorner, this);
			layer.Children.Add(_adorner);
		}

		_adorner.Brush = InsertionIndicatorBrush;
		_adorner.Line = rect;
		_adorner.InvalidateVisual();
	}

	private void HideIndicator() {
		if (_adorner is null)
			return;

		if (_adorner.Parent is Panel layer)
			layer.Children.Remove(_adorner);
		_adorner = null;
	}


	private enum DropKind {
		None, Row, NewRow
	}


	private sealed class InsertionAdorner : Control {
		public Rect? Line { set; get; }
		public IBrush? Brush { set; get; }

		public InsertionAdorner() => IsHitTestVisible = false;

		public override void Render(DrawingContext context) {
			if (Line != null && Brush != null)
				context.DrawRectangle(Brush, null, new RoundedRect(Line.Value, 1.5));
		}
	}
}

public sealed class ItemMovedEventArgs : EventArgs {
	public ItemMovedEventArgs(object? item, int oldRow, int oldColumn, int newRow, int newColumn) {
		Item = item;
		OldRow = oldRow;
		OldColumn = oldColumn;
		NewRow = newRow;
		NewColumn = newColumn;
	}

	public object? Item {
		get;
	}
	public int OldRow {
		get;
	}
	public int OldColumn {
		get;
	}
	public int NewRow {
		get;
	}
	public int NewColumn {
		get;
	}
}
