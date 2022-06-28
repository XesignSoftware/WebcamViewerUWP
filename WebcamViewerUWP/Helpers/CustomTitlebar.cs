using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using System.Collections.Generic;

namespace WebcamViewerUWP {
    public class CustomTitlebar {
        static CustomTitlebar Instance;
        public static CustomTitlebar GetInstance() {
            if (Instance == null) return new CustomTitlebar();
            return Instance;
        }
        public CustomTitlebar() {
            if (Instance != null) {
                Popups.TextMessageDialog("A CustomTitlebar instance already exists", "Tried creating a new State when one already exists. \nThis is bad!");
                return;
            }
            Instance = this;
        }

        public static State state { get { return State.GetInstance(); } }

        public static FrameworkElement titlebar_grabbable; // @ Naming
        public static Grid titlebar_custom_left;
        public static Grid titlebar_custom_right;

        public static AppVariablesDynamic app_vars { get { return (AppVariablesDynamic)Application.Current.Resources["app_variables"]; } }

        public static void setup_custom_titlebar(FrameworkElement titlebar_grabbable) {
            // We're storing the titlebar container globally. In the moment, we need it for the LayoutMetricsChanged() event,
            // but we're probably going to want to access the titlebar container itself in other views as well, so this 
            // is useful for that use case as well.
            CustomTitlebar.titlebar_grabbable = titlebar_grabbable;

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            Window.Current.SetTitleBar(titlebar_grabbable);

            // Set window control colors:
            // TODO: factor out into a different function?
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            // TODO: make these theme brushes:
            titleBar.ButtonHoverBackgroundColor = Color.FromArgb(35, 0, 0, 0);
            titleBar.ButtonPressedBackgroundColor = Color.FromArgb(55, 0, 0, 50);
        }
        static void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args) {
            app_vars.system_titlebar_height = sender.Height;
            // TODO: left inset?
            app_vars.system_titlebar_reserved_width = sender.SystemOverlayRightInset;

            // Update our custom titlebar container height when the actual titlebar changes.
            // This is also used to set the proper titlebar height on app startup.
            //titlebar_grabbable.Height = app_vars.system_titlebar_height;
        }

        // Custom elements:

        public enum Side { Left, Right }
        Grid get_titlebar_custom_side(Side side) {
            switch (side) {
                default:
                case Side.Left: return titlebar_custom_left;
                case Side.Right: return titlebar_custom_right;
            }
        }

        public struct TitlebarCustomElementInfo {
            public TitlebarCustomElementInfo(Side side, FrameworkElement element) {
                this.side = side;
                this.element = element;
                parent = (Grid)element.Parent;
            }
            public Side side;
            public FrameworkElement element;
            public Grid parent;
        }

        List<TitlebarCustomElementInfo> custom_elements = new List<TitlebarCustomElementInfo>();

        // NOTE TODO: The element and parent should both be Grids!
        // We might want to allow StackPanels or other containers.
        public TitlebarCustomElementInfo register_element_in_titlebar(Side side, FrameworkElement element) {
            TitlebarCustomElementInfo info = new TitlebarCustomElementInfo(side, element);
            custom_elements.Add(info);

            // 1. Unregister the element from its current parent:
            unregister_parent_from_element(element);
            // 2. Add element to the custom titlebar:
            get_titlebar_custom_side(side).Children.Add(element);

            return info;
        }

        public void unregister_info_from_titlebar(TitlebarCustomElementInfo info) {
            // 1. Unregister the element from its current parent:
            unregister_parent_from_element(info.element);
            // 2. Re-add element to its original parent:
            info.parent.Children.Add(info.element);
            // 3. Remove the info:
            custom_elements.Remove(info);
        }

        // ----- //

        void unregister_parent_from_element(FrameworkElement element) {
            // TODO: Hardcoding with Grids for now:
            Grid parent_grid = (Grid)element.Parent;
            parent_grid.Children.Remove(element); // TODO: error checking? Does element exist within?
        }

        // ----- //

        public void maybe_register_view_custom_elements(View view) {
            // TODO: We might want to think of multiple different regions within the titlebar, not just left and right...
            if (view.titlebar_custom_left != null) 
                view.infos.Add(register_element_in_titlebar(Side.Left, view.titlebar_custom_left));
            if (view.titlebar_custom_right != null) 
                view.infos.Add(register_element_in_titlebar(Side.Right, view.titlebar_custom_right));
        }

        public void maybe_unregister_view_custom_elements(View view) {
            if (!view) return;
            foreach (TitlebarCustomElementInfo info in view.infos)
                unregister_info_from_titlebar(info);
            view.infos.Clear();
        }
    }
}
