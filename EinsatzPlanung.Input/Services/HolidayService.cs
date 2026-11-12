namespace Einsatzplanung.Input.Services;

using System.Threading.Tasks;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Configuration;

public class HolidayService : EntitiyServiceBase<Holiday, HolidayConfig> {

	public HolidayService(IConfigService<HolidayConfig> configService) : base(configService) { }

	public override async Task<Holiday> MapConfigDataAsync(HolidayConfig config) {
		return new Holiday {
			Id = config.Id,
			From = config.From,
			To = config.To ?? config.From
		};
	}
}