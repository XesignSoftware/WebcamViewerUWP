using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WebcamViewerUWP.Models;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

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
        }

        private void ViewModel_OnIsLoadingChanged(object sender, bool e)
        {
            if (e) progress_show.Begin();
            else progress_hide.Begin();
        }

        object last_selected_menu_item;

        private void main_navView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked) { Settings(); main_navView.SelectedItem = last_selected_menu_item; return; }

            last_selected_menu_item = args.InvokedItem;
            ICamera c = (ICamera)args.InvokedItem;
            Camera(c);
        }

        async void Camera(ICamera c)
        {
            TextContentDialog("Camera() called!");

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
        }
    }
}
