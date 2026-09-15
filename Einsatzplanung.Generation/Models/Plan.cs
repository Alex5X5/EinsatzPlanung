namespace Einsatzplanung.Generation.Models;

using System;
using System.Collections.Generic;

using Einsatzplanung.Types.Models;

internal sealed class Plan {

	public Dictionary<Group, Dictionary<DateTime, Assignment>> Assignments = [];

}
