using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        public static AppVariables app_vars = AppVariables.GetInstance();

        private void Page_Loaded(object sender, RoutedEventArgs e) {
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

        public void change_view(string view_name) {
            string view_type_name = $"{nameof(WebcamViewerUWP)}.Views.{view_name}";
            Type view_type = Type.GetType(view_type_name);
            if (view_type == null) {
                Popups.TextMessageDialog("UI navigation failed", $"Could not navigate to requested page '{view_name}'.");
                return;
            }
            view_frame.Navigate(view_type);
        }
    }
}
