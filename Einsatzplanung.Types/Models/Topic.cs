namespace Einsatzplanung.Types.Models;

public class Topic {

    public string Name { get; set; }

    public string Color { get; set; }

	public Topic(string name, string color) {
		Name = name;
		Color = color;
	}

	public override string ToString() {
		return $"Topic[Name={Name}, Color={Color}]";
	}
}