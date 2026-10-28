namespace Einsatzplanung.Input.Interfaces;

using Einsatzplanung.Types;
using Einsatzplanung.Types.Models.Configuration;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface IConfigService<T> : IConfigService where T : Config {

	public Task<List<T>> GetDataAsync(CancellationToken? token = null);

	public Task<T?> GetByIdAsync(Guid id, CancellationToken? token = null);

	public Task<TryResult<T>> TryGetByIdAsync(Guid id, CancellationToken? token = null);

	public Task SetDataAsync(List<T> data, CancellationToken? token = null);
}

public interface IConfigService {

	public void SetSource(string path);

	public Task UpdateReferencesAsync(CancellationToken? token = null);

}
