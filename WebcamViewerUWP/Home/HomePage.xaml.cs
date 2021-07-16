using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WebcamViewerUWP.Home
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
        }

        private async void main_navView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Navigation test";

            var item = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.InvokedItemContainer;

            dialog.Content = string.Format("You pressed \"{0}\", with a tag of '{1}'.", item.Content.ToString(), item.Tag.ToString());
            dialog.PrimaryButtonText = "OK";

            await dialog.ShowAsync();
        }
    }
}
