namespace Einsatzplanung.GUI.ViewModels.Edit {
    using CommunityToolkit.Mvvm.ComponentModel;

    public partial class TeacherTopicCardViewModel : ObservableObject {
        [ObservableProperty]
        private string topic;

        public TeacherTopicCardViewModel(string topic) {
            this.topic = topic;
        }
    }
}
