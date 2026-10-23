namespace Einsatzplanung.Types.Models.Configuration;

using System;

public class TopicConfig : Config {
	
	public required string Name { set; get; }
	public required string Color { set; get; }

	public override string ToString() {
		return $"Topic[Name={Name}, Color={Color}";
	}

	public static TopicConfig Default => new() {
		Id = new Guid("00000000-0000-0000-0000-000000000acc"),
		Name = "",
		Color = "#FFFFFF"
	};
}