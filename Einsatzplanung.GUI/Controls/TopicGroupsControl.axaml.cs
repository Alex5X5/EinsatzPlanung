namespace Einsatzplanung.GUI.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;

using Einsatzplanung.GUI.CodeGenerators.Attributes;
using Einsatzplanung.GUI.ViewModels.Edit;
using Einsatzplanung.GUI.Views.Edit;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public partial class TopicGroupsControl : ContentControl {
	
    [BasicStyledProperty]
    private IEnumerable<IEnumerable<object>>? itemsSource = null;

    private IPointer? dragPointer;
	private TopicCardViewModel? draggedCard;
	private Point dragStart;
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

		dragPointer = e.Pointer;
		draggedCard = card;
		dragStart = e.GetPosition(this);
		isDragging = false;
		e.Pointer.Capture(container);
		e.Handled = true;
	}

	private void OnTopicDragMoved(object? sender, PointerEventArgs e) {
		if (dragPointer != e.Pointer || draggedCard is null || CardsControl is null)
			return;

		Point pointerPosition = e.GetPosition(this);
		Vector dragDelta = pointerPosition - dragStart;
		if (!isDragging && Math.Sqrt(dragDelta.X * dragDelta.X + dragDelta.Y * dragDelta.Y) < 5)
			return;

		isDragging = true;
	}

	private void OnTopicDragReleased(object? sender, PointerReleasedEventArgs e) {
		if (dragPointer != e.Pointer)
			return;

		e.Pointer.Capture(null);
		if (isDragging && draggedCard is not null &&
			DataContext is TopicViewModel viewModel &&
			CardsControl is EditCardsControl cardsControl) {
			Point pointerPosition = e.GetPosition(cardsControl);
			ObservableCollection<TopicCardViewModel>? targetGroup = GetTargetGroup(pointerPosition.Y, viewModel, cardsControl);
			if (targetGroup is not null) {
				int targetCardIndex = GetTargetCardIndex(targetGroup, pointerPosition.Y, cardsControl);
				if (targetCardIndex >= 0) {
					viewModel.MoveCardToGroup(new CardMovedEventArgs(draggedCard, targetGroup, targetCardIndex));
				}
			}
		}

		dragPointer = null;
		draggedCard = null;
		isDragging = false;
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
