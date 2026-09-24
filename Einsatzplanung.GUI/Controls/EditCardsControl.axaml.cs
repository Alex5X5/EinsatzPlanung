namespace Einsatzplanung.GUI.Controls;

using System;

using Avalonia.Controls;
using Avalonia.Controls.Templates;

public partial class EditCardsControl : ItemsControl {

	protected override Type StyleKeyOverride => typeof(ItemsControl);

	static EditCardsControl() {
		ItemsPanelProperty.OverrideDefaultValue<EditCardsControl>(
		new FuncTemplate<Panel>(() => new StackPanel())!);
	}

	public EditCardsControl() : base() {
		InitializeComponent();
	}
}
