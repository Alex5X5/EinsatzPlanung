namespace Einsatzplanung.Types.Models.Configuration;

using System;
using System.Linq;
using System.Collections.Generic;

using Einsatzplanung.Types.Models;

public class TeacherConfig {
	
	public required string Name { set; get; }
	public required string Abbreviation { set; get; }
	public required int WeeklyHours { set; get; } = 40;
	public List<Topic> Specializations { set; get; } = [];

	public bool CanTeachBlock(Block block) {
		return Specializations.Any(topic =>
			string.Equals(topic.Name, block.Name, StringComparison.OrdinalIgnoreCase) ||
			block.Name.Contains(topic.Name, StringComparison.OrdinalIgnoreCase));
	}

	public override string ToString() {
		return $"Teacher[Name={Name}, Abbreviation={Abbreviation}, WeeklyHours={WeeklyHours}, " +
			$"Specializations=[{string.Join(", ", Specializations.Select(t => t.Name))}]]";
	}

	public static TeacherConfig Default => new() {
		Name = "",
		Abbreviation = "",
		WeeklyHours = 0,
		Specializations = []
	};
}