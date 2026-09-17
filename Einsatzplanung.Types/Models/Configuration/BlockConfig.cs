namespace Einsatzplanung.Types.Models.Configuration;

using System;

public class BlockConfig {
	
	public int Anzahl { get; init; }
	public required string Name { get; init; }
	public required string Color { get; init; }

	public override string ToString() {
		return $"Block[Name={Name}, Count={Convert.ToString(Anzahl)}, Color={Color}]";
	}
}