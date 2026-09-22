namespace Einsatzplanung.Util.Interfaces;

public interface ILastSelectionService {

	/// <summary>
	/// Gets the value saved for the key. Returns an empty string if there is no value set.
	/// </summary>
	public string? GetSelection(string key);

	/// <summary>
	/// Gets the value saved for the key. Returns an empty string if there is no value set.
	/// </summary>
	public string GetSelectionOrDefault(string key);

	/// <summary>
	/// Sets the value for the key.
	/// </summary>
	public void SetSelection(string key, string value);

	/// <summary>
	/// Saves the changes to a file.
	/// </summary>
	public void SaveChanges();
}
