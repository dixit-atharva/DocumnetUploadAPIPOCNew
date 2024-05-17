using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using PDFtoImage.Model;
using iText.Html2pdf;
using System.IO;
using System;
using iText.Layout.Element;
using iText.Layout;
using System.Text.RegularExpressions;
using iText.Layout.Borders;
using iText.Kernel.Font;
using PdfSharpCore.Pdf.Advanced;
using iText.Kernel.Colors;
using iText.Layout.Properties;



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
                // Load the signature image


                foreach (var item in pDFCoordinates.Pages)
                {
                    if (!string.IsNullOrWhiteSpace(item.ImageBase64))
                    {


                        byte[] imageBytes = Convert.FromBase64String(Regex.Replace(item.ImageBase64, @"^data:image\/[a-zA-Z]+;base64,", string.Empty));

                        ImageData imageData = ImageDataFactory.Create(imageBytes);
                        // Add the signature image to the specified page
                        PdfPage pdfPage = pdfDocument.GetPage(item.PageNumber);
                        PdfCanvas pdfCanvas = new PdfCanvas(pdfPage.NewContentStreamAfter(), pdfPage.GetResources(), pdfDocument);



                        foreach (var itemcordinate in item.cordinate)
                        {
                            Image img = new Image(imageData);

                            img.ScaleToFit(itemcordinate.Width, itemcordinate.Height);

                            // Adjust Y-coordinate
                            float y = pdfPage.GetMediaBox().GetTop() - itemcordinate.Top - itemcordinate.Height;

                            img.SetFixedPosition(itemcordinate.Left, y);

                            new Canvas(pdfCanvas, pdfPage.GetMediaBox()).Add(img);


                            // Adding text below the image
                            // Define the text and its properties
                            Paragraph p = new Paragraph("GoDigitel Esigned")
                                .SetFontSize(12)
                                .SetFontColor(iText.Kernel.Colors.ColorConstants.BLACK)
                                .SetBorder(Border.NO_BORDER);

                            // Calculate the y-coordinate for the text (below the image)
                            float yText = y-10 ; // Adjust 20 points for the gap between image and text

                            // Add the text to the canvas
                            new Canvas(pdfCanvas, pdfPage.GetMediaBox())

                                .Add(p.SetFixedPosition(itemcordinate.Left, yText - 10, itemcordinate.Width));

                            Paragraph p1 = new Paragraph("IP Address")
                                .SetFontSize(12)
                                .SetFontColor(ColorConstants.BLACK)
                                .SetBorder(Border.NO_BORDER)
                                .SetRotationAngle(Math.PI / 2); // Rotate 90 degrees for vertical alignment

                            // Calculate the x and y coordinates for vertical text beside the image
                            float xText = itemcordinate.Left + 5; // Adjust 20 points for the gap between image and text
                            float yTextVertical = y + (itemcordinate.Height / 10); // Center vertically with the image

                            // Add the vertical text to the canvas
                            new Canvas(pdfCanvas, pdfPage.GetMediaBox())
                                .Add(p1.SetFixedPosition(xText, yTextVertical - 15, itemcordinate.Height + 30));

                        }
                    }
                }

                //// Load the signature image
                //ImageData imageData = ImageDataFactory.Create(sinaturePath);

                //foreach (var item in pDFCoordinates.Pages)
                //{
                //    // Add the signature image to the specified page
                //    PdfPage pdfPage = pdfDocument.GetPage(item.PageNumber);
                //    PdfCanvas pdfCanvas = new PdfCanvas(pdfPage.NewContentStreamAfter(), pdfPage.GetResources(), pdfDocument);
                //    iText.Kernel.Geom.Rectangle mediaBox = pdfPage.GetMediaBox();

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
