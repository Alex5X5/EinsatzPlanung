using System;
using System.Collections;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

using Einsatzplanung.GUI.ViewModels;
using Einsatzplanung.GUI.ViewModels.Edit;

namespace Einsatzplanung.GUI.Views.Edit;

public partial class AgeGroupPageView : ViewBase<AgeGroupPageViewModel> {

    public AgeGroupPageView() : base() {
        InitializeComponent();
    }

    private CardViewModel? _pressedItem;
    private PointerPressedEventArgs? _pressedArgs;
    private Point _startPoint;
    private CardViewModel? _draggedItem;

    private void Item_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Source is Visual src &&
            (src.FindAncestorOfType<TextBox>(true) is not null ||
             src.FindAncestorOfType<ComboBox>(true) is not null ||
             src.FindAncestorOfType<Button>(true) is not null))
            return;

        if (sender is Control isabella && e.GetCurrentPoint(isabella).Properties.IsLeftButtonPressed)
        {
            _pressedItem = isabella.DataContext as CardViewModel;
            _pressedArgs = e;
            _startPoint = e.GetPosition(this);
        }
    }

    private void Item_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _pressedItem = null;
        _pressedArgs = null;
    }

    private async void Item_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_pressedItem is null || _pressedArgs is null) return;

        var delta = e.GetPosition(this) - _startPoint;
        if (Math.Abs(delta.X) < 6 && Math.Abs(delta.Y) < 6) return;

        var args = _pressedArgs;
        _draggedItem = _pressedItem;
        _pressedItem = null;
        _pressedArgs = null;

        var data = new DataTransfer();
        data.Add(DataTransferItem.CreateText(_draggedItem.Header ?? ""));

        await DragDrop.DoDragDropAsync(args, data, DragDropEffects.Move);
        _draggedItem = null;
    }

    private void Item_DragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = _draggedItem is null ? DragDropEffects.None : DragDropEffects.Move;
        e.Handled = true;
    }

    private void Item_Drop(object? sender, DragEventArgs e)
    {
        if (_draggedItem is null ||
            sender is not Control card ||
            card.DataContext is not CardViewModel target) return;
        
        var host = card.FindAncestorOfType<ItemsControl>();
        if (host?.ItemsSource is not IList items) return;

        int from = items.IndexOf(_draggedItem);
        int to = items.IndexOf(target);
        
        if (from >= 0 && to >= 0 && from != to)
        {
            items.RemoveAt(from);
            items.Insert(to, _draggedItem);

            for (int i = 0; i < items.Count; i++)
                if (items[i] is CardViewModel c) c.Index = i;
        }

        e.Handled = true;
    }
}