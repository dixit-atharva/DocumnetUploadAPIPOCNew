using MultiEsignDLL;
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
    public partial class OTPEsign : System.Web.UI.Page
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
            //html += "</head><body onload='document.forms[0].submit()'>";
            html += "</head><body>";
            html += string.Format("<form name='PostForm' method='POST' action='{0}' enctype='multipart/form-data'>", remoteUrl);
            foreach (string key in collections.Keys)
            {
                //html += string.Format("<input name='{0}' type='text' value='{1}'>", key, collections[key]);
                html += string.Format("<input name='{0}' type='hidden' value='{1}'>", key, collections[key]);
            }
            html += "<input type=\"Submit\" value=\"Submit\" id =\"countButton\"/>";
            html += "</form></body></html>";
            Response.Clear();
            Response.ContentEncoding = Encoding.GetEncoding("ISO-8859-1");
            Response.HeaderEncoding = Encoding.GetEncoding("ISO-8859-1");
            Response.Charset = "ISO-8859-1";
            Response.Write(html);
            Response.End();
        }
    }
}