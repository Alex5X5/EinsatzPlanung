namespace Einsatzplanung.Input.Services;

using Einsatzplanung.Input.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Types.Models.Configuration;

using System.Collections.Generic;
using System.Linq;

public class HolidayService : IEntityService<Holiday> {
    
    private IConfigService<HolidayConfig> configService;

    public HolidayService(IConfigService<HolidayConfig> configService) {
        this.configService = configService;
    }

    public List<Holiday> GetEntities() {
        List<HolidayConfig> configs = configService.ParseSource();
        return configs.Select(c => new Holiday { From = c.From, To = c.To ?? c.From }).ToList();
	}

	public void SetSource(string path) {
		configService.SetSource(path);
	}
}
