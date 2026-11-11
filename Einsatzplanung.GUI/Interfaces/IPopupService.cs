using System.Threading.Tasks;

namespace Einsatzplanung.GUI.Services {
	public interface IPopupService {
		Task<bool> ConfirmAsync(string title, string message);
	}
}