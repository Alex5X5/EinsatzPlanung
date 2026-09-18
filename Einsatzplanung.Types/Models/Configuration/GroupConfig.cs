namespace Einsatzplanung.Types.Models.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;

public class GroupConfig {

    public required string Name { set; get; }

    public required string TeacherAbbreviation { set; get; }

    public required List<int> SchoolWeeks { set; get; }

	public required List<BlockConfig> Blocks { set; get; }

	public override string ToString() {
		return $"Group[Name={Name}, " +
			$"SchoolWeeks=[{string.Join(", ", SchoolWeeks.Select(t => Convert.ToString(t)))}], " +
			$"Blocks=[{string.Join(", ", Blocks.Select(b => b.ToString()))}]]";
	}
}