namespace Einsatzplanung.Generation.Interfaces;

using System.Threading.Tasks;

using Einsatzplanung.Types.Models.Generation;

public interface IGeneratorService {

	public Task<Plan> GeneratePlanAsync(GeneratorConfig config);

}
