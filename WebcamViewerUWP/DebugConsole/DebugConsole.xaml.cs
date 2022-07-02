using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using static WebcamViewerUWP.Logging;
using System.Collections.Generic;

namespace WebcamViewerUWP {
    public sealed partial class DebugConsole : UserControl {
        public static implicit operator bool(DebugConsole console) => console!= null;

        static DebugConsole instance;
        public static DebugConsole get_instance() => instance;
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

        DebugConsoleCommands command_system;

        private void console_Loaded(object sender, RoutedEventArgs e) {
            instance = this;

            open_anim  = (Storyboard)Resources["open_anim"];
            close_anim = (Storyboard)Resources["close_anim"];
            if (open_anim == null || close_anim == null) Popups.TextContentDialog("anim assignment failed");

            if (!is_open()) close(anim: false);

            command_system = DebugConsoleCommands.get_instance();

            log("[console] Console loaded.");
        }

        public void open() {
            if (state == State.Open) return;
            state = State.Open;

            Visibility = Visibility.Visible;
            close_anim.Stop();
            open_anim.Begin();

            input_field.Focus(FocusState.Programmatic);
        }
        public void close(bool anim = true) {
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

        static int CONSOLE_HistoryLength = 60;
        List<string> history = new List<string>(CONSOLE_HistoryLength);

        int history_index = -1;

        /// <param name="dir">-1 means backwards, 1 means forwards.</param>
        void history_next(int dir = 1) {
            if (history.Count == 0) return;

            history_index += dir;

            if (history_index >= history.Count) history_index = 0;
            else if (history_index < 0) history_index = history.Count - 1;

            input_field.Text = history[history_index];
        }
        private void input_field_TextChanged(object sender, TextChangedEventArgs e) {
            history_index = -1;
        }

        static bool CONSOLE_ClearInputFieldOnSubmit = true;
        private void submit_button_Click(object sender, RoutedEventArgs e) {
            string input = input_field.Text;
            submit(input);
        }
        private void input_field_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e) {
            // Submit:
            if (e.Key == VirtualKey.Enter) submit(input_field.Text);

            // History navigation:
            if (e.Key == VirtualKey.Up) history_next(1);
            else if (e.Key == VirtualKey.Down) history_next(-1);
        }

        public void submit(string input) {
            // log($"[console] submit: {input}");
            if (CONSOLE_ClearInputFieldOnSubmit) input_field.Text = string.Empty;

            if (history.Count == 0 || (history[0] != input && input != "")) {
                history.Insert(0, input);
                if (history.Count > CONSOLE_HistoryLength) history.RemoveAt(history.Count - 1);
            }

            command_system.interpret_input(input);
        }
    }
}
