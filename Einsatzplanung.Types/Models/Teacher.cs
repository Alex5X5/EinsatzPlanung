namespace Einsatzplanung.Types.Models;

using System.Collections.Generic;

public class Teacher {
	
	public required string Name { set; get; }
	public required string Abbreviation { set; get; }
	public required int WeeklyHours { set; get; } = 40;
	public List<Topic> Specializations { set; get; } = [];

	public bool CanTeachBlock(Block block) {
		return Specializations.Any(topic =>
			string.Equals(topic.Name, block.Name, StringComparison.OrdinalIgnoreCase) ||
			block.Name.Contains(topic.Name, StringComparison.OrdinalIgnoreCase));
	}
}