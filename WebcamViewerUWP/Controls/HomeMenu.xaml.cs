using System;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;

namespace WebcamViewerUWP.Controls {
    public sealed partial class HomeMenu : UserControl {
        public HomeMenu() {
            this.InitializeComponent();
        }

        public event EventHandler<Webcam> OnEntrySelected;

        private void listview_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            // Since this is a Single-selection mode list, we only ever want to get one argument from
            // a selection change event.
            Debug.Assert(e.AddedItems.Count == 1);

            // Make sure that the objects we want to get back are of the right type:
            Debug.Assert(e.AddedItems[0].GetType() == typeof(Webcam));

            Webcam w = (Webcam)e.AddedItems[0];
            OnEntrySelected?.Invoke(this, w);
        }
    }
}
