namespace Einsatzplanung.Types.Models;

using Einsatzplanung.Types.Models.Configuration;

using System;

public class Week
{
    public long Id { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public Group Class { get; set; }
    public TeacherConfig Teacher { get; set; }
}