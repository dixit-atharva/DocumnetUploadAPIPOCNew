using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using PDFtoImage.Model;
using iText.Html2pdf;
using System.IO;
using System;
using System.Text.RegularExpressions;

namespace PDFUtility;

public static class ItextSharpPDFConversation
{
    public static void SignedPdfByCoordinates(string documentPath, string sinaturePath, string outputDirectory, string fileName, PDFCoordinates pDFCoordinates)
    {
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        // Output PDF file
        string outputFile = $"{outputDirectory}/{fileName}";

        // Image file to be added
        string imagePath = sinaturePath;


        if (pDFCoordinates != null)
        {
            using (var pdfReader = new PdfReader(documentPath))
            using (var pdfWriter = new PdfWriter(outputFile))
            using (var pdfDocument = new PdfDocument(pdfReader, pdfWriter))
            {
                foreach (var item in pDFCoordinates.Pages)
                {
                    if (!string.IsNullOrWhiteSpace(item.ImageBase64))
                    {
                        byte[] imageBytes = Convert.FromBase64String(Regex.Replace(item.ImageBase64, @"^data:image\/[a-zA-Z]+;base64,", string.Empty));

                        // Load the signature image
                        ImageData imageData = ImageDataFactory.Create(imageBytes);

                        // Add the signature image to the specified page
                        PdfPage pdfPage = pdfDocument.GetPage(item.PageNumber);
                        PdfCanvas pdfCanvas = new PdfCanvas(pdfPage.NewContentStreamAfter(), pdfPage.GetResources(), pdfDocument);

                        iText.Kernel.Geom.Rectangle mediaBox = pdfPage.GetMediaBox();

                        foreach (var itemcordinate in item.cordinate)
                        {
                            // Adjust Y-coordinate
                            float y = mediaBox.GetHeight() - itemcordinate.Top;

                            pdfCanvas.AddImageAt(imageData, itemcordinate.Left, y, false);
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(item.SignText))
                    {
                    }
                        
                }

                pdfDocument.Close();
            }
        }
    }

    public static void GenearteHTML(string outputDirectory, string fileName, string htmlContent)
    {
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        var filePath = $"{outputDirectory}/{fileName}";
        // Create a PDF document

        var converter = new ConverterProperties();
        // Convert HTML to PDF and save directly to the file path
        HtmlConverter.ConvertToPdf(htmlContent, new FileStream(filePath, FileMode.Create), converter);


    }
}
