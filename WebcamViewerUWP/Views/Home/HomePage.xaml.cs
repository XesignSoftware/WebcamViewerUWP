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

namespace WebcamViewerUWP.Home
{
    public sealed partial class HomePage : Page
    {
        HomePageVM ViewModel { get; set; }

        public HomePage()
        {
            InitializeComponent();
            ViewModel = new HomePageVM();

            ViewModel.OnIsLoadingChanged += ViewModel_OnIsLoadingChanged;
        }

        private void ViewModel_OnIsLoadingChanged(object sender, bool e)
        {
            if (e) progress_show.Begin();
            else progress_hide.Begin();
        }

        private void main_navView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked) { Settings(); return; }

            ICamera c = (ICamera)args.InvokedItem;
            Camera(c);
        }

        public ICamera CurrentCamera { get; set; }

        async void Camera(ICamera c)
        {
            // TODO: Multiple camera types! We're testing with ImageCamera only for now.
            if (c.Type != CameraType.Image) return;

            ViewModel.IsLoading = true;

            CurrentCamera = c;

            ImageCamera ic = (ImageCamera)c;
            image.Source = await ic.GetBitmapImage();

            ViewModel.IsLoading = false;
        }

        void Settings()
        {
            TextContentDialog("Settings invoked.");
            ViewModel.IsLoading = false;
        }
    }
}
