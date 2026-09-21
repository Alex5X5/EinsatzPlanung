namespace Einsatzplanung.Util.Services;

public class LastSelectionService {

	private YmlService ymlService = new();

	private static readonly string PATH = PathService.FilesPath("last_selection");

	public LastSelectionService() {
		ymlService.ReadFromFile(PATH);
	}

	public string GetSelectionForKey(string key) {
		return ymlService[key];
	}

	public void SetSelectionForKey(string key, string value) {
		ymlService[key] = value;
	}

	public void SaveChanges() {
		ymlService.WriteTofile(PATH);
	}
}
