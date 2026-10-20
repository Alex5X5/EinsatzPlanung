using Einsatzplanung.Types.Models.Configuration;

namespace Einsatzplanung.Types.Models;

public class Topic : Entity {

    public string Name { get; set; }

    public string Color { get; set; }

	public Topic(string name, string color) {
		Name = name;
		Color = color;
	}

	public static Topic Default => new("Neues Thema", "#FFFFFF") {
		Id = 1
	};

	public override string ToString() {
		return $"Topic[Name={Name}, Color={Color}]";
	}
}