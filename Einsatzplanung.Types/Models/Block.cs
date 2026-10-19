namespace Einsatzplanung.Types.Models;

using System;

public class Block {
	
	public int Count { get; init; }
	public required Topic Topic { get; init; }

	public override string ToString() {
		return $"BlockConfig[Name={Topic.Name}, Count={Convert.ToString(Count)}, Color={Topic.Color}]";
	}
}