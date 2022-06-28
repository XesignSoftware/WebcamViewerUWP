using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace WebcamViewerUWP {
    public class View : Page {
        public static implicit operator bool(View view) {
            return view != null;
        }

        // ----- //

        public View() {
            Loaded += View_Loaded;
        }
        private void View_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            loaded = true;
        }

        public bool loaded = false;
        public async Task<bool> wait_until_loaded() {
            while (!loaded) await Task.Delay(1);
            return loaded;
        }

        // ----- //

        public async void view_requested() {
            // HACK: Wait for the page to be loaded before messing with custom titlebar elements:
            if (!loaded) await wait_until_loaded();
            CustomTitlebar.GetInstance().maybe_register_view_custom_elements(this);
        }
        public void view_disconnect() {
            CustomTitlebar.GetInstance().maybe_unregister_view_custom_elements(this);
        }

        // View animations / transitions?

        public Grid titlebar_custom_left  = null;
        public Grid titlebar_custom_right = null;
        public List<CustomTitlebar.TitlebarCustomElementInfo> infos = new List<CustomTitlebar.TitlebarCustomElementInfo>();
    }
}
