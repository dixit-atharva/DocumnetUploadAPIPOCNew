namespace VerasysWebAPI.Dtos
{
    public class AppendSignRequest
    {
        public string tempInfoPath { get; set; }
        public string signedFileParentPath { get; set; }
        public string responseXML { get; set; }
        public string pdfDestinationPath { get; set; }
        public string tickImgPath { get; set; }
        public string aspLogo { get; set; }
        public string signatureFontSize { get; set; }
    }
}
