namespace Einsatzplanung.GUI.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;

using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.CodeGenerators.Attributes;

using System;
using System.Linq;

[PseudoClasses(":editing")]
public partial class RedirectOrEditButton : TemplatedControl {

	protected override Type StyleKeyOverride => typeof(RedirectOrEditButton);


	[BasicStyledProperty<RedirectOrEditButton>]
	private IRelayCommand? removeCommand;

	[BasicStyledProperty<RedirectOrEditButton>]
	private IRelayCommand? redirectCommand;

	[BasicStyledProperty<RedirectOrEditButton>]
	private object? removeCommandParameter;

	[BasicStyledProperty<RedirectOrEditButton>]
	private object? redirectCommandParameter;

	[BasicStyledProperty<RedirectOrEditButton>]
	private string text = "";

	private readonly DispatcherTimer clickTimer = new();
	private Button? removeButton;
	private TextBox? textBox;
	private bool isEditing;


	public RedirectOrEditButton() : base() {
		clickTimer.Tick += OnClickTimerTick;
		AddHandler(PointerPressedEvent, OnAnyPointerPressed, RoutingStrategies.Tunnel, handledEventsToo: true);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
		base.OnApplyTemplate(e);

		if (textBox is not null) {
			textBox.LostFocus -= OnTextBoxLostFocus;
			textBox.KeyDown -= OnTextBoxKeyDown;
		}

		removeButton = e.NameScope.Find<Button>("PART_RemoveButton");
		textBox = e.NameScope.Find<TextBox>("PART_TextBox");

		if (textBox is not null) {
			textBox.LostFocus += OnTextBoxLostFocus;
			textBox.KeyDown += OnTextBoxKeyDown;
		}
	}

	private void OnAnyPointerPressed(object? sender, PointerPressedEventArgs e) {
		if (isEditing)
			return;

		// clicks on the remove button must not trigger click / edit
		if (e.Source is Visual source)
			if (removeButton is not null) {
				if (source == removeButton)
					return;
				if (source.GetVisualAncestors().Contains(removeButton))
					return;
			}

		if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
			return;
		
		// second click: stop the click timer and mark the event as handled
		if (e.ClickCount >= 2) {
			clickTimer.Stop();
			SetEditing(true);
			e.Handled = true;
			return;
		}

		// first click: wait to see whether a second one follows
		clickTimer.Stop();
		var doubleTapTime = TimeSpan.FromMilliseconds(300);
		clickTimer.Interval = doubleTapTime;
		clickTimer.Start();
	}

	private void OnClickTimerTick(object? sender, EventArgs e) {
		clickTimer.Stop();
		if (redirectCommand?.CanExecute(redirectCommandParameter) ?? false)
			redirectCommand.Execute(redirectCommandParameter);
	}

	private void SetEditing(bool value) {
		isEditing = value;
		PseudoClasses.Set(":editing", value);

		if (value && textBox is not null) {
			Dispatcher.UIThread.Post(() => {
				textBox.Focus();
				textBox.SelectAll();
			}, DispatcherPriority.Input);
		}
	}

	private void OnTextBoxLostFocus(object? sender, RoutedEventArgs e) => SetEditing(false);

	private void OnTextBoxKeyDown(object? sender, KeyEventArgs e) {
		if (e.Key is Key.Enter or Key.Escape) {
			SetEditing(false);
			Focus();
			e.Handled = true;
		}
	}
}