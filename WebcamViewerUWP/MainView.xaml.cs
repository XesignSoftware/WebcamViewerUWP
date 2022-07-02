using System;
using Windows.System;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static WebcamViewerUWP.Logging;

namespace WebcamViewerUWP {
    public sealed partial class MainView : Page {
        static MainView Instance;
        public static MainView GetInstance() {
            return Instance;
        }
        public MainView() {
            this.InitializeComponent();
            Instance = this;
        }

        AppVariables app_vars = AppVariables.GetInstance();
        DebugConsole console { get { return DebugConsole.get_instance(); } }

        private void Page_Loaded(object sender, RoutedEventArgs e) {
            register_console_commands();

            if (app_vars.TITLEBAR_AllowCustomTitleBar) {
                CustomTitlebar.setup_custom_titlebar(titlebar_grabbable);
                CustomTitlebar.titlebar_custom_left = titlebar_custom_left;
                CustomTitlebar.titlebar_custom_right = titlebar_custom_right;

                // Ensure that Views can expand into the titlebar area:
                Grid.SetRow(view_frame, 0);
                Grid.SetRowSpan(view_frame, 2);
            } else {
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;
                Grid.SetRow(view_frame, 1);
            }

            change_view(typeof(Views.TestView));

            log("[mainview] Application loaded.");
        }

        void register_console_commands() {
            DebugConsoleCommands commands = DebugConsoleCommands.get_instance();
            var cmd = commands.register_command(change_view, "Change the view to a specified 'View' type.");
            cmd.close_console_on_invoke = true;
        }

        View current_view = null;

        public void change_view(Type view_type) {
            /*
            if (view_type == null) {
                Popups.TextMessageDialog("UI navigation failed", "Type was null.");
                return;
            }
            */
            if (view_type != null && view_type.BaseType != typeof(View))
                Popups.TextMessageDialog($"UI navigation page is not of type '{nameof(View)}'!", $"'{view_type.Name}' has a base type of '{view_type.BaseType}'.");

            current_view?.view_disconnect();

            if (view_type != null) {
                view_frame.Visibility = Visibility.Visible;
                view_frame.Navigate(view_type);
                current_view = (View)view_frame.Content; // HACK
            } else {
                view_frame.Visibility = Visibility.Collapsed;
                current_view = null;
            }

            current_view?.view_requested();
        }
        public void change_view(string[] args) {
            if (args == null || args.Length == 0) {
                log("change_view(): no arguments were given.");
                return;
            }

            string view_type_name = $"{nameof(WebcamViewerUWP)}.Views.{args[0]}";
            Type view_type = Type.GetType(view_type_name);
            if (view_type == null) {
                string message = $"Non-existent view type '{view_type_name}'.";
                Popups.TextMessageDialog("UI navigation failed!", message);
                log(message);
                return;
            }
            else if (view_type.BaseType != typeof(View)) {
                string message = $"Invalid view type '{view_type.BaseType.Name}' for view '{view_type_name}'.";
                Popups.TextMessageDialog("UI navigation failed!", message);
                log(message);
                return;
            }

            change_view(view_type);
        }

        private void Page_PreviewKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e) {
            if (e.Key == VirtualKey.Number0) console.toggle();
            if (e.Key == VirtualKey.Escape && console.is_open()) console.close();
        }
    }
}
