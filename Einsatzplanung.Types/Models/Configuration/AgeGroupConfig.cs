namespace Einsatzplanung.Types.Models.Configuration;

using System.Collections.Generic;
using System.Linq;

public class AgeGroupConfig {

	public string Name { set; get; } = "";

	public List<GroupConfig> Groups { set; get; } = [];

	public override string ToString() {
		return $"AgeGroup[Name={Name}, " +
			$"Groups=[{string.Join(", ", Groups.Select(g => g.ToString()))}]]";
	}
}