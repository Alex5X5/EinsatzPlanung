namespace Einsatzplanung.Excel.Models;

using System.Collections.Generic;

public class Table {

	//public TableCell[,] Cells { get; set; } = new TableCell[1, 1];

	public int Index { get; set; }

	public List<List<TableCell>> Cells { set; get; } = [];

	public TableCell this[int row, int col] =>
		Cells[row][col];
}
