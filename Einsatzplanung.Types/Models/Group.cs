namespace Einsatzplanung.Types.Models;

using System;
using System.Collections.Generic;
using System.Linq;

public class Group {

    public required string Name { set; get; }

	public required Teacher Teacher { set; get; }

    public required List<int> SchoolWeeks { set; get; }

	public required List<Block> Blocks { set; get; }

	public required List<Holiday> Holidays { set; get; }

	public override string ToString() {
		return $"Group[Name={Name}, " +
			$"SchoolWeeks=[{string.Join(", ", SchoolWeeks.Select(t => Convert.ToString(t)))}], " +
			$"Blocks=[{string.Join(", ", Blocks.Select(b => b.ToString()))}]" +
			$"Holidays=[{string.Join(", ", Holidays.Select(h => h.ToString()))}]]";
	}
}