namespace Einsatzplanung.Excel.Services;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Types.Models;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

public class TopicExportService : IConfigExportService<Topic> {
	public Task ExportAsync(string filePath, List<Topic> entities) {
		throw new NotImplementedException();
	}
}
