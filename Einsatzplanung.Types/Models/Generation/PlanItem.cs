using Einsatzplanung.Types.Models.Configuration;

namespace Einsatzplanung.Types.Models.Generation;

public sealed class PlanItem {

	public required Group Group { set; get; }
	public required Block Block { set; get; }
	public required int QualifiedCount { set; get; }

}
