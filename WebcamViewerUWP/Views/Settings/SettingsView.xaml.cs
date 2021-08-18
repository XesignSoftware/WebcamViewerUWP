using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace WebcamViewerUWP.Views.Settings
{
    public sealed partial class SettingsView : Page
    {
        public static SettingsView Instance;

        public SettingsView()
        {
            this.InitializeComponent();
            Instance = this;

            TitlebarSetup();    
        }

        void TitlebarSetup()
        {
            CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += (s, args) => rowdef_titlebar.Height = new GridLength(s.Height, GridUnitType.Pixel);
            rowdef_titlebar.Height = new GridLength(AppState.TitlebarHeight, GridUnitType.Pixel);
        }
    }
}
