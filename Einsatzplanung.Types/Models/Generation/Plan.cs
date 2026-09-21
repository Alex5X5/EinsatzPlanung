namespace Einsatzplanung.Types.Models.Generation;

using System;
using System.Collections.Generic;

public sealed class Plan {

	public Dictionary<string, Dictionary<DateTime, Assignment>> Assignments = [];

}
