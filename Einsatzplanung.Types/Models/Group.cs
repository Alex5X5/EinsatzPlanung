namespace Einsatzplanung.Types.Models;

public class Group {

    public required string Name { set; get; }

    public required List<int> SchoolWeeks { set; get; }

	public required List<Block> Blocks { set; get; }
}