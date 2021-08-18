using System;
using System.Net;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using static ContentDialogHelper;

public enum CameraType { Image }

namespace WebcamViewerUWP.Models
{
    public interface ICamera
    {
        int ID { get; set; }
        string Name { get; set; }
        string Url { get; set; }
        CameraType Type { get; set; }
    }

    public class ImageCamera : ICamera
    {
        public ImageCamera(string name, string url = "", int id = -1, CameraType type = CameraType.Image)
        {
            Name = name;
            Url = url;
            Type = type;
            ID = id;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public CameraType Type { get; set; }

        string GetDummy() => "?dummy=" + DateTime.Now.Ticks;

        public async Task<BitmapImage> GetBitmapImage()
        {
            BitmapImage image = null;
            byte[] data = null;

            try
            {
                using (WebClient client = new WebClient())
                    data = await client.DownloadDataTaskAsync(Url + GetDummy());
            }
            catch (Exception ex) { TextContentDialog("Error", ex.Message); }

            if (data == null)
                return null;

            using (var stream = new InMemoryRandomAccessStream())
            {
                using (var writer = new DataWriter(stream))
                {
                    writer.WriteBytes(data);
                    await writer.StoreAsync();
                    await writer.FlushAsync();
                    writer.DetachStream();
                }

                stream.Seek(0);

                image = new BitmapImage();
                await image.SetSourceAsync(stream);
            }

            return image;
        }
    }
}
