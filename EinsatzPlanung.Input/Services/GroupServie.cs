namespace EinsatzPlanung.Input.Services;

using System.Collections.Generic;

using Einsatzplanung.Excel.Models;
using Einsatzplanung.Types.Models;
using Einsatzplanung.Excel.Services;
using EinsatzPlanung.Input.Interfaces;
using System.Linq;

public class GroupService : IEntityService<Group> {

	private const int AGE_COLUMN_INDEX = 0;
	private const int CLASS_COLUMN_INDEX = 1;
	private const int SCOOLWEEK_COLUMN_INDEX = 2;

	private ExcelImportService excelImportService;
	private IEntityService<AgeGroup> ageGroupService;
	private string SourceFilePath { get; set; } = "";

	public GroupService(ExcelImportService excelImportService, IEntityService<AgeGroup> ageGroupService) {
		this.excelImportService = excelImportService;
		this.ageGroupService = ageGroupService;
	}

	public void SetSource(string path) {
		ageGroupService.SetSource(path);
	}

	public List<Group> ParseSource() {
		return ageGroupService.ParseSource().SelectMany((group) => group.Groups).ToList();
	}

}
