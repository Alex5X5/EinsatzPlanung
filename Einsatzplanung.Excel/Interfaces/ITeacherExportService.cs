namespace Einsatzplanung.Excel.Interfaces;

using Einsatzplanung.Types.Models.Configuration;
using System.Collections.Generic;

public interface ITeacherExportService {
	void ExportTeachers(string filePath, List<TeacherConfig> teachers);
}
