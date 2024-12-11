using Newtonsoft.Json;
using System.Text;
using System.Xml;
using VerasysWebAPI.Dtos;

namespace VerasysWebAPI.Services
{
    public class EsignResponseHandler
    {
        string mainPath = "D:\\Project\\SignCare\\DocumnetUploadAPIPOCNew\\VerasysWebAPI\\Files\\";
        public async void HandleEsignResponse(string responseXml)
        {
            XmlDocument responseDoc = new XmlDocument();
            responseDoc.LoadXml(responseXml);

            string status = responseDoc.SelectSingleNode("/EsignResp/@status")?.Value;

            if (status == "1") // Success status code
            {
                string encodedXml = Base64Encode(responseXml);
                await AppendSignature(encodedXml);
            }
            else
            {
                string errCode = responseDoc.SelectSingleNode("/EsignResp/@errCode")?.Value;
                string errMsg = responseDoc.SelectSingleNode("/EsignResp/@errMsg")?.Value;

                Console.WriteLine($"Error Code: {errCode}");
                Console.WriteLine($"Error Message: {errMsg}");
            }
        }

        private string Base64Encode(string input)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(byteArray);
        }

        async Task AppendSignature(string responseXML)
        {
            var apiUrl = "http://127.0.0.1:7077/signpdfv4";

            // Creating the request object
            var apiRequest = new AppendSignRequest
            {
                tempInfoPath = mainPath,
                signedFileParentPath = mainPath,
                responseXML = responseXML,
                pdfDestinationPath = $"{mainPath}ekyc-Agreement_signed.pdf",
                tickImgPath = $"{mainPath}tick.png",
                aspLogo = "https://stage.signcare.io/assets/img/brand-logo-email.png",
                signatureFontSize = "10"
            };

            // Serializing the request object to JSON
            var jsonContent = JsonConvert.SerializeObject(apiRequest);

            // Sending the request using HttpClient
            using (var client = new HttpClient())
            {
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Send POST request
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Request successful!");
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response: " + responseContent);
                }
                else
                {
                    Console.WriteLine("Request failed with status: " + response.StatusCode);
                }
            }
        }
    }
}
