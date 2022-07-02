using System;
using Windows.UI.Xaml.Data;

namespace WebcamViewerUWP.Converters {
    public class NegativeValueConverter : IValueConverter {
        // Hard-coded for integers only at the moment!

        public object Convert(object value, Type targetType, object parameter, string language) {
            int value_as_int = (int)value;
            return -value_as_int;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            return null;
        }
    }
}
