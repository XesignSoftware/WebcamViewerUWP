using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WebcamViewerUWP.Models;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static ContentDialogHelper;

namespace WebcamViewerUWP.Views.Home
{
    public sealed partial class HomeView : Page
    {
        public static HomeView Instance;
        HomeViewVM ViewModel { get; set; }

        public HomeView()
        {
            InitializeComponent();
            Instance = this;
            ViewModel = new HomeViewVM();
            ViewModel.OnIsLoadingChanged += ViewModel_OnIsLoadingChanged;

            TitlebarSetup();
        }

        void TitlebarSetup()
        {
            CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += (s, args) => rowdef_titlebar.Height = new GridLength(s.Height, GridUnitType.Pixel);
            rowdef_titlebar.Height = new GridLength(AppState.TitlebarHeight, GridUnitType.Pixel);
        }

        private void ViewModel_OnIsLoadingChanged(object sender, bool e)
        {
            if (e) progress_show.Begin();
            else progress_hide.Begin();
        }

        object last_selected_menu_item; // Keep track of the last selected camera item (for Settings item)
        private void main_navView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                Settings();
                main_navView.SelectedItem = last_selected_menu_item; // Reset the selection indicator to last item
                return;
            }

            last_selected_menu_item = args.InvokedItem;
            ICamera c = (ICamera)args.InvokedItem;
            Camera(c);
        }

        async void Camera(ICamera c)
        {
            // TODO: Multiple camera types! We're testing with ImageCamera only for now.
            if (c.Type != CameraType.Image) return;

            ViewModel.IsLoading = true;

            ViewModel.CurrentCamera = c;

            ImageCamera ic = (ImageCamera)c;
            image.Source = await ic.GetBitmapImage();

            ViewModel.IsLoading = false;
        }

        void Settings()
        {
            // TextContentDialog("Settings invoked.");

            // TODO: View switching
            MainPage.Instance.SwitchToPage(typeof(Views.Settings.SettingsView));
            MainPage.Instance.ControlSettingsStuff(true);
        }
    }
}
