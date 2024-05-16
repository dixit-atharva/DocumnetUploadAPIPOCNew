using Newtonsoft.Json;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using PDFtoImage.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace PDFUtility;

public static class PDFConversation
{
    public static void CreatePdf(string folderPath, string outputDirectory, string fileName)
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

    public static void SignedPdf(string documentPath, string sinaturePath, string outputDirectory, string fileName)
    {
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        PdfDocument document = PdfReader.Open(documentPath, PdfDocumentOpenMode.Modify);

        // Get the first page of the document
        PdfPage page = document.Pages[0];

        // Define the coordinate where you want to add the signature
        XPoint signaturePosition = new XPoint(100, 100); // Adjust coordinates as needed

        // Load the signature image
        XImage signatureImage = XImage.FromFile(sinaturePath);

        // Draw the signature image onto the page
        XGraphics gfx = XGraphics.FromPdfPage(page);
        gfx.DrawImage(signatureImage, signaturePosition.X, signaturePosition.Y, signatureImage.PixelWidth, signatureImage.PixelHeight);

        // Save the modified document
        document.Save($"{outputDirectory}/{fileName}");
        document.Close();
    }

    public static void SignedPdfByCoordinates(string documentPath, string sinaturePath, string outputDirectory, string fileName)
    {
        string coordinatesObject = @"
        [
            {
                ""pageNumber"": ""1"",
                ""cordinate"": [
                    {
                        ""posX"": 34,
                        ""posY"": 67
                    }
                ]
            }
        ]";
#pragma warning disable IDE0059 // Unnecessary assignment of a value
        List<Pages>? pDFCoordinates = JsonConvert.DeserializeObject<List<Pages>>(coordinatesObject);
#pragma warning restore IDE0059 // Unnecessary assignment of a value

        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        // Input PDF file
        string inputFile = documentPath;
        // Output PDF file
        string outputFile = $"{outputDirectory}/{fileName}";

        // Image file to be added
        string imagePath = sinaturePath;


        if (pDFCoordinates != null)
        {
            PdfDocument document = PdfReader.Open(documentPath, PdfDocumentOpenMode.Modify);
            document.Info.Title = "Created with PDFsharp";

            foreach (var item in pDFCoordinates)
            {
                // Create an empty page
                PdfPage page = document.Pages[item.PageNumber - 1];
                // Get an XGraphics object for drawing
                XGraphics gfx = XGraphics.FromPdfPage(page);
                // Define the coordinates and dimensions from the provided data
                double left = 30;
                double top = 966;
                double width = 200;
                double height = 60;
                // Load the image
                XRect rect = new XRect(left, top, width, height);
                XImage image = XImage.FromFile(imagePath);
                // Draw the image on the page
                gfx.DrawImage(image, rect);
            }

            // Save the modified document
            document.Save($"{outputDirectory}/{fileName}");
            document.Close();
        }


    }

}
