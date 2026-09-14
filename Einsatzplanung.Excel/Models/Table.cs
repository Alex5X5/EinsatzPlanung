namespace Einsatzplanung.Excel.Models;

using System.Collections.Generic;
using System.Linq;

public class Table {

	public int RowCount {
		get => Cells.Count;
	}
	public int ColumnCount {
		get => Cells.Select(x => x.Count).Max();
	}

	public int Index { get; set; }

	public List<List<TableCell>> Cells { set; get; } = [];

	public TableCell this[int row, int col] =>
		Cells[row][col];
}
