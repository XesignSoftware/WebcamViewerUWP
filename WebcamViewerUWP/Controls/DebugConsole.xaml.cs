using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using static WebcamViewerUWP.Logging;

namespace WebcamViewerUWP {
    public sealed partial class DebugConsole : UserControl {
        public static implicit operator bool(DebugConsole view) => view != null;

        static DebugConsole instance;
        public static DebugConsole GetInstance() => instance;
        public DebugConsole() {
            this.InitializeComponent();
            if (!instance) return;
            instance = this;
        }

        public enum State      { Open, Closed }
        public enum Size_State { Quake, Maximized, Window }

        public State      state = State.Closed;
        public Size_State size_state;

        Storyboard open_anim;
        Storyboard close_anim;

        private void console_Loaded(object sender, RoutedEventArgs e) {
            instance = this;

            open_anim  = (Storyboard)Resources["open_anim"];
            close_anim = (Storyboard)Resources["close_anim"];
            if (open_anim == null || close_anim == null) Popups.TextContentDialog("anim assignment failed");

            if (!is_open()) close(anim: false);

            log("[console] Console loaded.");
        }

        public void open() {
            if (state == State.Open) return;
            state = State.Open;

            Visibility = Visibility.Visible;
            close_anim.Stop();
            open_anim.Begin();
        }
        public async void close(bool anim = true) {
            if (state == State.Closed) return;
            state = State.Closed;

            open_anim.Stop();
            if (anim) close_anim.Begin();
            else Visibility = Visibility.Collapsed;
        }

        public void toggle() {
            if (state == State.Closed) open();
            else close();
        }

        public bool is_open() => state == State.Open;

        private void submit_button_Click(object sender, RoutedEventArgs e) {
            log($"[console] submit: {console_textbox.Text}");
        }
    }
}
