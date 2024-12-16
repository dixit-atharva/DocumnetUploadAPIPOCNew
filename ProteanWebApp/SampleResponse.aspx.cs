using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Xml;
using MultiEsignDLL;

namespace ProteanWebApp
{
    public partial class SampleResponse : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string MainPath = @"D:\Rahul_Workplace\Code-Sample\DocumnetUploadAPIPOCNew\ProteanWebApp\Esign\";
            string multipartResponse = File.ReadAllText(Path.Combine(MainPath, "Protean_XML_response.txt"));


            // Step 1: Extract XML content from multipart data
            string extractedXml = ExtractXmlContent(multipartResponse);

            // Step 2: Parse the XML content
            if (!string.IsNullOrEmpty(extractedXml))
            {
                //ParseXmlContent(extractedXml);
                SignDoc(extractedXml);
            }
            else
            {
                Console.WriteLine("No valid XML content found.");
            }
        }

        string ExtractXmlContent(string multipartData)
        {
            // Find the start and end of the XML content
            string startBoundary = "\r\n<?xml";
            string endBoundary = "</EsignResp>";

            int startIndex = multipartData.IndexOf(startBoundary);
            int endIndex = multipartData.IndexOf(endBoundary);

            if (startIndex != -1 && endIndex != -1)
            {
                startIndex += 2; // Adjust for line breaks
                endIndex += endBoundary.Length;
                string extractedData = multipartData.Substring(startIndex, endIndex - startIndex);
                return extractedData;
            }

            return string.Empty;
        }

        static void ParseXmlContent(string xmlContent)
        {
            try
            {
                // Load the XML
                XDocument xmlDoc = XDocument.Parse(xmlContent);

                // Extract attributes from EsignResp
                XElement esignResp = xmlDoc.Root;
                if (esignResp != null)
                {
                    string errCode = esignResp.Attribute("errCode")?.Value;
                    string errMsg = esignResp.Attribute("errMsg")?.Value;
                    string resCode = esignResp.Attribute("resCode")?.Value;
                    string status = esignResp.Attribute("status")?.Value;
                    string txn = esignResp.Attribute("txn")?.Value;

                    Console.WriteLine($"Error Code: {errCode}");
                    Console.WriteLine($"Error Message: {errMsg}");
                    Console.WriteLine($"Response Code: {resCode}");
                    Console.WriteLine($"Status: {status}");
                    Console.WriteLine($"Transaction: {txn}");

                    // Extract UserX509Certificate
                    string userCert = esignResp.Element("UserX509Certificate")?.Value;
                    Console.WriteLine($"User Certificate: {userCert}");

                    // Extract DocSignature details
                    foreach (XElement signature in esignResp.Descendants("DocSignature"))
                    {
                        string id = signature.Attribute("id")?.Value;
                        string sigHashAlgorithm = signature.Attribute("sigHashAlgorithm")?.Value;
                        string docSignature = signature.Value;

                        Console.WriteLine($"Doc Signature ID: {id}");
                        Console.WriteLine($"Signature Hash Algorithm: {sigHashAlgorithm}");
                        Console.WriteLine($"Doc Signature: {docSignature}");
                    }
                }
            }
            catch (XmlException ex)
            {
                Console.WriteLine($"XML Parsing Error: {ex.Message}");
            }
        }

        static void SignDoc(string responseXml)
        {
            try
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
            catch (XmlException ex)
            {
                Console.WriteLine($"XML Parsing Error: {ex.Message}");
            }
        }
    }
}