using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;

namespace WebcamViewerUWP {
    public class AppVariables : INotifyPropertyChanged {
        public static AppVariables GetInstance() => (AppVariables)Application.Current.Resources["app_variables"];

        public event PropertyChangedEventHandler PropertyChanged;
        void NotifyPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool TITLEBAR_AllowCustomTitleBar = true;

        private double _system_titlebar_height = 32;
        public double system_titlebar_height {
            get { return _system_titlebar_height; }
            set { _system_titlebar_height = value; NotifyPropertyChanged(); }
        }

        private double _additional_titlebar_height = 0;
        public double additional_titlebar_height {
            get { return _additional_titlebar_height; }
            set { _additional_titlebar_height = value; NotifyPropertyChanged(); }
        }

        public double total_titlebar_height { get { return system_titlebar_height + additional_titlebar_height; } }

        private double _system_titlebar_reserved_width = 0;
        public double system_titlebar_reserved_width {
            get { return _system_titlebar_reserved_width; }
            set { _system_titlebar_reserved_width = value; NotifyPropertyChanged(); }
        }
    }
}