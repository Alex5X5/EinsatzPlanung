using System;
using System.Xml.Linq;

namespace Einsatzplanung.Types.Models;

public class Holiday {
    public DateOnly From { get; set; }
    public DateOnly? To { get; set; }
    public bool IsSingleDay => To == null;
    public bool IsPeriod => To != null;

	public override string ToString() {
		return $"Holiday[From={From}, To={To}]";
	}
}
