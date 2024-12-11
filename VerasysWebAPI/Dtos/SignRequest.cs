namespace VerasysWebAPI.Dtos
{
    public class PdfCoordinate
    {
        public int w { get; set; }
        public int x { get; set; }
        public int h { get; set; }
        public int y { get; set; }
    }

    public class SignatureDetail
    {
        public List<PdfCoordinate> coordinates { get; set; }
        public string page { get; set; }
    }

    public class PdfDetail
    {
        public string reason { get; set; }
        public List<SignatureDetail> signaturedetails { get; set; }
        public string docUrl { get; set; }
        public string docInfo { get; set; }
        public string pdfbase64val { get; set; }
    }

    public class SignRequest
    {
        public string signedPdfPath { get; set; }
        public string ver { get; set; }
        public string tempInfoPath { get; set; }
        public string pfxAlias { get; set; }
        public string isresponseXML { get; set; }
        public string txn { get; set; }
        public string pfxPath { get; set; }
        public string signerName { get; set; }
        public string pdfDestinationPath { get; set; }
        public string responseUrl { get; set; }
        public string pfxPassword { get; set; }
        public string aspId { get; set; }
        public string signingAlgorithm { get; set; }
        public string maxWaitPeriod { get; set; }
        public List<PdfDetail> pdfdetails { get; set; }
        public string isrequestXML { get; set; }
        public int AuthMode { get; set; }
        public string fileType { get; set; }
        public string tickImgPath { get; set; }
        public string aspLogo { get; set; }
        public string signatureFontSize { get; set; }
    }
}
