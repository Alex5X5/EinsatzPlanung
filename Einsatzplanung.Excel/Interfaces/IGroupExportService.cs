namespace Einsatzplanung.Excel.Interfaces;

using Einsatzplanung.Types.Models.Configuration;
using System.Collections.Generic;

public interface IGroupExportService {
	void ExportTeachers(string filePath, List<TeacherConfig> teachers);
}
