namespace Einsatzplanung.Types.Models.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;

public class GroupConfig : Config {

    public required string Name { set; get; }

	public required TeacherConfig Teacher { set; get; }

    public required List<int> SchoolWeeks { set; get; }

	public required List<BlockConfig> Blocks { set; get; }

	public override string ToString() {
		return $"Group[Name={Name}, " +
			$"Teacher={Teacher.ToString()}, " +
			$"AgeGroupName[AgeGroupName={Name}, " +
			$"SchoolWeeks=[{string.Join(", ", SchoolWeeks.Select(t => Convert.ToString(t)))}], " +
			$"Blocks=[{string.Join(", ", Blocks.Select(b => b.ToString()))}]]";
	}
}