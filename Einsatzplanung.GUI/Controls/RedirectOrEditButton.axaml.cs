namespace Einsatzplanung.GUI.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Einsatzplanung.GUI.CodeGenerators.Attributes;

using System;
using System.Linq;

[PseudoClasses(":editing")]
[PseudoClasses(":active")]
public partial class RedirectOrEditButton : TemplatedControl {

	private const int DOUBLE_CLICK_MAX_DELAY = 300;

	protected override Type StyleKeyOverride => typeof(RedirectOrEditButton);


	[BasicStyledProperty]
	private IRelayCommand? removeCommand = null;

	[BasicStyledProperty]
	private IRelayCommand? redirectCommand = null;

	[BasicStyledProperty]
	private object? removeCommandParameter = null;

	[BasicStyledProperty]
	private object? redirectCommandParameter = null;

	[BasicStyledProperty]
	private string text = "";

	[BasicStyledProperty]
	private bool isActive = true;

	private bool isEditing;

	private readonly DispatcherTimer clickTimer = new();
	private Button? removeButton;
	private TextBox? textBox;

	public RedirectOrEditButton() : base() {
		clickTimer.Tick += OnClickTimerTick;
		AddHandler(PointerPressedEvent, OnAnyPointerPressed, RoutingStrategies.Tunnel, handledEventsToo: true);
	}

	static RedirectOrEditButton() {
		IsActiveProperty.Changed.AddClassHandler<RedirectOrEditButton>((x, _) => x.UpdateActivePseudoClass());
		DataContextProperty.Changed.AddClassHandler<RedirectOrEditButton>((x, _) => x.UpdateActivePseudoClass());
	}

	private void UpdateActivePseudoClass() {
		PseudoClasses.Set(":active", IsActive);
		Console.WriteLine($"is active for card '{Text}' changed to {IsActive}");
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
		clickTimer.Interval = TimeSpan.FromMilliseconds(DOUBLE_CLICK_MAX_DELAY);
		clickTimer.Start();
	}

	private void OnClickTimerTick(object? sender, EventArgs e) {
		clickTimer.Stop();
		if (RedirectCommand?.CanExecute(RedirectCommandParameter) ?? false)
			RedirectCommand.Execute(RedirectCommandParameter);
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