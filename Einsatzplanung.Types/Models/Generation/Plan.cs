namespace Einsatzplanung.Types.Models.Generation;

using System;
using System.Collections.Generic;

using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Generation;

public sealed class Plan {

	public Dictionary<Group, Dictionary<DateTime, Assignment>> Assignments = [];

}
