using System;
using System.Collections.Generic;
using System.Linq;

using Einsatzplanung.Excel.Interfaces;
using Einsatzplanung.Excel.Models;
using Einsatzplanung.Types.Models.Configuration;

namespace Einsatzplanung.Input.Services.Configuration {

	public class TopicConfigService : ConfigServiceBase<TopicConfig> {

		private const int NAME_COLUMN_INDEX = 0;
		private const int COLOR_COLUMN_INDEX = 1;

		public TopicConfigService(IExcelImportService importService) : base(importService) { }

		protected override List<TopicConfig> ParseTable(Table table) {
			List<TopicConfig> topics = [];

			for (int row = 1; row < table.RowCount; row++) {
				var name = table[row, NAME_COLUMN_INDEX]?.Value;
				if (string.IsNullOrEmpty(name))
					continue;

				var color = table[row, COLOR_COLUMN_INDEX]?.Value;
				if (string.IsNullOrEmpty(color))
					continue;

				TopicConfig config = new() {
					Id = Guid.NewGuid(),
					Name = name,
					Color = color
				};

				topics.Add(config);
			}
			return topics;
		}
	}
}