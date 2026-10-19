namespace Einsatzplanung.Excel.Services;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

public class TopicExportService : IConfigExportService<TopicConfig> {
	public Task ExportAsync(string filePath, List<TopicConfig> entities) {
		throw new NotImplementedException();
	}
}
