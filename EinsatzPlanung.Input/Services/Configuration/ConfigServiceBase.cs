namespace Einsatzplanung.Input.Services.Configuration;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Excel.Models;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models.Configuration;

public abstract class ConfigServiceBase<Tc> : IConfigService<Tc> where Tc : Config {

	private IExcelImportService importService;

	private List<Tc>? bufferedData;

	private string SourceFilePath { get; set; } = "";

	public ConfigServiceBase(IExcelImportService importService) {
		this.importService = importService;
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
		Table table = importService.GetTable(SourceFilePath);
		bufferedData = ParseTable(table);
		return bufferedData;
	}

	public Tc? GetById(Guid id) {
		return GetData().FirstOrDefault(x => x.Id == id);
	}

	public bool TryGetById(Guid id, out Tc? result) {
		result = GetById(id);
		return result != null;
	}

	public async Task UpdateReferencesAsync() {
		bufferedData = await UpdateReferencesAsync(GetData());
	}

	protected abstract List<Tc> ParseTable(Table table);

	protected virtual Task<List<Tc>> UpdateReferencesAsync(List<Tc> data) {
		return Task.FromResult(data);
	}
}
