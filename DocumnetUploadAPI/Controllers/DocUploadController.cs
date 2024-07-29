using DocumnetUploadAPI.Model;
using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using PDFtoImage;
using PDFtoImage.Model;
using SkiaSharp;
using System.IO;
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

namespace DocumnetUploadAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocUploadController : ControllerBase
    {
        private readonly DocumentUpload documentUpload;
        private string _uploadFolder = "Documents";
        private readonly ILogger<DocUploadController> _logger;

        private const string SecretKey = "6LfqteQpAAAAACb9LMZ8MV7-zml74BkAdaSWs06I";
        private const string ReCaptchaVerificationUrl = "https://www.google.com/recaptcha/api/siteverify";
        private readonly ICaptchaValidator _captchaValidator;

        public DocUploadController(IOptions<DocumentUpload> documentUpload, ILogger<DocUploadController> logger, ICaptchaValidator captchaValidator)
        {
            this.documentUpload = documentUpload.Value;
            _uploadFolder = this.documentUpload.Path;
            _logger = logger;
            _captchaValidator = captchaValidator;
        }

        //[HttpPost]
        //public async Task<IActionResult> Upload([FromForm] IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //    {
        //        return BadRequest("No file uploaded!");
        //    }

        //    _logger.LogInformation($"Upload Started at {_uploadFolder}");

        //    try
        //    {
        //        var fileName = Path.GetFileName(file.FileName);
        //        var filePath = Path.Combine(_uploadFolder + "/" + Path.GetFileNameWithoutExtension(file.FileName), fileName);

        //        if (!Directory.Exists(_uploadFolder + "/" + Path.GetFileNameWithoutExtension(file.FileName)))
        //        {
        //            Directory.CreateDirectory(_uploadFolder + "/" + Path.GetFileNameWithoutExtension(file.FileName));
        //        }

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }

        //        // Convert PDF to images
        //        ConvertPDFUtilitys(filePath, _uploadFolder + "/" + Path.GetFileNameWithoutExtension(file.FileName) + "/Images");
        //        return Ok("File uploaded successfully!");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogInformation($"Upload Failed  {ex.Message} :: {ex.InnerException}");
        //        return Ok("File uploaded Failed!");
        //    }

        //}

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded!");
            }

            _logger.LogInformation($"Upload Started at {_uploadFolder}");

            try
            {
                var fileName = Path.GetFileName(file.FileName);

                string fileFolderName = GetFolderNameFromFile(fileName);

                var origionalPath = Path.Combine($"{_uploadFolder}/{fileFolderName}/{fileName}");

                string fileDirectory = Path.Combine($"{_uploadFolder}/{fileFolderName}"); ;

                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }

                string fileImagesDirectory = Path.Combine($"{_uploadFolder}/{fileFolderName}/ORG");

                if (!Directory.Exists(fileImagesDirectory))
                {
                    Directory.CreateDirectory(fileImagesDirectory);
                }

                using var fileStream = new FileStream(origionalPath, FileMode.Create);

                await file.CopyToAsync(fileStream);

                fileStream.Close();

                //byte[] pdfBytes = System.IO.File.ReadAllBytes(origionalPath);

                //// Convert PDF bytes to base64 string
                //string base64String = Convert.ToBase64String(pdfBytes);

                //int i = 1;
                //await foreach (var image in PDFUtility.Conversion.ToImagesAsync(base64String))
                //{
                //    using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                //    using (var stream = System.IO.File.OpenWrite(Path.Combine(fileImagesDirectory, $"{i}.png")))
                //    {
                //        data.SaveTo(stream);
                //        i++;
                //    }
                //}

                //string[] filePaths = Directory.GetFiles(fileImagesDirectory);

                //var base64Images = new List<string>();

                //foreach (var item in filePaths)
                //{
                //    base64Images.Add(await ImageToBase64(item));
                //}

                //string PDFCombinedDirectory = Path.Combine($"{_uploadFolder}/{fileFolderName}/SIGNED");

                // PDFUtility.PDFConversation.CreatePdf(fileImagesDirectory, PDFCombinedDirectory, fileName);

                //string signaturePath = Path.Combine($"{_uploadFolder}/RP_Signature.jpg");

                //PDFUtility.PDFConversation.SignedPdf(origionalPath, signaturePath, PDFCombinedDirectory, "PdfSharpCore.pdf");

                //PDFUtility.ItextSharpPDFConversation.SignedPdf(origionalPath, signaturePath, PDFCombinedDirectory, "iTextSharp.pdf");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Upload Failed  {ex.Message} :: {ex.InnerException}");
                return Ok("File uploaded Failed!");
            }

        }

        private static string GetFolderNameFromFile(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName).Replace(" ","_");
        }

        [HttpGet("files")]
        public async Task<ActionResult<List<string>>> GetFiles(string filename)
        {
            try
            {
                _logger.LogInformation($"GetFiles of {filename}");
                var base64Images = new List<string>();
                string folderName = Path.GetFileNameWithoutExtension(filename);

                string[] filePaths = Directory.GetFiles($"{_uploadFolder}/{folderName}/Images");

                foreach (var item in filePaths)
                {
                    base64Images.Add(await ImageToBase64(item));
                }

                return Ok(base64Images);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetFiles Failed  {ex.Message} :: {ex.InnerException}");
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }


        [HttpPost("pdfsigned")]
        public async Task<IActionResult> PDFSigned(PDFCoordinates pDFCoordinates)
        {
            _logger.LogInformation($"Upload Started at {_uploadFolder}");

            try
            {
                var fileName = Path.GetFileName(pDFCoordinates.FileName);

                string fileFolderName = GetFolderNameFromFile(fileName);

                string pdfFile = $"{_uploadFolder}/{fileFolderName}/{pDFCoordinates.FileName}";

                var origionalPath = Path.Combine($"{_uploadFolder}/{fileFolderName}/{fileName}");

                string fileDirectory = Path.Combine($"{_uploadFolder}/{fileFolderName}"); ;

                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }

                //System.IO.File.Copy(pdfFile, Path.Combine($"{_uploadFolder}/{fileFolderName}/{fileName}"),true);

                string PDFCombinedDirectory = Path.Combine($"{_uploadFolder}/{fileFolderName}/SIGNED");

                // PDFUtility.PDFConversation.CreatePdf(fileImagesDirectory, PDFCombinedDirectory, fileName);

                string signaturePath = Path.Combine($"{_uploadFolder}/RP_Signature.jpg");

                //PDFUtility.PDFConversation.SignedPdf(origionalPath, signaturePath, PDFCombinedDirectory, "PdfSharpCore.pdf");

                //PDFUtility.ItextSharpPDFConversation.SignedPdfByCoordinates(origionalPath, signaturePath, PDFCombinedDirectory, "iTextSharp.pdf");

                PDFUtility.PDFConversation.SignedPdfByCoordinates(origionalPath, signaturePath, PDFCombinedDirectory, "PdfSharpCore.pdf", pDFCoordinates);
                PDFUtility.ItextSharpPDFConversation.SignedPdfByCoordinates(origionalPath, signaturePath, PDFCombinedDirectory, "iTextSharp.pdf", pDFCoordinates);

                //var bytesReturn = PDFUtility.PDFConversation.SignedPdfByCoordinatesByte(origionalPath, signaturePath, PDFCombinedDirectory, "PdfSharpCore.pdf", pDFCoordinates);

                //System.IO.File.WriteAllBytes($"{PDFCombinedDirectory}/PdfSharpCoreByte.pdf", bytesReturn);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Upload Failed  {ex.Message} :: {ex.InnerException}");
                return Ok("File uploaded Failed!");
            }

        }

        [HttpPost("converttoPDF")]
        public async Task<IActionResult> ConvertImageToPDF(ImageUpload imageUpload)
        {
            _logger.LogInformation($"Upload Started at {_uploadFolder}");

            try
            {
                var fileName = Path.GetFileName(imageUpload.FileName);

                string fileFolderName = GetFolderNameFromFile(fileName);

                string pdfFileName = $"{fileName.Split('.')[0]}.pdf";

                string pdfFile = $"{_uploadFolder}/{fileFolderName}/{pdfFileName}";

                string fileDirectory = Path.Combine($"{_uploadFolder}/{fileFolderName}"); ;

                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }

                var bytesReturn = PDFUtility.PDFConversation.ConvertImagetoPDFByte(imageUpload.Data);

                System.IO.File.WriteAllBytes($"{pdfFile}", bytesReturn);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Upload Failed  {ex.Message} :: {ex.InnerException}");
                return Ok("File uploaded Failed!");
            }

        }

        private async Task<string> ImageToBase64(string imagePath)
        {
            byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);
            return Convert.ToBase64String(imageBytes);
        }

        [HttpGet("compare")]
        public async Task<IActionResult> DocumentCompare()
        {
            _logger.LogInformation($"DocumentCompare Started at {_uploadFolder}");

            try
            {
                var origionalfileName = "CV.pdf";

                string fileFolderName = GetFolderNameFromFile(origionalfileName);

                var origionalPath = Path.Combine($"{_uploadFolder}/{fileFolderName}/{origionalfileName}");

                var modifiedPath = Path.Combine($"{_uploadFolder}/{fileFolderName}/SIGNED/PdfSharpCore.pdf");

                var origionalFileBytes = GetFileBytes(origionalPath);

                var modifiedFileBytes = GetFileBytes(modifiedPath);

                var origionalFileHash = PdfHashingService.ComputeHash(origionalFileBytes);

                var modifiedFileHash = PdfHashingService.ComputeHash(modifiedFileBytes);

                var isSame = PdfHashingService.CompareHashes(origionalFileHash, modifiedFileHash);

                return Ok(isSame);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Upload Failed  {ex.Message} :: {ex.InnerException}");
                return Ok("File compare Failed!");
            }

        }

        [HttpGet("converttohtml")]
        public async Task<IActionResult> ConvertToHTML()
        {
            _logger.LogInformation($"ConvertToHTML Started at {_uploadFolder}");

            try
            {
                var origionalfileName = "email.html";

                string htmlContent = System.IO.File.ReadAllText(Path.Combine($"{_uploadFolder}/{origionalfileName}"));

                string PDFCombinedDirectory = Path.Combine($"{_uploadFolder}/HTML");

                PDFUtility.PDFConversation.GenearteHTML(PDFCombinedDirectory, "PdfSharpCore.pdf", htmlContent);
                //PDFUtility.ItextSharpPDFConversation.GenearteHTML(PDFCombinedDirectory, "iTextSharp.pdf", htmlContent);

                //PdfHeaderFooterHtmlExample.HTMLConvert.Main(PDFCombinedDirectory, "iTextCore.pdf", htmlContent);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ConvertToHTML Failed  {ex.Message} :: {ex.InnerException}");
                return Ok();
            }

        }

        [HttpPost("convertHtmlToPdf")]
        public async Task<IActionResult> ConvertHTMLToPdf(HtmlContentRequest request)
        {
            try
            {
                string csvFilePath = Path.Combine($"{_uploadFolder}/sample.csv"); 
                var csvData = ReadCsv(csvFilePath);

                if (csvData.Count > 0)
                {
                    var firstRow = csvData[0];
                    string pattern = @"\[\[([^\]]+)\]\]";
                    string result = Regex.Replace(request.Content, pattern, match =>
                    {
                        string key = match.Groups[1].Value;
                        return firstRow.ContainsKey(key) ? firstRow[key] : match.Value;
                    });
                    Console.WriteLine(result);
                    string PDFCombinedDirectory = Path.Combine($"{_uploadFolder}/HTML/ckeditorDemo");

                    PDFUtility.PDFConversation.GenearteHTML(PDFCombinedDirectory, "PdfSharpCore.pdf", result);
                    return Ok();
                }
                else
                {
                    Console.WriteLine("The CSV file is empty or could not be read.");
                    return Ok();
                }

                //string replacement = "sample_text"; 
                //string pattern = @"\[\[([^\]]+)\]\]";
                //string result = Regex.Replace(request.Content, pattern, replacement);
                //string PDFCombinedDirectory = Path.Combine($"{_uploadFolder}/HTML/ckeditorDemo");

                //PDFUtility.PDFConversation.GenearteHTML(PDFCombinedDirectory, "PdfSharpCore.pdf", result);
                //return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ConvertToHTML Failed  {ex.Message} :: {ex.InnerException}");
                return Ok();
            }

        }

        static List<Dictionary<string, string>> ReadCsv(string filePath)
        {
            var csvData = new List<Dictionary<string, string>>();
            using (var reader = new StreamReader(filePath))
            {
                var headers = reader.ReadLine().Split(',');
                while (!reader.EndOfStream)
                {
                    var values = reader.ReadLine().Split(',');
                    var row = new Dictionary<string, string>();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        row[headers[i]] = values[i];
                    }
                    csvData.Add(row);
                }
            }
            return csvData;
        }


        [HttpPost("verify")]
        public async Task<IActionResult> Verify(GoogleCaprchaV3Request googleCaprchaV3Request)
        {
            //if (string.IsNullOrEmpty(googleCaprchaV3Request.Token))
            //{
            //    return BadRequest("Invalid reCAPTCHA token.");
            //}

            //var client = _httpClientFactory.CreateClient();
            //var response = await client.PostAsync($"{ReCaptchaVerificationUrl}?secret={SecretKey}&response={token}", null);

            //if (!response.IsSuccessStatusCode)
            //{
            //    return StatusCode((int)response.StatusCode, "Error verifying reCAPTCHA.");
            //}

            //var jsonResponse = await response.Content.ReadAsStringAsync();
            //var reCaptchaResponse = JsonConvert.DeserializeObject<ReCaptchaResponse>(jsonResponse);

            //if (reCaptchaResponse == null || !reCaptchaResponse.Success)
            //{
            //    return BadRequest("reCAPTCHA verification failed.");
            //}

            //return Ok("reCAPTCHA verified successfully.");

            if (string.IsNullOrEmpty(googleCaprchaV3Request.Token))
            {
                return BadRequest("Invalid reCAPTCHA token.");
            }

            var isValid = await _captchaValidator.IsCaptchaPassedAsync(googleCaprchaV3Request.Token);

            if (!isValid)
            {
                return BadRequest("reCAPTCHA verification failed.");
            }

            return Ok("reCAPTCHA verified successfully.");
        }
        private byte[] GetFileBytes(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                return System.IO.File.ReadAllBytes(filePath);
            }
            else
            {
                throw new FileNotFoundException("File not found.", filePath);
            }
        }
    }
}
