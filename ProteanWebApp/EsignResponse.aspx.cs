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
            string Coordinates = @"D:\MultipleEsignTest\Coordinates.txt";
            // if user need to save other location then he has to specify folder path;
            string outputFinalPdfPath = @"D:";

            string responsexmlPath = pdfFolder + "\\" + "ResponseXml.txt";
            // ResonseXml digest
            string responseXml = Request["msg"].ToString();
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