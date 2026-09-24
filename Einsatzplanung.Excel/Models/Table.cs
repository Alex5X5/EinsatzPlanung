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

	public List<List<TableCell>> Cells { set; get; } = [];

	public TableCell? this[int row, int col] {
		get {
			if(row > Cells.Count - 1)
				return null;
			if (col > Cells[row].Count - 1)
				return null;
			return Cells[row][col];
		} 
	}

	public void AddRow(List<TableCell> cells) {
		Cells.Add(cells);
	}

	public void AddRow(List<string> cells) {
		Cells.Add(cells.Select(value => new TableCell { Value = value }).ToList());
	}
}
