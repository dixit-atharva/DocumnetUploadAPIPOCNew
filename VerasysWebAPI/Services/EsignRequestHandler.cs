using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Web;
using VerasysWebAPI.Dtos;

namespace VerasysWebAPI.Services
{
    public class EsignRequestHandler
    {
        string mainPath = "D:\\Project\\SignCare\\DocumnetUploadAPIPOCNew\\VerasysWebAPI\\Files\\";

        public async Task HandleEsignRequest()
        {
            // Create the request object
            var request = new SignRequest
            {
                signedPdfPath = mainPath,
                ver = "21",
                tempInfoPath = mainPath,
                pfxAlias = "{05ae2e10-4f6d-41a6-9f83-4d0025ca28a0}",
                isresponseXML = "0",
                txn = RandomTxn(),
                pfxPath = $"{mainPath}Class II Organization 2 Year Document Signer Signature-2024 (2).pfx",
                signerName = "Himang",
                pdfDestinationPath = $"{mainPath}ekyc-Agreement_signed.pdf",
                responseUrl = "http://localhost:5244/aspesignresponse",
                pfxPassword = "abc1234",
                aspId = "SSPLUAT001",
                signingAlgorithm = "RSA",
                maxWaitPeriod = "1440",
                pdfdetails = new List<PdfDetail>
                {
                    new PdfDetail
                    {
                        reason = "Demo Test eSign by Vsign",
                        signaturedetails = new List<SignatureDetail>
                        {
                            new SignatureDetail
                            {
                                coordinates = new List<PdfCoordinate>
                                {
                                    new PdfCoordinate { w = 180, x = 20, h = 80, y = 20 }
                                },
                                page = "1"
                            }
                        },
                        docUrl = "",
                        docInfo = "ekyc-Subscriber-Agreement.pdf",
                        pdfbase64val = $"{mainPath}signcare-test.pdf"
                    }
                },
                isrequestXML = "0",
                AuthMode = 1,
                fileType = "path",
                tickImgPath = $"{mainPath}tick.png",
                aspLogo = "https://stage.signcare.io/assets/img/brand-logo-email.png",
                signatureFontSize = "10"
            };

            // Serialize the request object to JSON
            var jsonData = JsonConvert.SerializeObject(request);

            // The URL of the API endpoint
            var apiUrl = "http://127.0.0.1:7077/gettxnrefv4";

            using (var client = new HttpClient())
            {
                // Set the content for the request
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Send the POST request asynchronously
                var response = await client.PostAsync(apiUrl, content);

                // Read and display the response
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response: " + responseData);
                    var responseObject = JsonConvert.DeserializeObject<EsignResponse>(responseData);

                    // Fetch the txnref value
                    var txnref = responseObject?.txnref;

                    // Output the txnref
                    Console.WriteLine("txnref: " + txnref);

                    // Generate the HTML content
                    var htmlContent = $@"
                            <!DOCTYPE html>
                            <html lang='en'>
                            <head>
                                <meta charset='UTF-8'>
                                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                                <title>Auto Populate and Submit</title>
                            </head>
                            <body>
                                <form id='authPageForm' method='post' action='https://esignuat.vsign.in/esp/authpage'>
                                    <label for='txnref'>TxnRef:</label>
                                    <input type='text' id='txnref' name='txnref' value='{HttpUtility.HtmlEncode(txnref)}'>
                                    <br><br>
                                    <input type='submit' value='Submit'>
                                </form>
                            <script>
                                document.addEventListener('DOMContentLoaded', () => {{
                                    document.getElementById('authPageForm').submit();
                                }});
                            </script>
                            </body>
                            </html>";

                    // Save the HTML file
                    var htmlFilePath = $"{mainPath}SamplePage.html";
                    File.WriteAllText(htmlFilePath, htmlContent);

                    // Open the HTML file in the default browser
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = htmlFilePath,
                        UseShellExecute = true
                    });
                }
                else
                {
                    Console.WriteLine("Error: " + response.StatusCode);
                }
            }
        }

        private string RandomTxn()
        {
            string prefix = "SIGNCARE";
            string dateTimePart = DateTime.UtcNow.ToString("yyyyMMddHHmmssffff"); // UTC timestamp with milliseconds
            Random random = new Random();
            string randomNumber = string.Concat(Enumerable.Range(0, 10).Select(_ => random.Next(0, 10).ToString()));
            string txn = $"{prefix}{dateTimePart}{randomNumber}";
            return txn;
        }
    }
}
