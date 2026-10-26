namespace Einsatzplanung.Types.Models.Generation;

using System;
using System.Collections.Generic;
using System.Text;

public class GeneratorConfig {

	public Dictionary<Guid, bool> GroupsToExport { set; get; }
}
