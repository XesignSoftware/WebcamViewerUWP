using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;

namespace WebcamViewerUWP {
    public class UserVariables {
        public static implicit operator bool(UserVariables view) => view != null;
        public static UserVariables GetInstance() => (UserVariables)Application.Current.Resources["user_variables"];

        public event PropertyChangedEventHandler PropertyChanged;
        void NotifyPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private List<Webcam> _webcams = new List<Webcam>() {
            new Webcam("Test webcam #1", new Uri("http://gw.msumi.sk:8001/jpg/1/image.jpg"), "Test owner", "Test location"),
            new Webcam("Test webcam #2", new Uri("http://funsat.sk/meteo/webcam/funsat1.jpg"), "Test owner", "Test location"),
            new Webcam("Test webcam #3", new Uri("http://funsat.sk/meteo/webcam/funsat2.jpg"), "Test owner", "Test location"),
        };
        public  List<Webcam>  Webcams {
            get { return _webcams; }
            set { _webcams = value; NotifyPropertyChanged(); }
        }
    }
}
