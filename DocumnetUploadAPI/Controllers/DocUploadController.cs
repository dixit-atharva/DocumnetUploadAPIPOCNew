using DocumnetUploadAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SkiaSharp;
using System.IO;

namespace DocumnetUploadAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocUploadController : ControllerBase
    {
        private readonly DocumentUpload documentUpload;
        private string _uploadFolder = "Documents";
        private readonly ILogger<DocUploadController> _logger;

        public DocUploadController(IOptions<DocumentUpload> documentUpload, ILogger<DocUploadController> logger)
        {
            this.documentUpload = documentUpload.Value;
            _uploadFolder = this.documentUpload.Path;
            _logger = logger;
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

                byte[] pdfBytes = System.IO.File.ReadAllBytes(origionalPath);

                // Convert PDF bytes to base64 string
                string base64String = Convert.ToBase64String(pdfBytes);

                int i = 1;
                await foreach (var image in PDFUtility.Conversion.ToImagesAsync(base64String))
                {
                    using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                    using (var stream = System.IO.File.OpenWrite(Path.Combine(fileImagesDirectory, $"{i}.png")))
                    {
                        data.SaveTo(stream);
                        i++;
                    }
                }

                string[] filePaths = Directory.GetFiles(fileImagesDirectory);

                var base64Images = new List<string>();

                foreach (var item in filePaths)
                {
                    base64Images.Add(await ImageToBase64(item));
                }

                string PDFCombinedDirectory = Path.Combine($"{_uploadFolder}/{fileFolderName}/SIGNED");

                // PDFUtility.PDFConversation.CreatePdf(fileImagesDirectory, PDFCombinedDirectory, fileName);

                string signaturePath = Path.Combine($"{_uploadFolder}/RP_Signature.jpg");

                //PDFUtility.PDFConversation.SignedPdf(origionalPath, signaturePath, PDFCombinedDirectory, "PdfSharpCore.pdf");

                PDFUtility.ItextSharpPDFConversation.SignedPdf(origionalPath, signaturePath, PDFCombinedDirectory, "iTextSharp.pdf");

                return Ok(base64Images);
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
        public async Task<IActionResult> PDFSigned()
        {
            _logger.LogInformation($"Upload Started at {_uploadFolder}");

            try
            {
                string pdfFile = $"{_uploadFolder}/CV.pdf";

                var fileName = Path.GetFileName(pdfFile);

                string fileFolderName = GetFolderNameFromFile(fileName);

                var origionalPath = Path.Combine($"{_uploadFolder}/{fileFolderName}/{fileName}");

                string fileDirectory = Path.Combine($"{_uploadFolder}/{fileFolderName}"); ;

                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }

                string PDFCombinedDirectory = Path.Combine($"{_uploadFolder}/{fileFolderName}/SIGNED");

                // PDFUtility.PDFConversation.CreatePdf(fileImagesDirectory, PDFCombinedDirectory, fileName);

                string signaturePath = Path.Combine($"{_uploadFolder}/RP_Signature.jpg");

                //PDFUtility.PDFConversation.SignedPdf(origionalPath, signaturePath, PDFCombinedDirectory, "PdfSharpCore.pdf");

                //PDFUtility.ItextSharpPDFConversation.SignedPdfByCoordinates(origionalPath, signaturePath, PDFCombinedDirectory, "iTextSharp.pdf");

                PDFUtility.PDFConversation.SignedPdfByCoordinates(origionalPath, signaturePath, PDFCombinedDirectory, "PdfSharpCore.pdf");

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


    }
}
