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

using Windows.Devices.Gpio;
using Windows.UI.Core;

namespace SimpleCameraApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //Member Variables
        private MediaCapture capture = new MediaCapture();
        public BitmapImage bImage;

        public IRandomAccessStream stream;
        //Clone of the photo stream
        public IRandomAccessStream rasClone;
        private string imageFileName;
        private int imageFileNumber = 0;
        StorageFolder local = ApplicationData.Current.LocalFolder;
        private string path;
        //Recomendation features
        //Initializing call to API by loading subscription key into the client
        const string apiKey = "af343b3eafef43d0987a3d1a9ebf2448";
        FaceServiceClient FaceServiceClient = new FaceServiceClient(apiKey);
        private double age;
        private string gender;
        private double beard;
        private double sideBurns;
        private double moustache;


        public MainPage()
        {
            this.InitializeComponent();
            //always enable preview so preview button is no longer needed
            preview();


        }

        private async void Photo_Click(object sender, RoutedEventArgs e)
        {
            Suggestion.RenderTransform = new CompositeTransform { TranslateX = 0 };
            await TakePhoto();
            makeRecommendation(rasClone);



        }
        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            Suggestion.RenderTransform = new CompositeTransform { TranslateX = 550 };
            Image1.Source = null;
        }


        public async void preview()
        {
            //Initialize media
            var media = new MediaCaptureInitializationSettings();
            await this.capture.InitializeAsync(media);
            //set media capture element in UI to the capture device and start the stream
            this.Capture1.Source = capture;
            await this.capture.StartPreviewAsync();
        }

        private async Task TakePhoto()
        {
            // Set properties of image(jpeg) and Capture a image into a new stream
            ImageEncodingProperties properties = ImageEncodingProperties.CreateJpeg();
             
            using (IRandomAccessStream ras = new InMemoryRandomAccessStream())
            {

                await this.capture.CapturePhotoToStreamAsync(properties, ras);
                await ras.FlushAsync();
                // Load the image into a BitmapImage set stream to 0 for next photo
                ras.Seek(0);
                var picLocation = new BitmapImage();
                picLocation.SetSource(ras);

                //place into listview
                var img = new Image() { Width = 200, Height = 158 };
                img.Source = picLocation;
                Image1.Source = picLocation;
                //Clone the stream and use this to run api call
                rasClone = ras.CloneStream();
            }
        }
        public async void makeRecommendation(IRandomAccessStream ras)
        {
            var att = new FaceAttributeType[] { FaceAttributeType.Age, FaceAttributeType.Gender, FaceAttributeType.Smile, FaceAttributeType.FacialHair, FaceAttributeType.Emotion, FaceAttributeType.Glasses };

            using (Stream s = ras.AsStream()) //File.OpenRead(imageFilePath))
            {
                var a = await FaceServiceClient.DetectAsync(s, returnFaceAttributes: att);


                for (int i = 0; i < a.Length; i++)
                {
                    Face face = a[i];
                    var features = face.FaceAttributes;
                    age = features.Age;
                    if (features.FacialHair.Moustache >= 0.4 || features.FacialHair.Beard >= 0.4 || features.FacialHair.Sideburns >= 0.4 && features.Gender == "male")
                    {
                        ResultTitle.Text = "Little Book of Beards";
                        BookCover.Source = new BitmapImage(new Uri("http://i357.photobucket.com/albums/oo17/nikisiasoco/bookOfBeards_zpsl7aqlwwa.jpg?t=1498900330"));
                        SummaryContent.Text = "A book for men, the manliest of men. Our bearded guardians, our stubbled knights.";
                    }
                    else if (age < 15)
                    {
                        ResultTitle.Text = "Twilight";
                        BookCover.Source = new BitmapImage(new Uri("http://i357.photobucket.com/albums/oo17/nikisiasoco/Twilightbook_zpszj9qzcry.jpg"));
                        SummaryContent.Text = "If you like apples, theres an apple on the cover. I guess.";
                    }
                    else if (age > 45)
                    {
                        ResultTitle.Text = "The Prince";
                        BookCover.Source = new BitmapImage(new Uri("http://i357.photobucket.com/albums/oo17/nikisiasoco/thePrince_zpsvxuexh1h.jpg"));
                        SummaryContent.Text = "Tyranny 101.";
                    }
                    else if (age < 50 && age > 16)
                    {
                        if (features.Emotion.Happiness>0.5) {
                            ResultTitle.Text = "You're Never Weird on the Internet";
                            BookCover.Source = new BitmapImage(new Uri("http://i357.photobucket.com/albums/oo17/nikisiasoco/funny_zpsehbxbjtg.jpg?t=1498958983"));
                            SummaryContent.Text = "This book is read by alot of happy adults.";

                        }
                        else {
                            ResultTitle.Text = "Kafka on the Shore";
                            BookCover.Source = new BitmapImage(new Uri("http://i357.photobucket.com/albums/oo17/nikisiasoco/kafkaOntheShore_zpsl85mx39u.jpg"));
                            SummaryContent.Text = "You look like you have good taste.";
                        }
                        /*
                        System.Diagnostics.Debug.WriteLine(features.FacialHair.Moustache);
                        System.Diagnostics.Debug.WriteLine(features.FacialHair.Sideburns);
                        System.Diagnostics.Debug.WriteLine(features.FacialHair.Beard);
                        System.Diagnostics.Debug.WriteLine(age+"Age inside algorithm. If this works, my calculations are right");
                        */
                    }
                    /*else
                    {
                        System.Diagnostics.Debug.WriteLine("It's still broken, you made a mistake.");
                    }*/
                }

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

