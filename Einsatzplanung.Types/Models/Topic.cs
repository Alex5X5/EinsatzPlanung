using System;

namespace Einsatzplanung.Types.Models;

public class Topic : Entity {

    public required string Name { get; set; }

    public required string Color { get; set; }

    public required int Level { get; set; }

	public static Topic Default => new() {
		Id = new Guid("00000000-0000-0000-0000-000000000acc"),
		Name = "",
		Color = "#FFFFFF",
		Level = 1
	};

	public override string ToString() {
		return $"Topic[Name={Name}, Color={Color}]";
	}
}