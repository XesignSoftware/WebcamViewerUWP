using Windows.UI.Xaml;

public static class BoolExtensions
{
    public static Visibility Visibility(this bool value) => value ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
}