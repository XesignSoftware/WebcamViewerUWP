using System;
using System.IO;
using Windows.Storage;

namespace WebcamViewerUWP.Views {
    public sealed partial class TestView : View {
        public TestView() {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            MainView.GetInstance().change_view(typeof(Views.HomeView));
        }

        private async void config_test_button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            StorageFolder local_folder = ApplicationData.Current.LocalFolder;
            StorageFile file = null;
            try {
                file = await local_folder.GetFileAsync("test.conf");
            } catch (FileNotFoundException ex) {
                await local_folder.CreateFileAsync("test.conf");
                // TODO: Write into the file.
            }
            var a = new ConfigFile("test.conf");
        }
    }
}
