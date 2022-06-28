using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WebcamViewerUWP {
    public class AppVariablesDynamic : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        void NotifyPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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