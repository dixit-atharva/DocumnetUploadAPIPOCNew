namespace VerasysWebAPI.Dtos
{
    public class EsignResponse
    {
        public string signedFileParentPath { get; set; }
        public string errorMessage { get; set; }
        public string errorCode { get; set; }
        public string responseXML { get; set; }
        public string txnref { get; set; }
        public string txn { get; set; }
        public string requestXML { get; set; }
        public string status { get; set; }
        public string responseCode { get; set; }
    }
}
