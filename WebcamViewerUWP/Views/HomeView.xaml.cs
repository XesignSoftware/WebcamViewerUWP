using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WebcamViewerUWP.Views {
    public sealed partial class HomeView : View {
        public HomeView() {
            this.InitializeComponent();
            titlebar_custom_left = titlebar_left;
            titlebar_custom_right = titlebar_right;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) {
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            MainView.GetInstance().change_view(typeof(Views.TestView));
        }
    }
}
