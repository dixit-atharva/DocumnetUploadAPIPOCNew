using MultiEsignDLL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace ProteanWebApp
{
    public partial class EsignResponse : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if the request is a POST
            if (Request.HttpMethod == "POST")
            {
                // Read the raw input stream
                string postData;
                using (StreamReader reader = new StreamReader(Request.InputStream))
                {
                    // Reset the stream position before reading
                    Request.InputStream.Position = 0;
                    postData = reader.ReadToEnd();
                }

                // Log or process the received data
                // Example: Save data to a file (for debugging purposes)
                string logFilePath = Server.MapPath("~/App_Data/CallbackLog.txt");
                File.AppendAllText(logFilePath, $"[{DateTime.Now}] Received: {postData}\n");

                // Send a response back to the Protean callback
                Response.ContentType = "text/plain";
                Response.Write("Callback received successfully.");
            }
            else
            {
                // Handle non-POST requests if necessary
                Response.ContentType = "text/plain";
                Response.Write("Invalid request method.");
            }

            string requestBody;
            using (var reader = new StreamReader(Request.InputStream))
            {
                // Move the position of the stream to the beginning
                HttpContext.Current.Request.InputStream.Position = 0;
                requestBody = reader.ReadToEnd();
            }

            // ResonseXml digest
            string responseXml = Request["msg"].ToString();

            string MainPath = @"D:\Rahul_Workplace\Code-Sample\DocumnetUploadAPIPOCNew\ProteanWebApp\Esign\";
            string pdfPath = $"{MainPath}test.pdf"; //PDF File path which needs to sign
            string jarPath = $"{MainPath}v1.4_RunnableMultiO.jar";
            string tickImagePath = $"{MainPath}tick.png"; //Tick Image for signature symbol
            int serverTime = 15;
            string nameToShowOnSignatureStamp = "SignCare";
            string locationToShowOnSignatureStamp = "Ahmedabad";
            string reasonForSign = "Digitally Signed by SignCare Solutions";
            string pdfPassword = "";

            int log_err = 0;

            string pdfFolder = Path.GetDirectoryName(pdfPath);
            string PdfName = Path.GetFileNameWithoutExtension(pdfPath);
            string Coordinates = $"{MainPath}Coordinates.txt";
            // if user need to save other location then he has to specify folder path;
            string outputFinalPdfPath = MainPath;

            string responsexmlPath = pdfFolder + "\\" + "ResponseXml.txt";
  
            //string responseXml = System.IO.File.ReadAllText($"{MainPath}test_eSignRequestXml.txt");
            // write response to txt file
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(responsexmlPath, false))
            {
                writer.WriteLine(responseXml);
                writer.Close();
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(responseXml);
            XmlElement EsignResponse = xmlDoc.DocumentElement;
            //RequestResponse response = new RequestResponse();
            // dll call 
            Esign response = new Esign();
            if (EsignResponse.Attributes != null && EsignResponse.Attributes["status"].Value != "1")
            {
                response.WriteLog("errCode: " + EsignResponse.Attributes["errCode"].Value + " & Error Message: " + EsignResponse.Attributes["errMsg"].Value, 1, pdfFolder);
            }

            string rtn = response.stampSignOnDocument(pdfPath, jarPath, tickImagePath, responsexmlPath, serverTime, nameToShowOnSignatureStamp, locationToShowOnSignatureStamp, reasonForSign, pdfPassword, outputFinalPdfPath, Coordinates, log_err);
            string signedPdfPath = "";
            if (outputFinalPdfPath == "")
                signedPdfPath = pdfFolder + "\\" + PdfName + "_signedFinal.pdf";
            else
                signedPdfPath = outputFinalPdfPath + "\\" + PdfName + "_signedFinal.pdf";
            while (!File.Exists(signedPdfPath))
            {
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}