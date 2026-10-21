namespace Einsatzplanung.Types.Models.Configuration;

using System;

public class TopicConfig : Config {
	
	public required string Name { set; get; }
	public required string Color { set; get; }

	public static TopicConfig Default => new() {
		Id = new Guid("00000000-0000-0000-000000000acc"),
		Name = "Neues Thema",
		Color = "#FFFFFF"
	};
}