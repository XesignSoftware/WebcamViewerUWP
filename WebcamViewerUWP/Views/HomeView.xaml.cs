using WebcamViewerUWP.Engines;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using static WebcamViewerUWP.Logging;

namespace WebcamViewerUWP.Views {
    public sealed partial class HomeView : View {
        public HomeView() {
            this.InitializeComponent();
            titlebar_custom_left = titlebar_left;
            titlebar_custom_right = titlebar_right;
        }

        Storyboard progress_ui_show;
        Storyboard progress_ui_hide;

        private void Page_Loaded(object sender, RoutedEventArgs e) {
            DebugConsoleCommands.get_instance().register_command(toggle_progress_ui);
            progress_ui_show = (Storyboard)Resources["progress_ui_show"];
            progress_ui_hide = (Storyboard)Resources["progress_ui_hide"];
        }

        bool is_progress_ui = false;

        void show_progress_ui() {
            is_progress_ui = true;
            progress_ui_hide.Stop();
            progress_ui_show.Begin();
        }

        void hide_progress_ui() {
            is_progress_ui = false;
            progress_ui_show.Stop();
            progress_ui_hide.Begin();
        }

        void toggle_progress_ui() {
            if (!is_progress_ui) show_progress_ui();
            else hide_progress_ui();
            log($"progress UI state: {is_progress_ui}");
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            MainView.GetInstance().change_view(typeof(Views.TestView));
        }

        private async void HomeMenu_OnEntrySelected(object sender, Webcam e) {
            show_progress_ui();

            BitmapImage image = await ImageCamera.get_image_from_uri(e.uri);
            webcam_image.Source = image;

            hide_progress_ui();
        }
    }
}
