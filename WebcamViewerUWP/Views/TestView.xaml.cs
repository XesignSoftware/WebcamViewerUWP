namespace WebcamViewerUWP.Views {
    public sealed partial class TestView : View {
        public TestView() {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            MainView.GetInstance().change_view(typeof(Views.HomeView));
        }
    }
}
