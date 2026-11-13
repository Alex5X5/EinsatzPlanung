namespace Einsatzplanung.GUI.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;

using DocumentFormat.OpenXml.Drawing.Charts;

using Einsatzplanung.GUI.CodeGenerators.Attributes;
using Einsatzplanung.GUI.ViewModels.Edit;
using Einsatzplanung.GUI.Views.Edit;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public partial class TopicGroupsControl : ContentControl {
	
    [BasicStyledProperty]
    private IEnumerable<IEnumerable<object>>? itemsSource = null;
    private Control? draggedControl;

    private IPointer? dragPointer;
	private TopicCardViewModel? draggedCard;
	private Point dragStart;
	private OverlayLayer? overlay;
	private Point grabOffset;
	private Border? dragVisual;
	private Border? dropIndicator;
	private bool isDragging;
	private const double NewGroupZone = 20;
	private const double IndicatorThickness = 3;
	private const double IndicatorGap = 4;
	private sealed record GroupArea(ObservableCollection<TopicCardViewModel> Group, Rect Bounds);
	private sealed record DropTarget(ObservableCollection<TopicCardViewModel>? Group, int Position, Rect Indicator);
    private EditCardsControl? CardsControl;

	public TopicGroupsControl() : base() {
        
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
		base.OnApplyTemplate(e);

		if (CardsControl is not null) {
			CardsControl.PointerPressed -= OnTopicDragPressed;
			CardsControl.PointerMoved -= OnTopicDragMoved;
			CardsControl.PointerReleased -= OnTopicDragReleased;
		}

		CardsControl = e.NameScope.Find<EditCardsControl>("CardsControl");
		if (CardsControl is null)
			return;

		CardsControl.PointerPressed += OnTopicDragPressed;
		CardsControl.PointerMoved += OnTopicDragMoved;
		CardsControl.PointerReleased += OnTopicDragReleased;
	}

	private void OnTopicDragPressed(object? sender, PointerPressedEventArgs e) {
		if (e.Source is not Control handle || handle.DataContext is not TopicCardViewModel card)
			return;

		Control? container = null;
		for (Control? current = handle; current is not null; current = current.GetVisualParent() as Control) {
			if (ItemsControl.ItemsControlFromItemContainer(current) is EditCardsControl inner && !ReferenceEquals(inner, CardsControl)) {
				container = current;
				break;
			}
		}

		if (container is null)
			return;

		dragPointer = e.Pointer;
		draggedCard = card;
		draggedControl = container;
		dragStart = e.GetPosition(this);
		isDragging = false;
		e.Handled = true;
	}
	
	private void OnTopicDragMoved(object? sender, PointerEventArgs e) {
		if (dragPointer != e.Pointer || draggedCard is null || draggedControl is null || CardsControl is null)
			return;

		Vector dragDelta = e.GetPosition(this) - dragStart;

		if (!isDragging){
            if (Math.Sqrt(dragDelta.X * dragDelta.X + dragDelta.Y * dragDelta.Y) < 5)
                return;

            isDragging = true;
			e.Pointer.Capture(CardsControl);
			BeginDragViaual(e);
        } 

		UpdateDragVisual(e);
		UpdateDropIndicator(e);

	}

	private void UpdateDropIndicator(PointerEventArgs e) {
		if (dropIndicator is null || overlay is null)
			return;

		DropTarget? target = FindDropTarget(e.GetPosition(this).Y);
		if (target is null || this.TranslatePoint(target.Indicator.Position, overlay) is not Point indicatorPosition) {
			dropIndicator.IsVisible = false;
			return;
		}

		dropIndicator.Width = target.Indicator.Width;
		dropIndicator.Height = target.Indicator.Height;
		dropIndicator.RenderTransform = new TranslateTransform(indicatorPosition.X, indicatorPosition.Y);
		dropIndicator.IsVisible = true;
	}

	private void BeginDragViaual(PointerEventArgs e) {
		if (draggedControl is null) {
			return;
		}

		overlay = OverlayLayer.GetOverlayLayer(draggedControl);
		if (overlay is null) {
			return;
		}

		double scale = draggedControl.TransformToVisual(overlay)?.M11 ?? 1; 

		grabOffset = e.GetPosition(draggedControl) * scale;
		dragVisual = new Border {
			Width = draggedControl.Bounds.Width * scale,
			Height = draggedControl.Bounds.Height * scale,
			Background = new VisualBrush {
				Visual = draggedControl,
				Stretch = Stretch.Fill,
			},
			Opacity = 1,
			IsHitTestVisible = false
		};
		dropIndicator = new Border {
			Height = 3,
			CornerRadius = new CornerRadius(1.5),
			Background = Brushes.SkyBlue,
			IsHitTestVisible = false,
			IsVisible = false
		};
		overlay.Children.Add(dragVisual);
		overlay.Children.Add(dropIndicator);
		
		UpdateDragVisual(e);
	}

	private void UpdateDragVisual(PointerEventArgs e) {
		if (dragVisual is null || overlay is null)
			return;
		var p = e.GetPosition(overlay);
		dragVisual.RenderTransform = new TranslateTransform(p.X - grabOffset.X, p.Y - grabOffset.Y);
	}

	private void EndDragVisual() {
		if (dragVisual is not null)
			overlay?.Children.Remove(dragVisual);
		if (draggedControl is not null)
			draggedControl.Opacity = 1;
		if (dropIndicator is not null)
			overlay?.Children.Remove(dropIndicator);

		dropIndicator = null;
		dragVisual = null;
		overlay = null;
	}

	private void OnTopicDragReleased(object? sender, PointerReleasedEventArgs e) {
		if (dragPointer != e.Pointer)
			return;

		e.Pointer.Capture(null);
		EndDragVisual();
        if (draggedControl is not null) {
            draggedControl.RenderTransform = null;
            draggedControl.Opacity = 1;
            draggedControl.ZIndex = 0;
            draggedControl = null;
        }
		if (isDragging && FindDropTarget(e.GetPosition(this).Y) is DropTarget target)
			ApplyDrop(target);

		dragPointer = null;
		draggedCard = null;
		draggedControl = null;
		isDragging = false;
	}
	private void MoveCard(TopicCardViewModel card, ObservableCollection<TopicCardViewModel> target, int index) {
		if (ItemsSource is not IList groups)
			return;

		var source = groups.OfType<ObservableCollection<TopicCardViewModel>>().FirstOrDefault(g => g.Contains(card));
		if (source is null)
			return;

		int oldIndex = source.IndexOf(card);
		if (ReferenceEquals(source, target) && index > oldIndex)
			index--;

		source.RemoveAt(oldIndex);
		target.Insert(Math.Clamp(index, 0, target.Count), card);

		if (source.Count == 0)
			groups.Remove(source);
	}

	private DropTarget? FindDropTarget(double pointerY) {
		List<GroupArea> groupAreas = GetGroupAreas();
		if (groupAreas.Count == 0)
			return null;

		DropTarget? newGroupTarget = FindNewGroupTarget(pointerY, groupAreas);
		if (newGroupTarget is not null && !IsSamePlace(newGroupTarget, groupAreas))
			return newGroupTarget;

		DropTarget? cardTarget = FindCardTarget(pointerY, groupAreas);
		if (cardTarget is not null && !IsSamePlace(cardTarget, groupAreas))
			return cardTarget;

		return null;
	}

	private DropTarget? FindNewGroupTarget(double pointerY, List<GroupArea> groupAreas) {
		for (int gap = 0; gap <= groupAreas.Count; gap++) {
			bool isFirstGap = gap == 0;
			bool isLastGap = gap == groupAreas.Count;

			double gapTop = isFirstGap ? groupAreas[0].Bounds.Top : groupAreas[gap - 1].Bounds.Bottom;
			double gapBottom = isLastGap ? groupAreas[gap - 1].Bounds.Bottom : groupAreas[gap].Bounds.Top;

			bool isPointerAboveZone = !isFirstGap && pointerY < gapTop - NewGroupZone;
			bool isPointerBelowZone = !isLastGap && pointerY > gapBottom + NewGroupZone;
			if (isPointerAboveZone || isPointerBelowZone)
				continue;

			GroupArea neighbor = groupAreas[isLastGap ? gap - 1 : gap];
			double lineY = (gapTop + gapBottom) / 2;
			return new DropTarget(null, gap, HorizontalLine(neighbor.Bounds, lineY));
		}

		return null;
	}

	private DropTarget? FindCardTarget(double pointerY, List<GroupArea> groupAreas) {
		GroupArea? hoveredGroup = groupAreas.FirstOrDefault(area => area.Bounds.Top <= pointerY && pointerY <= area.Bounds.Bottom);
		if (hoveredGroup is null)
			return null;

		List<Rect> cardRects = GetCardRects(hoveredGroup.Group);
		if (cardRects.Count == 0)
			return new DropTarget(hoveredGroup.Group, 0, HorizontalLine(hoveredGroup.Bounds, hoveredGroup.Bounds.Center.Y));

		int cardIndex = cardRects.FindIndex(cardRect => pointerY < cardRect.Center.Y);
		if (cardIndex < 0)
			cardIndex = cardRects.Count;
		
		double lineY;
		if (cardIndex == 0)
			lineY = cardRects[0].Top - IndicatorGap;
		else if (cardIndex == cardRects.Count)
			lineY = cardRects[^1].Bottom + IndicatorGap;
		else
			lineY = (cardRects[cardIndex - 1].Bottom + cardRects[cardIndex].Top) / 2;

		// double lineY = cardIndex < cardRects.Count ? cardRects[cardIndex].Top : cardRects[^1].Bottom;
		return new DropTarget(hoveredGroup.Group, cardIndex, HorizontalLine(cardRects[0], lineY));
	}

	#region helpers
	private List<GroupArea> GetGroupAreas() {
		var groupAreas = new List<GroupArea>();
		if (ItemsSource is not IList groups)
			return groupAreas;

		foreach (Border border in this.GetVisualDescendants().OfType<Border>()) {
			if (border.DataContext is not ObservableCollection<TopicCardViewModel> group || !groups.Contains(group))
				continue;
			if (groupAreas.Any(area => ReferenceEquals(area.Group, group)))
				continue;
			if (border.TranslatePoint(default, this) is not Point topLeft)
				continue;

			groupAreas.Add(new GroupArea(group, new Rect(topLeft, border.Bounds.Size)));
		}

		return groupAreas;
	}

	private List<Rect> GetCardRects(ObservableCollection<TopicCardViewModel> group) {
		List<ExtendingCard> cardViews = this.GetVisualDescendants().OfType<ExtendingCard>().ToList();
		var cardRects = new List<Rect>();

		foreach (TopicCardViewModel card in group) {
			ExtendingCard? cardView = cardViews.FirstOrDefault(view => ReferenceEquals(view.DataContext, card));
			if (cardView?.TranslatePoint(default, this) is Point topLeft)
				cardRects.Add(new Rect(topLeft, cardView.Bounds.Size));
		}

		return cardRects;
	}

	private bool IsSamePlace(DropTarget target, List<GroupArea> groupAreas) {
		if (draggedCard is not TopicCardViewModel card)
			return true;

		int sourceGroupIndex = groupAreas.FindIndex(area => area.Group.Contains(card));
		if (sourceGroupIndex < 0)
			return true;

		ObservableCollection<TopicCardViewModel> sourceGroup = groupAreas[sourceGroupIndex].Group;

		if (target.Group is null)
			return sourceGroup.Count == 1 && IsGapNextToItem(target.Position, sourceGroupIndex);

		return ReferenceEquals(target.Group, sourceGroup) && IsGapNextToItem(target.Position, sourceGroup.IndexOf(card));
	}

	private static bool IsGapNextToItem(int gapIndex, int itemIndex) =>
		gapIndex == itemIndex || gapIndex == itemIndex + 1;

	private static Rect HorizontalLine(Rect area, double y) =>
		new(area.X, y - IndicatorThickness / 2, area.Width, IndicatorThickness);
	
	#endregion

	private void ApplyDrop(DropTarget target) {
		if (draggedCard is null)
			return;

		if (target.Group is null)
			MoveCardToNewGroup(draggedCard, target.Position);
		else
			MoveCard(draggedCard, target.Group, target.Position);
	}

	private void MoveCardToNewGroup(TopicCardViewModel card, int groupPosition) {
		if (ItemsSource is not IList groups)
			return;

		var sourceGroup = groups.OfType<ObservableCollection<TopicCardViewModel>>().FirstOrDefault(group => group.Contains(card));
		if (sourceGroup is null)
			return;

		sourceGroup.Remove(card);
		groups.Insert(Math.Clamp(groupPosition, 0, groups.Count), new ObservableCollection<TopicCardViewModel> { card });

		if (sourceGroup.Count == 0)
			groups.Remove(sourceGroup);
	}
	
}
