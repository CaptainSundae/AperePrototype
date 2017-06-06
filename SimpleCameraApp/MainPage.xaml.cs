using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Capture;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;

using System;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;



// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SimpleCameraApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {



        public MainPage()
        {
            this.InitializeComponent();


        }
        private async Task CapturePic()
        {
            var camera = new CameraCaptureUI();
            StorageFile file = await camera.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (file == null)
            {
                return;
            }

            using (IRandomAccessStream imageStream = await file.OpenAsync(FileAccessMode.Read))
            {
                // Load the stream into a BitmapImage
                BitmapImage source = new BitmapImage();
                source.SetSource(imageStream);

                this.Image1.Source = source;

            }

            //We need to give MakeDetectRequest(file); the path name

        }


        private async void Pic_Click(object sender, RoutedEventArgs e)
        {
            await CapturePic();


        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        static async void MakeDetectRequest(string imageFilePath)
        {
            var client = new HttpClient();

            // Request headers - replace this example key with your valid key.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "af343b3eafef43d0987a3d1a9ebf2448");

            // Request parameters and URI string.
            string queryString = "returnFaceId=true&returnFaceLandmarks=false&returnFaceAttributes=age,gender";
            string uri = "https://westus.api.cognitive.microsoft.com/face/v1.0/detect?" + queryString;

            HttpResponseMessage response;
            string responseContent;

            // Request body. Try this sample with a locally stored JPEG image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (var content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
                responseContent = response.Content.ReadAsStringAsync().Result;
            }

            //A peak at the JSON response.
            //Console.WriteLine(responseContent);
        }
    }
}

