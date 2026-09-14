namespace EinsatzPlanung.Input.Services;

using System.Collections.Generic;

using Einsatzplanung.Types.Models;
using Einsatzplanung.Excel.Models;
using Einsatzplanung.Excel.Services;

using EinsatzPlanung.Input.Interfaces;

public class TopicService : IEntityService<Topic> {

	private ExcelImportService excelImportService;

	private string SourceFilePath { get; set; } = "";

	public TopicService(ExcelImportService excelImportService) {
		this.excelImportService = excelImportService;
	}

	public void SetSource(string path) {
		SourceFilePath = path;
	}

	public List<Topic> ParseSource() {
		Table table = excelImportService.GetTable(SourceFilePath);
		List<Topic> topics = new List<Topic>();
		for(int row=0; row < table.RowCount; row++) {
			Topic topic = new(table.Cells[row][0].Value);
			topics.Add(topic);
		}
		return topics;
	}
}
