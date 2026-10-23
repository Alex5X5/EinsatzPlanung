namespace Einsatzplanung.Input.Services.Configuration;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Einsatzplanung.Input.Interfaces;
using System.Linq;
using Einsatzplanung.Types.Models.Configuration;

public class GroupConfigService : IConfigService<GroupConfig> {

	private IConfigService<AgeGroupConfig> ageGroupService;

	public GroupConfigService(IConfigService<AgeGroupConfig> ageGroupService) {
		this.ageGroupService = ageGroupService;
	}

	public void SetSource(string path) {
		ageGroupService.SetSource(path);
	}

	public List<GroupConfig> GetData() {
		return ageGroupService
			.GetData()
			.SelectMany((group) => group.Groups)
			.ToList();
	}

	public void SetData(List<GroupConfig> data) {
		var oldAgeGroups = ageGroupService.GetData();
		
		var ageGroups = oldAgeGroups
			.Select(
				ageGroup => new AgeGroupConfig() {
					Id = ageGroup.Id,
					Name = ageGroup.Name,
					Groups = ageGroup.Groups
						.Select(x=>x.Id)
						.Select(id => data.First(g=>g.Id == id))
						.ToList()
				})
			.ToList();
		
		ageGroupService.SetData(ageGroups);
	}

	public GroupConfig? GetById(Guid id) {
		return GetData().FirstOrDefault(x => x.Id == id);
	}

	public bool TryGetById(Guid id, out GroupConfig? result) {
		result = GetById(id);
		return result != null;
	}

	public async Task UpdateReferencesAsync() {
		await ageGroupService.UpdateReferencesAsync();
	}
}
