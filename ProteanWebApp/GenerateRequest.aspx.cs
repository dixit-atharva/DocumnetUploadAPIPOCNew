using MultiEsignDLL;
using Pkcs7pdf_Multiple_EsignService;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace ProteanWebApp
{
    public partial class GenerateRequest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string MainPath = @"D:\Rahul_Workplace\Code-Sample\DocumnetUploadAPIPOCNew\ProteanWebApp\Esign\";
            string pdfPath = $"{MainPath}test.pdf"; //PDF File path which needs to sign
            string jarPath = $"{MainPath}v1.4_RunnableMultiO.jar";
            string ekycId = "";// "9115915849419678";// Aadhar number token /UID ID 72 digit its optional field
            string aspId = "NSDLeGOVTest002"; //"ASPNIIPLMUMTEST132";  ; //"ASPMOSLMUMTEST133";

            string authMode = "1";
            string resp_url = "http://localhost:51180/EsignResponse.aspx";// response URL after esign process ; //
            string certificatePath = $"{MainPath}SignCare.p12"; // ASP private cretificate [ie .p12] full path
            string certificatePassward = "123456"; //ASP private cretificate[ie.p12] password
            string tickImagePath = $"{MainPath}tick.jpg"; //Tick Image for signature symbol
            int serverTime = 15;
            string alias = "1";
            string nameToShowOnSignatureStamp = "SignCare";
            string locationToShowOnSignatureStamp = "Ahmedabad";
            string reasonForSign = "Digitally Signed by SignCare Solutions";
            string pdfPassword = "";
            string txn = "";
            int log_err = 1;

            string CoordinatesList = "";
            string CoordinatesPath = $"{MainPath}Coordinates.txt";

            using (StreamReader sr = new StreamReader(CoordinatesPath))
            {
                CoordinatesList = sr.ReadToEnd();
            }

            // 1 : Release_eSign_2_1_MultiSign_DotNET
            // 2 : Release_eSign_2.1_MultiSign_DotNET_pkcs7
            // 3: Release_eSign_2.1_MultiSign_DotNET_pkcs7_login
            int defaultMode = 1;

            switch (defaultMode)
            {
                case 1:
                    Release_eSign_2_1_MultiSign_DotNET(pdfPath, jarPath, ekycId, aspId, authMode, resp_url, certificatePath, certificatePassward, tickImagePath, serverTime, alias, nameToShowOnSignatureStamp, locationToShowOnSignatureStamp, reasonForSign, pdfPassword, txn, log_err, CoordinatesPath);
                    break;
                case 2:
                    Release_eSign_2_1_MultiSign_DotNET_pkcs7(pdfPath, jarPath, ekycId, aspId, authMode, resp_url, certificatePath, certificatePassward, tickImagePath, serverTime, alias, nameToShowOnSignatureStamp, locationToShowOnSignatureStamp, reasonForSign, pdfPassword, txn, log_err, CoordinatesPath);
                    break;
                case 3:
                    Release_eSign_2_1_MultiSign_DotNET_pkcs7_login(pdfPath, jarPath, tickImagePath, serverTime, nameToShowOnSignatureStamp, locationToShowOnSignatureStamp, reasonForSign, pdfPassword, log_err, CoordinatesPath);
                    break;
            }


        }

        private void Release_eSign_2_1_MultiSign_DotNET_pkcs7_login(string pdfPath, string jarPath, string tickImagePath, int serverTime, string nameToShowOnSignatureStamp, string locationToShowOnSignatureStamp, string reasonForSign, string pdfPassword, int log_err, string Coordinates)
        {
            string jrebinpath = @"C:\Program Files\Java\jre1.8.0_431\bin";
            // if user need to save other location then he has to specify folder path;
            string outputFinalPdfPath = "";
            string pdfFolder = Path.GetDirectoryName(pdfPath);
            string PdfName = Path.GetFileNameWithoutExtension(pdfPath);
            try
            {
                // get ResonseXml 
                string responseXml = Request["msg"].ToString();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(responseXml);
                XmlElement EsignResponse = xmlDoc.DocumentElement;
                PKCS7PDFMultiEsign response = new PKCS7PDFMultiEsign();
                if (EsignResponse.Attributes != null && EsignResponse.Attributes["status"].Value != "1")
                {
                    response.WriteLog("errCode: " + EsignResponse.Attributes["errCode"].Value + " & Error Message: " + EsignResponse.Attributes["errMsg"].Value, 1, pdfFolder);
                }
                else
                {
                    string responsexmlPath = pdfFolder + "\\" + "ResponseXml.txt";
                    File.WriteAllText(responsexmlPath, responseXml);
                    string rtn = response.SignDocument(pdfPath, jarPath, tickImagePath, responsexmlPath, serverTime, nameToShowOnSignatureStamp, locationToShowOnSignatureStamp, reasonForSign, pdfPassword, outputFinalPdfPath, Coordinates, jrebinpath, log_err);
                }
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
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
        }

        private void Release_eSign_2_1_MultiSign_DotNET_pkcs7(string pdfPath, string jarPath, string ekycId, string aspId, string authMode, string resp_url, string certificatePath, string certificatePassward, string tickImagePath, int serverTime, string alias, string nameToShowOnSignatureStamp, string locationToShowOnSignatureStamp, string reasonForSign, string pdfPassword, string txn, int log_err, string CoordinatesPath)
        {
            try
            {
                string jrebinpath = @"C:\Program Files\Java\jre1.8.0_431\bin";
                string responsesigtype = "";
                PKCS7PDFMultiEsign req_resp = new PKCS7PDFMultiEsign();
                string req = req_resp.GenerateRequestXml(jarPath, ekycId, pdfPath, aspId, authMode, resp_url, certificatePath, certificatePassward, tickImagePath, serverTime, alias, nameToShowOnSignatureStamp, locationToShowOnSignatureStamp, reasonForSign, pdfPassword, txn, responsesigtype, CoordinatesPath, jrebinpath, log_err);
                string base_folder_path = Path.GetDirectoryName(pdfPath);
                string file_withoutExtn = Path.GetFileNameWithoutExtension(pdfPath);
                string request = file_withoutExtn + "_eSignRequestXml.txt";
                while (!File.Exists(base_folder_path + "\\" + request))
                {
                    System.Threading.Thread.Sleep(1000);
                }
                string xml_get = null;
                using (StreamReader sr = new StreamReader(base_folder_path + "\\" + request))
                {
                    xml_get = sr.ReadToEnd();
                }
                NameValueCollection collections = new NameValueCollection();
                collections.Add("msg", xml_get);
                string remoteUrl = "https://pregw.esign.egov-nsdl.com/nsdl-esp/authenticate/esign-doc/";
                string html = "<html><head>";
                html += "</head><body onload='document.forms[0].submit()'>";
                html += string.Format("<form name='PostForm' method='POST' action='{0}' enctype='multipart/form-data'>", remoteUrl);
                foreach (string key in collections.Keys)
                {
                    html += string.Format("<input name='{0}' type='text' value='{1}'>", key, collections[key]);
                }
                html += "</form></body></html>";
                Response.Clear();
                Response.ContentEncoding = Encoding.GetEncoding("ISO-8859-1");
                Response.HeaderEncoding = Encoding.GetEncoding("ISO-8859-1");
                Response.Charset = "ISO-8859-1";
                Response.Write(html);
                Response.End();
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
        }

        private void Release_eSign_2_1_MultiSign_DotNET(string pdfPath, string jarPath, string ekycId, string aspId, string authMode, string resp_url, string certificatePath, string certificatePassward, string tickImagePath, int serverTime, string alias, string nameToShowOnSignatureStamp, string locationToShowOnSignatureStamp, string reasonForSign, string pdfPassword, string txn, int log_err, string CoordinatesPath)
        {
            #region Release_eSign_2.1_MultiSign_DotNET
            // MultipleEsign req_resp = new MultipleEsign();
            // dll

            Esign req_resp = new Esign();

            string req = req_resp.CreateRequestXml(jarPath, ekycId, pdfPath, aspId, authMode, resp_url, certificatePath, certificatePassward, tickImagePath, serverTime, alias, nameToShowOnSignatureStamp, locationToShowOnSignatureStamp, reasonForSign, pdfPassword, txn, CoordinatesPath, log_err);

            //dll ref call
            // MultipleEsign req = new MultipleEsign();
            string base_folder_path = System.IO.Path.GetDirectoryName(pdfPath);
            string file_withoutExtn = Path.GetFileNameWithoutExtension(pdfPath);
            string request = file_withoutExtn + "_eSignRequestXml.txt";

            while (!File.Exists(base_folder_path + "\\" + request))
            {
                System.Threading.Thread.Sleep(1000);
            }

            // Request xml generated successfully.

            string xml_get = null;
            using (StreamReader sr = new StreamReader(base_folder_path + "\\" + request))
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
                html += string.Format("<input name='{0}' type='text' value='{1}'>", key, collections[key]);
            }
            html += "</form></body></html>";
            Response.Clear();
            Response.ContentEncoding = Encoding.GetEncoding("ISO-8859-1");
            Response.HeaderEncoding = Encoding.GetEncoding("ISO-8859-1");
            Response.Charset = "ISO-8859-1";
            Response.Write(html);
            Response.End();
            #endregion
        }
    }
}