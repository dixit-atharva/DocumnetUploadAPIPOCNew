using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MultiEsignDLL;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Xml;

namespace ProteanWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ESignController : ControllerBase
    {
        public ESignController()
        {
            string BasePath = AppDomain.CurrentDomain.BaseDirectory;
        }

        [HttpPost("request")]
        public IActionResult EsignRequestDocumentAsync()
        {
            try
            {
                string BasePath = AppDomain.CurrentDomain.BaseDirectory;
                string ContentPath = Path.Combine(BasePath, "Content");

                string pdfPath = Path.Combine(ContentPath, "PDF", "test.pdf"); //PDF File path which needs to sign
                string jarPath = Path.Combine(ContentPath, "Cert", "v1.4_RunnableMultiO.jar");
                string ekycId = "";// "9115915849419678";// Aadhar number token /UID ID 72 digit its optional field
                string aspId = "ASPSSPLUAT008151";

                string authMode = "1";
                string resp_url = "http://localhost:5034/api/eSign/Protean-CallBack";
                string certificatePath = Path.Combine(ContentPath, "Cert", "SignCare_SelfSign_Cert.p12"); // ASP private cretificate [ie .p12] full path
                string certificatePassward = "signcare"; //ASP private cretificate[ie.p12] password
                string tickImagePath = Path.Combine(ContentPath, "Cert", "tick.png"); //Tick Image for signature symbol
                int serverTime = 15;
                string alias = "signcare";
                string nameToShowOnSignatureStamp = "SignCare";
                string locationToShowOnSignatureStamp = "Ahmedabad";
                string reasonForSign = "Digitally Signed by SignCare Solutions";
                string pdfPassword = "";
                string txn = Guid.NewGuid().ToString();
                int log_err = 1;

                //string CoordinatesList = "";
                string CoordinatesPath = Path.Combine(ContentPath, "Cert", "Coordinates.txt");

                //using (StreamReader sr = new StreamReader(CoordinatesPath))
                //{
                //    CoordinatesList = sr.ReadToEnd();
                //}

                // MultipleEsign req_resp = new MultipleEsign();
                // dll

                Esign req_resp = new Esign();

                string req = req_resp.CreateRequestXml(jarPath, ekycId, pdfPath, aspId, authMode, resp_url, certificatePath, certificatePassward, tickImagePath, serverTime, alias, nameToShowOnSignatureStamp, locationToShowOnSignatureStamp, reasonForSign, pdfPassword, txn, CoordinatesPath, log_err);

                //dll ref call
                // MultipleEsign req = new MultipleEsign();
                string base_folder_path = System.IO.Path.GetDirectoryName(pdfPath)!;
                string file_withoutExtn = Path.GetFileNameWithoutExtension(pdfPath);
                string request = file_withoutExtn + "_eSignRequestXml.txt";

                while (!System.IO.File.Exists(Path.Combine(base_folder_path, request)))
                {
                    System.Threading.Thread.Sleep(1000);
                }

                // Request xml generated successfully.

                string xml_get = null;
                using (StreamReader sr = new StreamReader(Path.Combine(base_folder_path, request)))
                {
                    xml_get = sr.ReadToEnd();
                }

                // Request XML send to generate Signed XML
                NameValueCollection collections = new NameValueCollection();
                collections.Add("msg", xml_get);
                string remoteUrl = "https://pregw.esign.egov-nsdl.com/nsdl-esp/authenticate/esign-doc/";  //URL for eSign 2.1 Web

                string html = "<html><head>";
                html += "</head><body onload='document.forms[0].submit()'>";
                html += string.Format("<form name='PostForm' method='POST' action='{0}' enctype='multipart/form-data'>", remoteUrl);
                foreach (string key in collections.Keys)
                {
                    //html += string.Format("<input name='{0}' type='text' value='{1}'>", key, collections[key]);
                    html += string.Format("<input name='{0}' type='hidden' value='{1}'>", key, collections[key]);
                }
                html += "<input type=\"Submit\" value=\"Submit\" id =\"countButton\"/>";
                html += "</form></body></html>";
                return Content(html, "text/html");
                //Response.ContentEncoding = Encoding.GetEncoding("ISO-8859-1");
                //Response.HeaderEncoding = Encoding.GetEncoding("ISO-8859-1");
                //Response.Charset = "ISO-8859-1";
                //Response.Write(html);
                //Response.End();
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }

            return Ok();
        }

        [HttpPost("Protean-CallBack")]
        public async Task<IActionResult> EsignRedirectDocument()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var requestBody = await reader.ReadToEndAsync();

                // Step 1: Extract XML content from multipart data
                string responseXml = ExtractXmlContent(requestBody);

                string BasePath = AppDomain.CurrentDomain.BaseDirectory;
                string ContentPath = Path.Combine(BasePath, "Content");

                string pdfPath = Path.Combine(ContentPath, "PDF", "test.pdf"); //PDF File path which needs to sign
                string jarPath = Path.Combine(ContentPath, "Cert", "v1.4_RunnableMultiO.jar");
                string tickImagePath = Path.Combine(ContentPath, "Cert", "tick.png"); //Tick Image for signature symbol
                int serverTime = 15;
                string nameToShowOnSignatureStamp = "SignCare";
                string locationToShowOnSignatureStamp = "Ahmedabad";
                string reasonForSign = "Digitally Signed by SignCare Solutions";
                string pdfPassword = "";

                int log_err = 0;

                string pdfFolder = Path.GetDirectoryName(pdfPath)!;
                string PdfName = Path.GetFileNameWithoutExtension(pdfPath);
                string CoordinatesPath = Path.Combine(ContentPath, "Cert", "Coordinates.txt");
                string outputFinalPdfPath = Path.Combine(ContentPath, "PDF");

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
                XmlElement EsignResponse = xmlDoc.DocumentElement!;
                //RequestResponse response = new RequestResponse();
                // dll call 
                Esign response = new Esign();
                if (EsignResponse.Attributes != null && EsignResponse.Attributes["status"]!.Value != "1")
                {
                    response.WriteLog("errCode: " + EsignResponse.Attributes["errCode"]!.Value + " & Error Message: " + EsignResponse.Attributes["errMsg"]!.Value, 1, pdfFolder);
                }

                string rtn = response.stampSignOnDocument(pdfPath, jarPath, tickImagePath, responsexmlPath, serverTime, nameToShowOnSignatureStamp, locationToShowOnSignatureStamp, reasonForSign, pdfPassword, outputFinalPdfPath, CoordinatesPath, log_err);
                string signedPdfPath = "";
                if (outputFinalPdfPath == "")
                    signedPdfPath = pdfFolder + "\\" + PdfName + "_signedFinal.pdf";
                else
                    signedPdfPath = outputFinalPdfPath + "\\" + PdfName + "_signedFinal.pdf";
                while (!System.IO.File.Exists(signedPdfPath))
                {
                    System.Threading.Thread.Sleep(1000);
                }

                return Ok("Signed Successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Error = ex.Message });
            }
        }

        private string ExtractXmlContent(string multipartData)
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
    }
}
