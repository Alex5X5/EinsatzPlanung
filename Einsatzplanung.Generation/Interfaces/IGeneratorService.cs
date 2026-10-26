namespace Einsatzplanung.Generation.Interfaces;

using Einsatzplanung.Types.Models.Generation;
using Einsatzplanung.Types.Models.Configuration;

public interface IGeneratorService {

	public Plan GeneratePlan(GeneratorConfig config);

}
