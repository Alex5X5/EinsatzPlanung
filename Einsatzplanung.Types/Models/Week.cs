namespace Einsatzplanung.Types.Models;

using System;

public class Week
{
    public long Id { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public Class Class { get; set; }
    public Teacher Teacher { get; set; }
}