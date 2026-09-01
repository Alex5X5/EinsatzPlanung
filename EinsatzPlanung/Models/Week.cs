using System;

namespace EinsatzPlanung.Models;

public class Week
{
    public long Id { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public Class Class { get; set; }
    public Teacher Teacher { get; set; }
}