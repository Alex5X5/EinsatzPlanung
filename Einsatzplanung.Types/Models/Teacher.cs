namespace Einsatzplanung.Types.Models;

using System.Collections.Generic;

public class Teacher
{
    public long Id { get; set; }
    public string Name { get; set; }
    public List<Topic> Specializations { get; set; }
}