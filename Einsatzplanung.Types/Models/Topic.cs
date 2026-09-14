namespace Einsatzplanung.Types.Models;

public class Topic {

    public string Name { get; set; }

	public Topic(string name) {
		Name = name;
	}

	public override string ToString() {
		return $"Topic[Name={Name}]";
	}
}