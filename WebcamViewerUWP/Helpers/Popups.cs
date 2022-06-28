using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace WebcamViewerUWP {
    public static class Popups {
        public async static Task<IUICommand> TextMessageDialogAsync(string title = "", string text = "", string button_text = "OK") {
            MessageDialog dialog = new MessageDialog(text, title);
            dialog.Commands.Add(new UICommand(button_text, null));
            return await dialog.ShowAsync();
        }

        public static void TextMessageDialog(string title = "", string text = "", string button_text = "OK") {
            _ = TextMessageDialogAsync(title, text, button_text);
        }
    }
}
