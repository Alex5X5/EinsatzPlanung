namespace Einsatzplanung.Input.Services.Configuration;

using System.Collections.Generic;
using System.Linq;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Excel.Models;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public abstract class ConfigServiceBase<Tc> : IConfigService<Tc> where Tc : Config {

	private IExcelImportService excelImportService;

	private List<Tc>? bufferedData;

	private string SourceFilePath { get; set; } = "";

	public ConfigServiceBase(IExcelImportService excelImportService) {
		this.excelImportService = excelImportService;
	}

	public void SetSource(string path) {
		SourceFilePath = path;
	}

	public void SetData(List<Tc> data) {
		bufferedData = data;
	}

	public List<Tc> GetData() {
		if (bufferedData != null)
			return bufferedData;

		Table table = excelImportService.GetTable(SourceFilePath);
		return ParseTable(table);
	}

	public Tc? GetById(int id) {
		return GetData().FirstOrDefault(x => x.Id == id);
	}

	public bool TryGetById(int id, out Tc? result) {
		result = GetById(id);
		return result != null;
	}

	protected abstract List<Tc> ParseTable(Table table);
}
