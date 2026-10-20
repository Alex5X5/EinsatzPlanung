namespace Einsatzplanung.Types.Models;

using System.Collections.Generic;
using System.Linq;

public class AgeGroup : Entity {

	public string Name { set; get; } = "";

	public List<Group> Groups { set; get; } = [];

	public override string ToString() {
		return $"AgeGroup[Name={Name}, " +
			$"Groups=[{string.Join(", ", Groups.Select(g => g.ToString()))}]]";
	}
}