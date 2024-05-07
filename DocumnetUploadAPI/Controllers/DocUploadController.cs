using DocumnetUploadAPI.Model;
using Ghostscript.NET.Rasterizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
                var filePath = Path.Combine(_uploadFolder + "/" + Path.GetFileNameWithoutExtension(file.FileName), fileName);

                if (!Directory.Exists(_uploadFolder + "/" + Path.GetFileNameWithoutExtension(file.FileName)))
                {
                    Directory.CreateDirectory(_uploadFolder + "/" + Path.GetFileNameWithoutExtension(file.FileName));
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Convert PDF to images
                ConvertPdfToImages(filePath, _uploadFolder + "/" + Path.GetFileNameWithoutExtension(file.FileName) + "/Images");
                return Ok("File uploaded successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Upload Failed  {ex.Message} :: {ex.InnerException}");
                return Ok("File uploaded Failed!");
            }

        }

        [HttpGet("filecount")]
        public async Task<ActionResult<string[]>> GetFileCount(string folderPath)
        {
            try
            {
                string[] filePaths = Directory.GetFiles(folderPath);
                for (global::System.Int32 i = 0; i < filePaths.Length; i++)
                {
                    filePaths[i] = filePaths[i].Replace("Documents/21583473018/Images\\", "../../../assets/Images/");
                }

                return Ok(filePaths);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
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

        private async Task<string> ImageToBase64(string imagePath)
        {
            byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);
            return Convert.ToBase64String(imageBytes);
        }

        private void ConvertPdfToImages(string pdfFilePath, string outputDirectory)
        {
            using (var rasterizer = new GhostscriptRasterizer())
            {
                rasterizer.Open(Path.GetFullPath(pdfFilePath));

                // Ensure the output directory exists
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                for (int pageNumber = 1; pageNumber <= rasterizer.PageCount; pageNumber++)
                {
                    var pageFilePath = Path.Combine(outputDirectory, $"{pageNumber}.jpg");
                    var img = rasterizer.GetPage(300, pageNumber);
                    img.Save(pageFilePath,
                             format: System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
        }
    }
}
