namespace Einsatzplanung.GUI.Views.Edit;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.VisualTree;

using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.CodeGenerators.Attributes;
using Einsatzplanung.GUI.Controls;
using Einsatzplanung.GUI.ViewModels.Edit;

public partial class TopicView : ViewBase<TopicViewModel> {

	private IPointer? dragPointer;
	private TopicCardViewModel? draggedCard;
	private Point dragStart;
	private bool isDragging;
	private TopicGroupsControl GroupCardsControl;

	[BasicStyledProperty]
	private IRelayCommand<CardMovedEventArgs>? cardMovedCommand;

	public TopicView() : base() {
		InitializeComponent();
		Bind(CardMovedCommandProperty, new Binding("MoveCardToGroupCommand"));
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
	}

	private void OnTopicDragReleased(object? sender, PointerReleasedEventArgs e) {
		if (dragPointer != e.Pointer)
			return;

		if (isDragging && draggedCard is not null && DataContext is TopicViewModel viewModel) {
			Point pointerPosition = e.GetPosition(GroupCardsControl);
			Border? targetBorder = GetGroupBorders()
				.FirstOrDefault(border => GetBounds(border).Contains(pointerPosition));

			if (targetBorder?.DataContext is ObservableCollection<TopicCardViewModel> targetGroup) {
				int targetCardIndex = GetTargetCardIndex(targetBorder, pointerPosition.Y, targetGroup);
				var commadArgs = new CardMovedEventArgs(draggedCard, targetGroup, targetCardIndex);
				if(CardMovedCommand.CanExecute(commadArgs))
					CardMovedCommand.Execute(commadArgs);
				// viewModel.MoveCardToGroup(draggedCard, targetGroup, targetCardIndex);
			}
		}

		e.Pointer.Capture(null);
		dragPointer = null;
		draggedCard = null;
		isDragging = false;
	}

	private IEnumerable<Border> GetGroupBorders() =>
		GroupCardsControl.GetVisualDescendants()
			.OfType<Border>()
			.Where(border => border.DataContext is ObservableCollection<TopicCardViewModel>);

	private Rect GetBounds(Control control) {
		Point? position = control.TranslatePoint(new Point(0, 0), GroupCardsControl);
		return position is null ? default : new Rect(position.Value, control.Bounds.Size);
	}

	private int GetTargetCardIndex(Border groupBorder, double pointerY, ObservableCollection<TopicCardViewModel> group) {
		foreach (ExtendingCard cardContainer in groupBorder.GetVisualDescendants().OfType<ExtendingCard>()) {
			if (cardContainer.DataContext is not TopicCardViewModel card)
				continue;

			Rect bounds = GetBounds(cardContainer);
			if (pointerY < bounds.Y + bounds.Height / 2)
				return group.IndexOf(card);
		}

		return group.Count;
	}

	private int GetTargetGroupIndex(double pointerY) {
		int index = 0;
		foreach (Border border in GetGroupBorders()) {
			Rect bounds = GetBounds(border);
			if (pointerY < bounds.Y + bounds.Height / 2)
				return index;
			index++;
		}

		return index;
	}
}

public sealed class CardMovedEventArgs(object model, ObservableCollection<TopicCardViewModel> targetGroup, int targetCardIndex) : EventArgs {

	public object Model {set; get;} = model;
	public ObservableCollection<TopicCardViewModel> TargetGroup {set; get;} = targetGroup;
	public int TargetCardIndex {set; get;} = targetCardIndex;
	
}