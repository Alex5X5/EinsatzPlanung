namespace Einsatzplanung.Types.Models.Configuration;

using System;

public class BlockConfig : Config {
	
	public int Count { get; init; }
	public required TopicConfig Topic { get; init; }

	public override string ToString() {
		return $"BlockConfig[Name={Topic.Name}, Count={Convert.ToString(Count)}]";
	}
}