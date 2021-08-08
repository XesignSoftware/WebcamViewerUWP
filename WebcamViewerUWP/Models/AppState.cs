using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebcamViewerUWP.Models
{
    public static class AppState
    {
        static AppState()
        {
            Cameras = new List<ImageCamera>()
            {
                new ImageCamera("Test 1", "http://gw.msumi.sk:8001/jpg/1/image.jpg"),
                new ImageCamera("Test 2", "http://gecom.sk/img/kam-autobuska-orig.jpg"),
                new ImageCamera("Test 3", "http://funsat.sk/meteo/webcam/funsat2.jpg"),
            };
        }

        public static List<ImageCamera> Cameras { get; set; }
    }
}
