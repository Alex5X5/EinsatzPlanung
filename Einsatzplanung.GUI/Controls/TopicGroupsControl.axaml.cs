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
	private bool isDragging;
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

	}

	private void BeginDragViaual(PointerEventArgs e) {
		if (draggedControl is null) {
			return;
		}

		overlay = OverlayLayer.GetOverlayLayer(draggedControl);
		if (overlay is null) {
			return;
		}

		grabOffset = e.GetPosition(draggedControl);
		dragVisual = new Border {
			Width = draggedControl.Bounds.Width,
			Height = draggedControl.Bounds.Height,
			Background = new VisualBrush {
				Visual = draggedControl,
				Stretch = Stretch.None,
				AlignmentX = AlignmentX.Left,
				AlignmentY = AlignmentY.Top
			},
			Opacity = 1,
			IsHitTestVisible = false
		};
		overlay.Children.Add(dragVisual);
		
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
		if (isDragging && draggedCard is not null &&
			DataContext is TopicViewModel viewModel &&
			CardsControl is EditCardsControl cardsControl) {
			Point pointerPosition = e.GetPosition(cardsControl);
			ObservableCollection<TopicCardViewModel>? targetGroup = GetTargetGroup(pointerPosition.Y, viewModel, cardsControl);
			if (targetGroup is not null) {
				int targetCardIndex = GetTargetCardIndex(targetGroup, pointerPosition.Y, cardsControl);
				if (targetCardIndex >= 0) {
					// viewModel.MoveCardToGroup(new CardMovedEventArgs(draggedCard, targetGroup, targetCardIndex));
					MoveCard(draggedCard, targetGroup, targetCardIndex);
				}
			}
		}

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


	private ObservableCollection<TopicCardViewModel>? GetTargetGroup(
		double pointerY,
		TopicViewModel viewModel,
		EditCardsControl cardsControl) {
		ObservableCollection<TopicCardViewModel>? lastGroup = null;
		foreach (Border border in cardsControl.GetVisualDescendants()
			.OfType<Border>()
			.Where(border => border.DataContext is ObservableCollection<TopicCardViewModel>)) {
			var group = (ObservableCollection<TopicCardViewModel>)border.DataContext!;
			if (!viewModel.Groups.Contains(group))
				continue;

			Point? position = border.TranslatePoint(new Point(0, 0), cardsControl);
			if (position is null)
				continue;

			lastGroup = group;
			if (pointerY < position.Value.Y + border.Bounds.Height / 2)
				return group;
		}

		return lastGroup;
	}

	private int GetTargetCardIndex(
		ObservableCollection<TopicCardViewModel> group,
		double pointerY,
		EditCardsControl cardsControl) {
		foreach (ExtendingCard cardContainer in cardsControl.GetVisualDescendants()
			.OfType<ExtendingCard>()
			.Where(card => card.DataContext is TopicCardViewModel)) {
			var card = (TopicCardViewModel)cardContainer.DataContext!;
			int index = group.IndexOf(card);
			if (index < 0)
				continue;

			Point? position = cardContainer.TranslatePoint(new Point(0, 0), cardsControl);
			if (position is not null && pointerY < position.Value.Y + cardContainer.Bounds.Height / 2)
				return index;
		}

		return group.Count;
	}
	
}
