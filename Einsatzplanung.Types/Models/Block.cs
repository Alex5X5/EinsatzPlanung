namespace Einsatzplanung.Types.Models;

using System;

public class Block {
	
	public int Anzahl { get; init; }
	public required string Name { get; init; }
	public required string Farbe { get; init; }

}