namespace Einsatzplanung.Types.Models.Generation;

public sealed class Assignment {
	public required string Trainer { get; init; }
	public required string BlockName { get; init; }
	public required string BlockColor { get; init; }
}
