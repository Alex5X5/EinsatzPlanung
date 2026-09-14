namespace EinsatzPlanung.Input.Services;

using System.Collections.Generic;

using EinsatzPlanung.Generation.Interfaces;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Excel.Models;

public class TopicService : IEnttityService<Topic> {

	public TopicService() {
		
	}

	public List<Topic> ParseExcelTable(Table table) {
		List<Topic> topics = new List<Topic>();
		for(int row=0; row < table.RowCount; row++) {
			Topic topic = new() {
				Name = table.Cells[row][0].Value
			};
			topics.Add(topic);
		}
		return topics;
	}
}
