using Avalonia.Controls;
using Avalonia.Input;

using System;

namespace Einsatzplanung.GUI.Controls;

public class NoScrollComboBox : ComboBox {
	protected override Type StyleKeyOverride => typeof(ComboBox);

	protected override void OnPointerWheelChanged(PointerWheelEventArgs e) {
		if (IsDropDownOpen) {
            base.OnPointerWheelChanged(e);
        }
	}
}