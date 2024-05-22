namespace DocumnetUploadAPI.Controllers
{
    internal class ReCaptchaResponse
    {
        public bool Success { get; set; }
        public IEnumerable<string> ErrorCodes { get; set; }
    }
}