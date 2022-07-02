using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace WebcamViewerUWP.Engines {
    public class ImageCamera {
        static string get_uri_time_dummy() => "?dummy=" + DateTime.Now.Millisecond;

        public static async Task<BitmapImage> get_image_from_uri(Uri uri) {
            byte[] bytes;
            try {
                using (HttpClient client = new HttpClient()) {
                    bytes = await client.GetByteArrayAsync(uri + get_uri_time_dummy());
                }
            } catch (Exception ex) {
                Popups.TextContentDialog("Failed to load camera image.", $"Error: {ex.Message}\nCamera URI: {uri}");
                return null;
            }

            if (bytes == null || bytes.Length == 0) {
                Popups.TextMessageDialog("Failed to load camera image.", $"Error: bytes were null or empty!\nCamera URI: {uri}");
                return null;
            }

            BitmapImage image = new BitmapImage();
            MemoryStream stream = new MemoryStream(bytes);
            await image.SetSourceAsync(stream.AsRandomAccessStream());

            return image;
        }
    }
}
