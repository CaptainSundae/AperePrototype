using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;

namespace SimpleCameraApp
{
    public class faceApiCall
    {
        //member variables
        private string path;
        private string responseJSON;

        //default constructor
        public faceApiCall()
        {
            path = "";
        }

        //overloaded constroctor
        public faceApiCall(string path)
        {
            MakeDetectRequest(path);
        }
        public string getPath()
        {
            return path;
        }
        public void setPath(string pathName)
        {
            path = pathName;
        }

        public string getResponseJSON()
        {
            return responseJSON;
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

            // Request headers 
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "af343b3eafef43d0987a3d1a9ebf2448");

            // Request parameters and URI string.
            string queryString = "returnFaceId=true&returnFaceLandmarks=false&returnFaceAttributes=age,gender";
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
                responseContent = response.Content.ReadAsStringAsync().Result;
            }
            //A peak at the JSON response.
            System.Diagnostics.Debug.WriteLine(responseContent);
            // Create sample file; replace if exists.
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync("sample1.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);

            storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            sampleFile = await storageFolder.GetFileAsync("sample1.txt");

            await Windows.Storage.FileIO.WriteTextAsync(sampleFile, responseContent);


        }
    }

}
