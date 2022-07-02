using System;

namespace WebcamViewerUWP {
    public class Webcam {
        public enum WebcamType { Image, AnimatedImage, VideoStream, Webpage }

        public Webcam(string name, Uri uri, string owner = null, string location = null) {
            this.name = name;
            this.uri = uri;
            this.owner = owner;
            this.location = location;
        }

        public string name;
        public Uri uri;
        public string owner;
        public string location;
    }
}
