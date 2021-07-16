using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

public static class ContentDialogHelper
{
    public static void TextContentDialog(string title = "", string text = "", string primary_text = "OK")
    {
        ContentDialog dialog = new ContentDialog() { Title = title, Content = text, PrimaryButtonText = primary_text };
        _ = dialog.ShowAsync();
    }
}