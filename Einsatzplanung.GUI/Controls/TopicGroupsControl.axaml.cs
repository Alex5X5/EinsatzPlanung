namespace Einsatzplanung.GUI.Controls;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;

using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.CodeGenerators.Attributes;
using Einsatzplanung.GUI.ViewModels.Edit;

public partial class TopicGroupsControl : ContentControl {
	
    [BasicStyledProperty]
    private IEnumerable<IEnumerable<object>> itemsSource;

    private IPointer? dragPointer;
	private Control? draggedCard;
	private Point dragStart;
	private bool isDragging;
    private EditCardsControl? CardsControl;

	public TopicGroupsControl() : base() {
        
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
		base.OnApplyTemplate(e);

		CardsControl = e.NameScope.Find<EditCardsControl>("CardsControl");
	}

	private void OnTopicDragPressed(object? sender, PointerPressedEventArgs e) {
		if (sender is not Control handle || handle.DataContext is not TopicCardViewModel card)
			return;

		dragPointer = e.Pointer;
		draggedCard = handle;
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
		int targetIndex = ItemsSource.Count() - 1;

        int targetGroupIndex = 0;

        bool isInsideGroup = false;

        foreach(ExtendingCard groupContainer in CardsControl?.GetVisualDescendants().OfType<ExtendingCard>() ?? []) {
            
            //get the position of the group of topic cards
            Point? containerPosition = CardsControl?.TranslatePoint(new Point(0, 0), groupContainer);
            if (containerPosition is null) {
                targetGroupIndex++;
                continue;
            }

            // check  wheter the pointer is over a group box
            if (pointerY < containerPosition.Value.Y + CardsControl?.Bounds.Height / 2) {
                isInsideGroup = true;
                break;
            }

            targetGroupIndex++;
        }

        int oldGroupIndex = 0;

        int oldIndex = 0;

        foreach(ExtendingCard groupContainer in CardsControl?.GetVisualDescendants().OfType<ExtendingCard>() ?? []) {
            
            oldIndex = 0;

            // check wheter the  control that is beeing dragged is a child group widget 
            foreach (var card in groupContainer.GetVisualDescendants().OfType<Control>()) {
                card.Equals(draggedCard);
                oldIndex ++;
            }

            oldGroupIndex++;
        }


        // if (ItemsSource is ObservableCollection<ObservableCollection<object>> list) {
        //     foreach (var innerList in list) {
        //         // if (container.DataContext is not TopicCardViewModel card)
        //         //     continue;
        //         //get the position of 
        //         Point? containerPosition = CardsControl?.TranslatePoint(new Point(0, 0), CardsControl);
        //         if (containerPosition is null)
        //             continue;

        //         // check  wheter the pointer is over a group box
        //         if (pointerY < containerPosition.Value.Y + CardsControl?.Bounds.Height / 2) {
        //             continue;
        //         }
        //         var index = innerList.IndexOf(draggedCard);
        //         if (index == -1) {
        //             continue;
        //         }   
        //         targetIndex = index;
        //         break;
        //     }
        // }

		// int currentIndex = viewModel.Cards.IndexOf(draggedCard);
		// if (currentIndex < 0 || currentIndex == targetIndex)
		// 	return;
        var model = viewModel.Cards[oldGroupIndex][oldIndex];
        viewModel.Cards[targetGroupIndex].Add(model);
        viewModel.Cards[oldGroupIndex].RemoveAt(oldIndex);
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
