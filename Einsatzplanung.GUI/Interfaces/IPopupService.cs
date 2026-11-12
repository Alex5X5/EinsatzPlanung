namespace Einsatzplanung.GUI.Interfaces; 

using System.Threading.Tasks;

	public interface IPopupService {
		Task<bool> ConfirmAsync(string title, string message);

		void Notify(string title, string message);

		void Warn(string errorMessage,string title = "Error", string message = "Ein fehler ist aufgetreten!");
	}