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
using Windows.Media.MediaProperties;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

namespace SimpleCameraApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //StorageFolder appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
        //StorageFolder assets = await appInstalledFolder.GetFolderAsync("Assets");
        private MediaCapture capture = new MediaCapture();
        public BitmapImage bImage;
        public IRandomAccessStream stream;
        StorageFolder local = ApplicationData.Current.LocalFolder;
        private string imageFileName;
        private int imageFileNumber = 0;
        private string path;
        public IRandomAccessStream rasClone;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void PreviewButton_Click_1(object sender, RoutedEventArgs e)
        {
            PreviewButton.IsEnabled = false;
            preview();
        }

        private async void Photo_Click(object sender, RoutedEventArgs e)
        {
            Suggestion.RenderTransform = new CompositeTransform { TranslateX = 0 };
            await TakePhoto();



        }
        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            Suggestion.RenderTransform = new CompositeTransform { TranslateX = 550 };
            Image1.Source = null;
        }
        private void usePhoto_Click(object sender, RoutedEventArgs e)
        {
            faceApiCall rasCall = new faceApiCall(rasClone);
            //faceApiCall call = new faceApiCall(@"C:\Users\nikis\AppData\Local\Packages\e6204497-620b-4f81-ab82-fbd8d000332b_x3hap7anq5jnt\LocalState\" + imageFileName);
           

        }

        public async void preview()
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
                Image1.Source = picLocation;
                //ListView1.Items.Add(img);
                stream = ras;
                /*
                Stream s=ras.AsStream();
                await s.FlushAsync();
                    faceApiCall call=new faceApiCall(s);
                */
                rasClone = ras.CloneStream();

                saveFile();

            }





        }
        public async void saveFile()
        {
            imageFileName = "apereImage7" + imageFileNumber + ".jpg";
            imageFileNumber += 1;

            //Save: Start by copying the stream and loading it into a decoder
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream.CloneStream());
            SoftwareBitmap softBitmap = await decoder.GetSoftwareBitmapAsync();
            //To actually save, we need to encode it. This first line creates a file with name image1.jpg and replaces the image if it exists
            StorageFile saveFile = await local.CreateFileAsync(imageFileName, CreationCollisionOption.ReplaceExisting);
            //Bitmap encoder encode in jpeg, specify it to the stream above "saveFile" openasync allows me to write to this stream
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(Windows.Graphics.Imaging.BitmapEncoder.JpegEncoderId, await saveFile.OpenAsync(FileAccessMode.ReadWrite));
            encoder.SetSoftwareBitmap(softBitmap);
            await encoder.FlushAsync();
            FileInfo f = new FileInfo(imageFileName);
            path = f.FullName;
            System.Diagnostics.Debug.WriteLine(f.FullName);


        }


    }
}

