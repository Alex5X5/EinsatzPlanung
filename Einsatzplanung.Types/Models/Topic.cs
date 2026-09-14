namespace Einsatzplanung.Types.Models;

public class Topic {

    public required string Name { get; set; }

	public override string ToString() {
		return $"Topic[Name={Name}]";
	}
}