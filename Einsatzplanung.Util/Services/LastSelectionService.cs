namespace Einsatzplanung.Util.Services;

using Einsatzplanung.Util.Interfaces;

using System.Collections.Generic;
using System.IO;
using System.Linq;

public class LastSelectionService : ILastSelectionService {

	private static readonly string PATH = PathService.FilesPath("last_selection");

	private readonly Dictionary<string, string> data;

	public LastSelectionService() {
		data = [];
		ReadFromFile();
	}

	public string? GetSelection(string key) {
		if(data.TryGetValue(key, out var val))
			return val;
		return null;
	}

	public string GetSelectionOrDefault(string key) {
		if (data.TryGetValue(key, out var val))
			return val;
		return "";
	}

	public void SetSelection(string key, string value) {
		data[key] = value;
	}

	public void SaveChanges() {
		IEnumerable<string> lines = data.Select(pair => $"{pair.Key}:{pair.Value}");
		string text = string.Join('\n', lines);
		File.WriteAllText(PATH, text);
	}

	private void ReadFromFile() {
		
		string[] lines = File.Exists(PATH) ? File.ReadAllLines(PATH) : [];

		foreach (var line in lines) {

			if (string.IsNullOrEmpty(line))
				continue;

			string[] segments = line.Split(':', 2);

			if (segments.Length == 0)
				continue;
			if (segments.Length == 1)
				data[segments[0]] = "";
			if (segments.Length == 2)
				data[segments[0]] = segments[1];
		}
	}
}
