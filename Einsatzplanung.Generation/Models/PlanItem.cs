namespace Einsatzplanung.Types.Models;

internal sealed class PlanItem {

	internal required Group Group { set; get; }
	internal required Block Block { set; get; }
	internal required int QualifiedCount { set; get; }

}
