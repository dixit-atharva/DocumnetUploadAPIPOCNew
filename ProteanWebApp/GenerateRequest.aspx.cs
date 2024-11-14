using MultiEsignDLL;
using Pkcs7pdf_Multiple_EsignService;
using PKCS7PDF_Signature;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading;
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
            string resp_url = "http://localhost:51180/EsignResponse.aspx";// response URL after esign process ; //http://localhost:51180/EsignResponse.aspx
            string certificatePath = $"{MainPath}rahul_cert.p12"; // ASP private cretificate [ie .p12] full path
            string certificatePassward = "123456"; //ASP private cretificate[ie.p12] password
            string tickImagePath = $"{MainPath}tick.png"; //Tick Image for signature symbol
            int serverTime = 15;
            string alias = "rahulpatel";
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
            // 4 : eSign_2_1_DotNET_Utility_new
            int defaultMode = 1;

            switch (defaultMode)
            {
                case 1:
                    Release_eSign_2_1_MultiSign_DotNET(pdfPath, jarPath, ekycId, aspId, authMode, resp_url, certificatePath, certificatePassward, tickImagePath, serverTime, alias, nameToShowOnSignatureStamp, locationToShowOnSignatureStamp, reasonForSign, pdfPassword, txn, log_err, CoordinatesPath);
                    break;

                case 11:
                    Release_eSign_2_1_MultiSign_DotNET11(pdfPath, jarPath, ekycId, aspId, authMode, resp_url, certificatePath, certificatePassward, tickImagePath, serverTime, alias, nameToShowOnSignatureStamp, locationToShowOnSignatureStamp, reasonForSign, pdfPassword, txn, log_err, CoordinatesPath);
                    break;

                case 2:
                    Release_eSign_2_1_MultiSign_DotNET_pkcs7(MainPath, pdfPath, jarPath, ekycId, aspId, authMode, resp_url, certificatePath, certificatePassward, tickImagePath, serverTime, alias, nameToShowOnSignatureStamp, locationToShowOnSignatureStamp, reasonForSign, pdfPassword, txn, log_err, CoordinatesPath);
                    break;
                case 3:
                    Release_eSign_2_1_MultiSign_DotNET_pkcs7_login(pdfPath, jarPath, tickImagePath, serverTime, nameToShowOnSignatureStamp, locationToShowOnSignatureStamp, reasonForSign, pdfPassword, log_err, CoordinatesPath);
                    break;
                case 4:
                    eSign_2_1_DotNET_Utility_new(MainPath, pdfPath, ekycId, aspId, authMode, resp_url, certificatePath, certificatePassward, nameToShowOnSignatureStamp, locationToShowOnSignatureStamp
                        , tickImagePath, serverTime, alias, reasonForSign, pdfPassword, txn);
                    break;
            }


        }

        private void eSign_2_1_DotNET_Utility_new(string MainPath, string Pdfpath, string ekycId, string aspId, string authMode, string resp_url
            , string certificatePath, string certificatePassward, string nameToShowOnSignatureStamp, string locationOnSignature
            , string tickImagePath, int serverTime, string alias, string reasonForSign, string pdfPassword, string txn)
        {
            // jdk or jre bin folder path for 64 bit(C:\Program Files\Java) and for 32 bit(C:\Program Files (x86)\Java)system
            string Jrebinfolderpath = @"C:\Program Files\Java\jre1.8.0_181\bin";
            string ResponseSignaturetype = "pkcs7pdf";
            string Jarpath = $"{MainPath}Runnable2_eSign2.1_Single.jar";
            try
            {
                Pkcs7pdfEsign req_resp = new Pkcs7pdfEsign();
                string ret = req_resp.CreateRequestXml(Pdfpath, Jarpath, ekycId, aspId, authMode, resp_url, certificatePath, certificatePassward
                    , tickImagePath, "100", "100", serverTime, alias, 1, nameToShowOnSignatureStamp, locationOnSignature, reasonForSign, 100, 100, pdfPassword, txn, ResponseSignaturetype, Jrebinfolderpath, 1);
                string base_folder_path = Path.GetDirectoryName(Pdfpath);
                string file_withoutExtn = Path.GetFileNameWithoutExtension(Pdfpath);
                string req_flnm = file_withoutExtn + "_eSignRequestXml.txt";
                while (!File.Exists(base_folder_path + "\\" + req_flnm))
                {
                    System.Threading.Thread.Sleep(1000);
                }

                string xml_get = null;
                using (StreamReader sr = new StreamReader(base_folder_path + "\\" + req_flnm))
                {
                    xml_get = sr.ReadToEnd();
                }
                // post Request
                NameValueCollection collections = new NameValueCollection();
                collections.Add("msg", xml_get);
                //UAT url
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

        private void Release_eSign_2_1_MultiSign_DotNET_pkcs7(string MainPath, string pdfPath, string jarPath, string ekycId, string aspId, string authMode, string resp_url, string certificatePath, string certificatePassward, string tickImagePath, int serverTime, string alias, string nameToShowOnSignatureStamp, string locationToShowOnSignatureStamp, string reasonForSign, string pdfPassword, string txn, int log_err, string CoordinatesPath)
        {
            try
            {
                string jrebinpath = @"C:\Program Files\Java\jre1.8.0_431\bin";
                jarPath = $"{MainPath}Runnable2_eSign2.1_Multiple.jar";
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
                //html += string.Format("<input name='{0}' type='text' value='{1}'>", key, collections[key]);
                html += string.Format("<input name='{0}' type='hidden' value='{1}'>", key, collections[key]);
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

        private void Release_eSign_2_1_MultiSign_DotNET11(string inpt_PDF_path, string jar_path, string ekycId, string asp_id,
            string authMode, string resp_url, string cert, string passward,
            string img_path, int serverTime, string alias, string nameToShowOnSignatureStamp,
            string locationToShowOnSignatureStamp, string reasonForSign, string pdfPassword, string Txn, int log_err, string Coordinates)
        {
            #region Release_eSign_2.1_MultiSign_DotNET
            // MultipleEsign req_resp = new MultipleEsign();
            // dll

            string text = "\"" + inpt_PDF_path + "\"";
            string text2 = "\"" + ekycId + "\"";
            string text3 = "\"" + authMode + "\"";
            string text4 = "\"" + jar_path + "\"";
            string text5 = "\"" + asp_id + "\"";
            string text6 = "\"" + cert + "\"";
            string text7 = "\"" + passward + "\"";
            string text8 = "\"" + alias + "\"";
            string text9 = "\"" + resp_url + "\"";
            string text10 = "\"" + img_path + "\"";
            string text11 = "\"" + nameToShowOnSignatureStamp + "\"";
            string text12 = "\"" + locationToShowOnSignatureStamp + "\"";
            string text13 = "\"" + reasonForSign + "\"";
            string text14 = "\"" + pdfPassword + "\"";
            string text15 = "\"" + Txn + "\"";
            string text16 = "\"" + Coordinates + "\"";
            string directoryName = Path.GetDirectoryName(inpt_PDF_path);
            if (log_err == 1)
            {
                WriteLog("CreatingRequestXml Process Start...", log_err, directoryName);
            }

            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo("java.exe", $" -jar {text4} 1 {text2} {text} {text5} {text3} {text9} {text6} {text7} {text10} {serverTime} {text8} {text11} {text12} {text13} {text14} {text15} {text16}");
                processStartInfo.CreateNoWindow = true;
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processStartInfo.UseShellExecute = false;
                Process process = Process.Start(processStartInfo);
                process.WaitForExit(5000);
                if (log_err == 1)
                {
                    string errorMessage = jar_path + " @ " + ekycId + " @ " + inpt_PDF_path + " @ " + asp_id + " @ " + authMode + " @ " + resp_url + " @ " + cert + " @ " + passward + " @ " + img_path + " @ " + serverTime + " @ " + alias + " @ " + nameToShowOnSignatureStamp + " @ " + locationToShowOnSignatureStamp + " @ " + reasonForSign + " @ " + pdfPassword + " @ " + Txn + "@" + Coordinates;
                    WriteLog(errorMessage, log_err, directoryName);
                }

                int num = 0;
                while (!process.HasExited)
                {
                    Thread.Sleep(1000);
                    if (num > 6)
                    {
                        process.Close();
                        break;
                    }

                    num++;
                }

                process.Close();
                if (log_err == 1)
                {
                    WriteLog("Creating Requestxml process end", log_err, directoryName);
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message.ToString(), log_err, directoryName);
            }

            string req = "CreateRequestXml() Completed...";

            //dll ref call
            // MultipleEsign req = new MultipleEsign();
            string base_folder_path = System.IO.Path.GetDirectoryName(inpt_PDF_path);
            string file_withoutExtn = Path.GetFileNameWithoutExtension(inpt_PDF_path);
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

        public void WriteLog(string errorMessage, int log, string logFilePath)
        {
            try
            {
                string path = null;
                string text = null;
                string text2 = DateTime.Now.ToString("dd_MM_yyyy");
                if (log == 1)
                {
                    path = logFilePath + "\\" + text2 + "_Log.txt";
                    text += Environment.NewLine;
                    text += "-----------------------------------------------------------";
                    text += Environment.NewLine;
                    text += errorMessage;
                }
                using (StreamWriter streamWriter = new StreamWriter(path, append: true))
                {
                    streamWriter.WriteLine(text);
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message.ToString(), log, logFilePath);
            }
        }
    }
}