using System.Collections.Generic;

namespace EinsatzPlanung.Models;

public class Teacher
{
    public long Id { get; set; }
    public string Name { get; set; }
    public List<Topic> Specializations { get; set; }
}