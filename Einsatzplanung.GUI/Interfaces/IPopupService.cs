namespace Einsatzplanung.GUI.Interfaces; 

using System.Threading.Tasks;

	public interface IPopupService {
		
		///  <summary>
		///	 Creates an async popup that waits for a user input and returns a bool
		///  </summary>
		///  <param name="title"> <see cref="string"/> </param>
		///  <param name="message"><see cref="string"/> </param>
		/// <returns><see cref="bool"/></returns>
		Task<bool> ConfirmAsync(string title, string message);

		///  <summary>
		///	 Creates a popup with a notification
		///  </summary>
		///  <param name="title"> <see cref="string"/> </param>
		///  <param name="message"><see cref="string"/> </param>
		void Notify(string title, string message);

		///  <summary>
		///	 Creates a popup with a warning message
		///  </summary>
		///  <param name="errorMessage"> <see cref="string"/> </param>
		///  <param name="title"> <see cref="string"/> </param>
		///  <param name="message"><see cref="string"/> </param>
		void Warn(string errorMessage,string title = "Error", string message = "Ein fehler ist aufgetreten!");
	}