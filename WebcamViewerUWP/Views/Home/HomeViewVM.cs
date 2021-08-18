using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using WebcamViewerUWP.Models;
using Windows.UI.Xaml;

namespace WebcamViewerUWP.Views.Home
{
    public class HomeViewVM : ObservableObject
    {
        public HomeViewVM()
        {
            Cameras = AppState.Cameras;
        }

        public List<ImageCamera> Cameras { get; set; }
        public ICamera CurrentCamera { get; set; }

        public event EventHandler<bool> OnIsLoadingChanged;

        bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set { SetProperty(ref _isLoading, value); OnIsLoadingChanged?.Invoke(this, value); }
        }
    }
}
