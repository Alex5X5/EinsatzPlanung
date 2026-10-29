namespace Einsatzplanung.Input.Services.Configuration;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Excel.Models;
using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types;
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

	public async Task SetDataAsync(List<Tc> data, CancellationToken? token = null) {
		await Task.Run(() => bufferedData = data);
	}

	public async Task<List<Tc>> GetDataAsync(CancellationToken? token = null) {
		return await Task.Run(
			async () => {
				if (bufferedData != null)
					return bufferedData;
				Table table = importService.GetTable(SourceFilePath);
				bufferedData = await ParseTableAsync(table, token);
				return bufferedData;
			});
	}

	public async Task<Tc?> GetByIdAsync(Guid id, CancellationToken? token = null) {
		return (await GetDataAsync()).FirstOrDefault(x => x.Id == id);
	}

	public async Task<TryResult<Tc>> TryGetByIdAsync(Guid id, CancellationToken? token = null) {
		var result = await GetByIdAsync(id, token);
		return new(result != null, result);
	}

	public async Task UpdateReferencesAsync(CancellationToken? token = null) {
		var data = await GetDataAsync(token);
		if (token?.IsCancellationRequested ?? false)
			return;
		bufferedData = await UpdateReferencesAsync(data, token);
	}

	protected abstract Task<List<Tc>> ParseTableAsync(Table table, CancellationToken? token = null);

	protected virtual Task<List<Tc>> UpdateReferencesAsync(List<Tc> data, CancellationToken? token = null) {
		return Task.FromResult(data);
	}
}
