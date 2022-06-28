using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WebcamViewerUWP {
    public class GridLengthConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) => new GridLength((double) value);
        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            GridLength gridlength = (GridLength)value;
            return gridlength.Value;
        }
    }
}
