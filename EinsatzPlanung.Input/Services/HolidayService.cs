namespace Einsatzplanung.Input.Services;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Configuration;

public class HolidayService : EntitiyServiceBase<Holiday, HolidayConfig> {

	public HolidayService(IConfigService<HolidayConfig> configService) : base(configService) { }

	protected override Holiday MapConfigData(HolidayConfig config) {
		return new Holiday {
			From = config.From,
			To = config.To ?? config.From
		};
	}
}