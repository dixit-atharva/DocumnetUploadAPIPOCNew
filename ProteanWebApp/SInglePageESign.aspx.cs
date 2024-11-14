using PKCS7PDF_Signature;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProteanWebApp
{
    public partial class SInglePageESign : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string mainPath = @"D:\Rahul_Workplace\Code-Sample\DocumnetUploadAPIPOCNew\ProteanWebApp\Esign\";

            string Pdfpath = $"{mainPath}test.pdf";
            string Jarpath = $"{mainPath}Runnable2_eSign2.1_Single.jar";
            string ekycId = "";
            string AspId = "NSDLeGOVTest002";
            string AuthMode = "1";
            string Responseurl = "http://localhost:51180/EsignResponse.aspx";
            string Certificatepath = $"{mainPath}rahul_cert.p12";
            string Certificatepassward = "123456";
            string tickImagePath = $"{mainPath}tick.png";
            string xCo_ordinates = "40";
            string yCo_ordinates = "40";
            int signatureWidth = 133;
            int signatureHeight = 33;
            int serverTime = 15;
            string alias = "rahulpatel";
            int page = 1;
            string NameonSignature = "Sushrut Sathe";
            string locationOnSignature = "Pune";
            string reasonForSign = "Testing";
            string pdfPassword = "";
            string txn = "";
            int writelog = 1;
            // jdk or jre bin folder path for 64 bit(C:\Program Files\Java) and for 32 bit(C:\Program Files (x86)\Java)system
            string Jrebinfolderpath = @"C:\Program Files\Java\jre1.8.0_431\bin";
            string ResponseSignaturetype = "pkcs7pdf";
            try
            {
                Pkcs7pdfEsign req_resp = new Pkcs7pdfEsign();
                string ret = req_resp.CreateRequestXml(Pdfpath, Jarpath, ekycId, AspId, AuthMode, Responseurl, Certificatepath, Certificatepassward, tickImagePath, xCo_ordinates, yCo_ordinates, serverTime, alias, page, NameonSignature, locationOnSignature, reasonForSign, signatureWidth, signatureHeight, pdfPassword, txn, ResponseSignaturetype, Jrebinfolderpath, writelog);
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
    }
}