namespace Einsatzplanung.Types.Models;

using System;

public class Block {
	
	public int Count { get; init; }
	public required string Name { get; init; }
	public required string Color { get; init; }

	public override string ToString() {
		return $"Block[Name={Name}, Count={Convert.ToString(Count)}, Color={Color}]";
	}
}