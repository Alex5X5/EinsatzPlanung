namespace Einsatzplanung.GUI.Views.Edit;

using System;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

using Einsatzplanung.GUI.Controls;
using Einsatzplanung.GUI.ViewModels.Edit;

public partial class TopicView : ViewBase<TopicViewModel> {

	private IPointer? dragPointer;
	private TopicCardViewModel? draggedCard;
	private Point dragStart;
	private bool isDragging;

	public TopicView() : base() {
		InitializeComponent();
	}

	private void OnTopicDragPressed(object? sender, PointerPressedEventArgs e) {
		if (sender is not Control handle || handle.DataContext is not TopicCardViewModel card)
			return;

		dragPointer = e.Pointer;
		draggedCard = card;
		dragStart = e.GetPosition(this);
		isDragging = false;
		e.Pointer.Capture(handle);
		e.Handled = true;
	}

	private void OnTopicDragMoved(object? sender, PointerEventArgs e) {
		if (dragPointer != e.Pointer || draggedCard is null)
			return;

		Point pointerPosition = e.GetPosition(this);
		Vector dragDelta = pointerPosition - dragStart;
		if (!isDragging && Math.Sqrt(dragDelta.X * dragDelta.X + dragDelta.Y * dragDelta.Y) < 5)
			return;

		isDragging = true;
		if (DataContext is not TopicViewModel viewModel)
			return;

		double pointerY = e.GetPosition(CardsControl).Y;
		int targetIndex = viewModel.Cards.Count - 1;
		foreach (ExtendingCard container in CardsControl.GetVisualDescendants().OfType<ExtendingCard>()) {
			if (container.DataContext is not TopicCardViewModel card)
				continue;
			Point? containerPosition = container.TranslatePoint(new Point(0, 0), CardsControl);
			if (containerPosition is null)
				continue;

			if (pointerY < containerPosition.Value.Y + container.Bounds.Height / 2) {
				targetIndex = viewModel.Cards.IndexOf(card);
				break;
			}
		}

		int currentIndex = viewModel.Cards.IndexOf(draggedCard);
		if (currentIndex < 0 || currentIndex == targetIndex)
			return;

		viewModel.Cards.Move(currentIndex, targetIndex);
		for (int index = 0; index < viewModel.Cards.Count; index++)
			viewModel.Cards[index].Index = index;
	}

	private void OnTopicDragReleased(object? sender, PointerReleasedEventArgs e) {
		if (dragPointer != e.Pointer)
			return;

		e.Pointer.Capture(null);
		dragPointer = null;
		draggedCard = null;
		isDragging = false;
	}
}
