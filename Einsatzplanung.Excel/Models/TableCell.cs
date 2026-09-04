namespace Einsatzplanung.Excel.Models;

using Avalonia.Media;

public class TableCell {
	
	public Color BackgroundColor { get; set; } = Colors.White;

	public string Value { get; set; } = "";

	public bool Bold { get; set; }
	public int TextRotation { get; set; }
	public string BackgroundColor1 { get; set; } = "";

}
