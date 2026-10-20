namespace Einsatzplanung.Types.Models.Configuration;

public class TopicConfig : Config {
	
	public required string Name { set; get; }
	public required string Color { set; get; }

	public static TopicConfig Default => new() {
		Name = "Neues Thema",
		Color = "#FFFFFF"
	};
}