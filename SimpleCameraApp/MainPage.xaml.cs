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

using Windows.UI.Xaml.Media.Imaging;

//Aforge to directly take input devices by name
//using AForge.Video;
//using AForge.Video.DirectShow;

using Windows.Media.MediaProperties;




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

        private MediaCapture capture = new MediaCapture();

        private async void PreviewButton_Click_1(object sender, RoutedEventArgs e)
        {
                
                var media = new MediaCaptureInitializationSettings();
                await this.capture.InitializeAsync(media);
                this.Capture1.Source = capture;
                await this.capture.StartPreviewAsync();

        }

        private async Task TakePhoto()
        {

                // Capture a image into a new stream
                ImageEncodingProperties properties = ImageEncodingProperties.CreateJpeg();
                using (IRandomAccessStream ras = new InMemoryRandomAccessStream())
                {

                    await this.capture.CapturePhotoToStreamAsync(properties, ras);
                    await ras.FlushAsync();

                    // Load the image into a BitmapImage
                    ras.Seek(0);
                    var picLocation = new BitmapImage();
                    picLocation.SetSource(ras);

                    //place into listview
                    var img = new Image() { Width = 200, Height = 158 };
                    img.Source = picLocation;
                    ListView1.Items.Add(img);

                }
        }

        private async void Photo_Click(object sender, RoutedEventArgs e)
        {
            await TakePhoto();
        }
    }
}

