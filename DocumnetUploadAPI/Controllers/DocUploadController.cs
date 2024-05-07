using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.PortableExecutable;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf.IO;
using SixLabors.ImageSharp.Formats;
using System.Drawing;
using System.Drawing.Imaging;
using Ghostscript.NET.Rasterizer;
using Ghostscript.NET;

namespace DocumnetUploadAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocUploadController : ControllerBase
    {
        private string _uploadFolder = "Documents";
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded!");
            }

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
            // Read PDF file and convert pages to images


            // Convert PDF to images
            ConvertPdfToImages(filePath, "Documents/" + Path.GetFileNameWithoutExtension(file.FileName) + "/Images");
            return Ok("File uploaded successfully!");
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

        public void ConvertPdfToImages(string pdfFilePath, string outputDirectory)
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
                    img.Save(pageFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
        }
    }
}
