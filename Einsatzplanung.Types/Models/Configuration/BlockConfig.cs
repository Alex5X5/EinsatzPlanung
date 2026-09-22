namespace Einsatzplanung.Types.Models.Configuration;

using System;

public class BlockConfig {
	
	public int Count { get; init; }
	public required string Name { get; init; }
	public required string Color { get; init; }

	public override string ToString() {
		return $"Block[Name={Name}, Count={Convert.ToString(Count)}, Color={Color}]";
	}
}