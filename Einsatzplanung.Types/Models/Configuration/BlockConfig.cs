namespace Einsatzplanung.Types.Models.Configuration;

using System;

public class BlockConfig : Config {
	
	public int Count { get; init; }
	public required TopicConfig Topic { get; init; }

	public static BlockConfig Default => new() {
		Id = new Guid("00000000-0000-0000-0000-000000000acc"),
		Topic = TopicConfig.Default,
		Count = 1
	};

	public override string ToString() {
		return $"BlockConfig[Topic={Topic.Name}, Count={Convert.ToString(Count)}]";
	}
}