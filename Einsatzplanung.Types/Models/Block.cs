namespace Einsatzplanung.Types.Models;

using System;

public class Block {
	
	public required string Name { get; init; }
    public required DateTime From { get; init; }
    public required DateTime To { get; init; }
    public required string Color { get; init; }

	public override string ToString() {
		return $"Block[Name={Name}, From={From}, To={To} Color={Color}]";
	}
}