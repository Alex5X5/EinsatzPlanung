using System.Threading.Tasks;

namespace Einsatzplanung.GUI.Services {
	public interface IDialogService {
		Task<bool> ConfirmAsync(string title, string message);
	}
}