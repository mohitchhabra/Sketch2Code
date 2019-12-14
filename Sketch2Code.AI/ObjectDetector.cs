using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;
using Sketch2Code.AI.Entities;
using Line = Sketch2Code.AI.Entities.Line;

namespace Sketch2Code.AI
{
    public class ObjectDetector : CustomVisionClient
    {
        private const int numberOfCharsInOperationId = 36;
        private static readonly HttpClient Client = new HttpClient { Timeout = TimeSpan.FromMinutes(1) };

        public ObjectDetector()
            : base(ConfigurationManager.AppSettings["ObjectDetectionApiKey"],
                   ConfigurationManager.AppSettings["ObjectDetectionPublishedModelName"],
                   ConfigurationManager.AppSettings["ObjectDetectionProjectName"])
        {
        }

        public ObjectDetector(string trainingKey, string predictionKey, string projectName) 
            : base(trainingKey, predictionKey, projectName)
        {

        }

        public async Task<PredictionResult> GetDetectedObjects(byte[] image)
        {
            return await PredictImageAsync(image);
        }

        public async Task<List<String>> GetText(byte[] image)
        {
            var list = new List<String>();
            var lines = await GetTextLines(image);

            if (lines != null)
            {
              //  list = lines.SelectMany(l => l.Words?.Select(w => w.Text)).ToList();
            }
            
            return list;
        }

        public async Task<List<Line>> GetTextLines(byte[] image)
        {
            try
            {
                string url = @"https://cv1aiboot.cognitiveservices.azure.com/vision/v2.0/ocr?languag=unk&detectOrientation=true";
                string responseContent;
                using (var content = new ByteArrayContent(image))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    content.Headers.Add("Ocp-Apim-Subscription-Key", "ba1222dd88644efebc5a73e666684277");
                    var response = await Client.PostAsync(url, content);
                    response.EnsureSuccessStatusCode();
                    responseContent = await response.Content.ReadAsStringAsync();
                    var ocrData = JsonConvert.DeserializeObject<OCRData> (responseContent);
                    return (from region in ocrData.regions
                            from line in region.lines
                            select line).ToList();
                }


                //var client = new HttpClient();
                //    var queryString = HttpUtility.ParseQueryString(string.Empty);

                //    // Request headers
                //    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "");

                //    // Request parameters
                //    queryString["language"] = "unk";
                //    queryString["detectOrientation"] = "true";
                //    var uri = "https://westus.api.cognitive.microsoft.com/vision/v2.0/ocr?" + queryString;

                //    HttpResponseMessage response;

                //    // Request body
                //    byte[] byteData = Encoding.UTF8.GetBytes("{body}");

                //    using (var content = new ByteArrayContent(byteData))
                //    {
                //        content.Headers.ContentType = new MediaTypeHeaderValue("< your content type, i.e. application/json >");
                //        response = await client.PostAsync(uri, content);
                //    }

                
                // var operation = await _visionClient.BatchReadFileInStreamWithHttpMessagesAsync(ms);
                // var operationLocation = operation.Headers.OperationLocation;

                // Retrieve the URI where the recognized text will be
                // stored from the Operation-Location header
                // string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

                // var result = await _visionClient.GetReadOperationResultWithHttpMessagesAsync(operationId);

                // Wait for the operation to complete

                //int i = 0;
                // while ((result.Body.Status == TextOperationStatusCodes.Running ||
                //         result.Body.Status == TextOperationStatusCodes.NotStarted) && i++ < MaxRetries)
                // {
                //     Console.WriteLine("Server status: {0}, waiting {1} seconds...", result.Body.Status, i);
                //     await Task.Delay(Convert.ToInt32(ConfigurationManager.AppSettings["ComputerVisionDelay"]));

                 //   result = await _visionClient.GetReadOperationResultWithHttpMessagesAsync(operationId);
                // }

                // return result.Body.RecognitionResults.SelectMany(rs => rs.Lines).ToList();
            
            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }
}

