using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using Windows.Storage.Streams;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

namespace SimpleCameraApp
{
    public class faceApiCall
    {
        //member variables
        private string path;

        const string apiKey = "af343b3eafef43d0987a3d1a9ebf2448";
        FaceServiceClient FaceServiceClient = new FaceServiceClient(apiKey);
        



        //default constructor
        public faceApiCall()
        {
            path = "";

        }
        /*
        public async void takeS(IRandomAccessStream s)
        {

            faceResult = await faceClient.DetectAsync(s.AsStream());

            if (faceResult != null)
            {

                FaceAttributes a = faceResult[0].FaceAttributes;
                System.Diagnostics.Debug.WriteLine("Gender is:" + a.Age);
                System.Diagnostics.Debug.WriteLine("Gender is:" + a.Gender);
            }
        }
        */

        public async void takestream(string imageFilePath)
        {
            var att = new FaceAttributeType[] { FaceAttributeType.Age, FaceAttributeType.Gender, FaceAttributeType.Smile, FaceAttributeType.FacialHair };

            using(Stream s = File.OpenRead(imageFilePath))
            {
                var a = await FaceServiceClient.DetectAsync(s,returnFaceAttributes:att);
                foreach(var face in a)
                {
                    var features = face.FaceAttributes;
                    System.Diagnostics.Debug.WriteLine(face.FaceAttributes.Age);
                    System.Diagnostics.Debug.WriteLine(face.FaceAttributes.Smile);
                    System.Diagnostics.Debug.WriteLine(face.FaceAttributes.Gender);
                    System.Diagnostics.Debug.WriteLine(face.FaceAttributes.FacialHair.Sideburns);
                    System.Diagnostics.Debug.WriteLine(face.FaceAttributes.FacialHair.Moustache);

                    if (features.FacialHair.Moustache > 0.5)
                    {
                        System.Diagnostics.Debug.WriteLine("Strong Moustache game");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Git gud clean faced scrub");
                    }

                    if (face.FaceAttributes.Gender == "male")
                    {

                        System.Diagnostics.Debug.WriteLine("IT WORKED!");

                    }else if (face.FaceAttributes.Gender == "female")
                    {
                        System.Diagnostics.Debug.WriteLine("a bit rude");
                        System.Diagnostics.Debug.WriteLine(face.FaceAttributes.Age);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("failed again.");
                    }
                }


            }

        }

        public async void takeS(IRandomAccessStream ras)
        {
            var att = new FaceAttributeType[] { FaceAttributeType.Age, FaceAttributeType.Gender, FaceAttributeType.Smile, FaceAttributeType.FacialHair };

            using (Stream s = ras.AsStream()) //File.OpenRead(imageFilePath))
            {
                var a = await FaceServiceClient.DetectAsync(s, returnFaceAttributes: att);
                foreach (var face in a)
                {
                    var features = face.FaceAttributes;
                    System.Diagnostics.Debug.WriteLine("RASCLONE WORKED!!!!!!!!!!!!!!!");
                    System.Diagnostics.Debug.WriteLine(face.FaceAttributes.Age);
                    System.Diagnostics.Debug.WriteLine(face.FaceAttributes.Smile);
                    System.Diagnostics.Debug.WriteLine(face.FaceAttributes.Gender);
                    System.Diagnostics.Debug.WriteLine(face.FaceAttributes.FacialHair.Sideburns);
                    System.Diagnostics.Debug.WriteLine(face.FaceAttributes.FacialHair.Moustache);

                    if (features.FacialHair.Moustache > 0.5)
                    {
                        System.Diagnostics.Debug.WriteLine("Strong Moustache game");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Git gud clean faced scrub");
                    }

                    if (face.FaceAttributes.Gender == "male")
                    {

                        System.Diagnostics.Debug.WriteLine("IT WORKED!");

                    }
                    else if (face.FaceAttributes.Gender == "female")
                    {
                        System.Diagnostics.Debug.WriteLine("a bit rude");
                        System.Diagnostics.Debug.WriteLine(face.FaceAttributes.Age);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("failed again.");
                    }
                }


            }

        }



        //overloaded constroctor
        public faceApiCall(string imageFilePath)
        {
            takestream(imageFilePath);
            
        }

        public faceApiCall(IRandomAccessStream ras)
        {
            takeS(ras);

        }
        /*
        public void setStream(IRandomAccessStream currentStream)
        {
            stream = currentStream;
        }
        
        

        static byte[] GetImageAsByteArray(String imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
*/
        /*

                static async void MakeDetectRequest(String imageFilePath)
                {
                    var client = new HttpClient();

                    // Request headers 
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "af343b3eafef43d0987a3d1a9ebf2448");

                    // Request parameters and URI string.
                    string queryString = "returnFaceAttributes=age,gender,smile,facialHair,emotion,glasses";
                    string uri = "https://westus.api.cognitive.microsoft.com/face/v1.0/detect?" + queryString;

                    HttpResponseMessage response;
                    string responseContent;



                    byte[] byteData = GetImageAsByteArray(imageFilePath);

                    using (var content = new ByteArrayContent(byteData))
                    {
                        // This example uses content type "application/octet-stream".
                        // The other content types you can use are "application/json" and "multipart/form-data".
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        response = await client.PostAsync(uri, content);
                        var res = response.Content.ReadAsStreamAsync();
                        responseContent = response.Content.ReadAsStringAsync().Result;

                        //FaceAttributes a  = JsonConvert.DeserializeObject<FaceAttributes>("{\"faceAttributes\":{ \"smile\":1.0,\"gender\":\"male\",\"age\":25.5,\"facialHair\":{ \"moustache\":0.1,\"beard\":0.3,\"sideburns\":0.0},\"glasses\":\"ReadingGlasses\",\"emotion\":{ \"anger\":0.0,\"contempt\":0.0,\"disgust\":0.0,\"fear\":0.0,\"happiness\":1.0,\"neutral\":0.0,\"sadness\":0.0,\"surprise\":0.0},\"makeup\":{ \"eyeMakeup\":false,\"lipMakeup\":false} } }");  //Deserialization<Face>(responseContent);

                        //System.Diagnostics.Debug.WriteLine(a);
                    }
                    */

        /*

            //A peak at the JSON response.
            System.Diagnostics.Debug.WriteLine(responseContent);
        // Create sample file; replace if exists.
        Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync("sampleJSON.json", Windows.Storage.CreationCollisionOption.ReplaceExisting);

        storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        sampleFile = await storageFolder.GetFileAsync("sampleJSON.json");

        await Windows.Storage.FileIO.WriteTextAsync(sampleFile, responseContent);

    }
    */

    }

}
