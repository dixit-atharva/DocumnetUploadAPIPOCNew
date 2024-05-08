using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Drawing;
using System.IO;

namespace PDFtoImage;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public static class PDFConversation
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static void CreatePdf(string folderPath, string outputDirectory, string fileName)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        // Get all image files in the specified folder
        string[] imageFiles = Directory.GetFiles(folderPath);

        if (imageFiles.Length == 0)
        {
            Console.WriteLine("No image files found in the specified folder.");
            return;
        }

        // Create a new PDF document
        PdfDocument document = new PdfDocument();

        foreach (string imagePath in imageFiles)
        {
            //Read the image bytes
            byte[] imageBytes = File.ReadAllBytes(imagePath);

            //using Bitmap bitmap = new Bitmap(imagePath);

            //// Get the size of the image
            //Size imageSize = bitmap.Size;

            // Add a new page to the document
            PdfPage page = document.AddPage();

            // Create an XGraphics object for drawing on the page
            XGraphics gfx = XGraphics.FromPdfPage(page);

            //XImage image = XImage.FromFile(imagePath);
            //gfx.DrawImage(image, 0, 0, imageWidth, imageHeight);

            // Draw the image
            //using (MemoryStream stream = new MemoryStream(imageBytes))
            //{
            //    XImage image = XImage.FromStream(() => stream);
            //    gfx.DrawImage(image, 0, 0, imageSize.Width, imageSize.Height);
            //}

            using (Bitmap bitmap = new Bitmap(imagePath))
            {
                // Calculate scaling factors to fit the image within the page boundaries
                double widthScale = (double)page.Width / bitmap.Width;
                double heightScale = (double)page.Height / bitmap.Height;
                double scale = Math.Min(widthScale, heightScale);

                // Calculate new image size
                int newWidth = (int)(bitmap.Width * scale);
                int newHeight = (int)(bitmap.Height * scale);

                // Draw the image on the page
                gfx.DrawImage(XImage.FromFile(imagePath), 0, 0, newWidth, newHeight);
            }
        }

        // Save the document to the specified path
        document.Save($"{outputDirectory}/{fileName}");
    }
}
