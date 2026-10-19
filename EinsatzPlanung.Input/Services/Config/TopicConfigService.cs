namespace Einsatzplanung.Input.Services;

using System.Collections.Generic;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Excel.Models;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public class TopicConfigService : IConfigService<TopicConfig> {

	private const int NAME_COLUMN_INDEX = 0;
	private const int COLOR_COLUMN_INDEX = 1;

	private IExcelImportService excelImportService;
	private string SourceFilePath { get; set; } = "";

	private List<TopicConfig>? bufferedData;

	public TopicConfigService(IExcelImportService excelImportService) {
		this.excelImportService = excelImportService;
	}

	public void SetSource(string path) {
		SourceFilePath = path;
	}

	public List<TopicConfig> GetData() {
		if (bufferedData != null)
			return bufferedData;

		Table table = excelImportService.GetTable(SourceFilePath);
		List<TopicConfig> topics = [];

		for (int row = 1; row < table.RowCount; row++) {
			var name = table[row, NAME_COLUMN_INDEX]?.Value;
			if (string.IsNullOrEmpty(name))
				continue;

			var color = table[row, COLOR_COLUMN_INDEX]?.Value;
			if (string.IsNullOrEmpty(color))
				continue;

			TopicConfig config = new() {
				Name = name,
				Color = color
			};

			topics.Add(config);
		}

		bufferedData = topics;

		return topics;
	}

	public void SetData(List<TopicConfig> data) {
		bufferedData = data;
	}
}
