namespace Einsatzplanung.Types.Models.Configuration;

using System;

public class BlockConfig {
	
	public int Count { get; init; }
	public required string Topic { get; init; }

	public override string ToString() {
		return $"BlockConfig[Name={Topic}, Count={Convert.ToString(Count)}]";
	}
}