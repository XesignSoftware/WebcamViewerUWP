using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace WebcamViewerUWP {
    public static class Popups {
        // ----- Win8-style MessageDialog ----- //
        public async static Task<IUICommand> TextMessageDialogAsync(string title = "", string text = "", string button_text = "OK") {
            MessageDialog dialog = new MessageDialog(text, title);
            dialog.Commands.Add(new UICommand(button_text, null));
            return await dialog.ShowAsync();
        }

        public static void TextMessageDialog(string text = "") => TextMessageDialog(null, text);
        public static void TextMessageDialog(string title = "", string text = "", string button_text = "OK") {
            _ = TextMessageDialogAsync(title, text, button_text);
        }

        // ----- ContentDialog ----- //
        public async static Task<ContentDialogResult> TextContentDialogAsync(string title = "", string text = "", string button_text = "OK") {
            ContentDialog dialog = new ContentDialog() { Title = title, Content = text, PrimaryButtonText = button_text };
            return await dialog.ShowAsync();
        }

        public static void TextContentDialog(string text = "") => TextContentDialog(null, text);
            public static void TextContentDialog(string title = "", string text = "", string button_text = "OK") {
            _ = TextContentDialogAsync(title, text, button_text);
        }
    }
}
