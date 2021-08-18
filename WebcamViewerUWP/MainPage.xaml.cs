using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static ContentDialogHelper;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WebcamViewerUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Instance;

        public MainPage()
        {
            this.InitializeComponent();
            Instance = this;

            SetupTitlebar();
        }

        ConfigurationManager config_manager = new ConfigurationManager();

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Configuration test:
            var created = await config_manager.CreateConfigFile("test", true);
            var parsed = await config_manager.ReadConfigFile("test");

            // Load Home:
            SwitchToPage(typeof(Views.Home.HomeView));
        }

        /// Titlebar: ///
        void SetupTitlebar()
        {
            // Extending:
            var core_titlebar = CoreApplication.GetCurrentView().TitleBar;
            core_titlebar.ExtendViewIntoTitleBar = true;
            core_titlebar.LayoutMetricsChanged += (s, args) =>
                titlebar_rowdef.Height = new GridLength(s.Height, GridUnitType.Pixel);

            // Window control button transparency:
            var titlebar = ApplicationView.GetForCurrentView().TitleBar;
            titlebar.ButtonBackgroundColor = Colors.Transparent;
            titlebar.ButtonHoverBackgroundColor = new Color() { R = 0, G = 0, B = 0, A = 40 };
            titlebar.ButtonPressedBackgroundColor = Colors.Transparent;
            titlebar.ButtonInactiveBackgroundColor = Colors.Transparent;
        }

        /// -------------- VIEW MANAGEMENT -------------- ///

        public void SwitchToPage(Type type)
        {
            Page page = null;

            FieldInfo field = type.GetField("Instance", BindingFlags.Public | BindingFlags.Static);
            if (field != null)
                page = (Page)field.GetValue(null);

            if (page == null)
                page = (Page)Activator.CreateInstance(type);

            Frame target_frame = GetLoadedFrameForPage(page);
            if (target_frame == null) target_frame = CreateFrameForPage(page);

            RequestFrameVisibility(target_frame);
        }
        public static bool VIEWMANAGER_AllowReflectionPageCreation = false;
        public void SwitchToPage(string page_name)
        {
            Frame target_frame = GetLoadedFrameForPage(page_name);
            if (target_frame == null)
            {
                if (VIEWMANAGER_AllowReflectionPageCreation) { /* tba */ }
                else TextContentDialog("Could not switch to page", "This page hasn't been loaded yet.\nName: %".Parse(page_name));
            }
        }

        public List<Page> loaded_pages = new List<Page>();
        public Frame CreateFrameForPage(Page page)
        {
            loaded_pages.Add(page);
            Frame frame = new Frame() { Content = page, Name = page.Name };
            AddPageFrame(frame);

            return frame;
        }

        public Frame GetLoadedFrameForPage(Page page) => GetLoadedFrameForPage(page.Name);
        public Frame GetLoadedFrameForPage(string page_name)
        {
            // TODO: Multiple instances support!
            return loaded_frames.Where(x => x.Name == page_name).FirstOrDefault();
        }

        public List<Frame> loaded_frames = new List<Frame>();
        public void AddPageFrame(Frame frame, bool allow_multiple_instances = false)
        {
            if (!allow_multiple_instances && loaded_frames.Any(x => x.Name == frame.Name))
            {
                TextContentDialog("This frame is already loaded.", "You cannot add multiple instances of this frame.");
                return;
            }

            frame.Visibility = Visibility.Collapsed;

            loaded_frames.Add(frame);
            frame_container.Children.Add(frame);
        }

        public void RequestFrameVisibility(Page page) => RequestFrameVisibility(page.Name);
        public void RequestFrameVisibility(string page_name)
        {
            Frame target_frame = loaded_frames.Where(x => x.Name == page_name).FirstOrDefault();
            RequestFrameVisibility(target_frame);
        }
        public void RequestFrameVisibility(Frame frame)
        {
            // Hide all other frames: | TODO: animations!!!
            foreach (Frame f in loaded_frames) f.Visibility = Visibility.Collapsed;

            frame.Visibility = Visibility.Visible;
        }
    }
}
