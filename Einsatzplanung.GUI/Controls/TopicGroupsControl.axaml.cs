namespace Einsatzplanung.GUI.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.CodeGenerators.Attributes;
using Einsatzplanung.GUI.ViewModels.Edit;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

		if (CardsControl is null)
			return;
		CardsControl.PointerPressed += OnTopicDragPressed;
		CardsControl.PointerMoved += OnTopicDragMoved;
		CardsControl.PointerReleased += OnTopicDragReleased;
	}

	private void OnTopicDragPressed(object? sender, PointerPressedEventArgs e) {
		var container = (e.Source as Visual)?
			.GetSelfAndVisualAncestors()
			.OfType<Control>()
			.FirstOrDefault(c => c.GetVisualParent() == CardsControl?.ItemsPanelRoot);
		dragPointer = e.Pointer;
		draggedCard = container;
		dragStart = e.GetPosition(this);
		isDragging = false;
		e.Pointer.Capture(container);
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
            foreach (var card in groupContainer.GetVisualDescendants().OfType<ExtendingCard>()) {
				if (card.Equals(draggedCard))
					break;
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
		if (oldGroupIndex >= viewModel.Groups.Count)
			return;
		if (oldIndex >= viewModel.Groups[oldGroupIndex].Count)
			return;
        var model = viewModel.Groups[oldGroupIndex][oldIndex];
		viewModel.Groups[targetGroupIndex].Add(model);
		viewModel.Groups[oldGroupIndex].RemoveAt(oldIndex);
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
