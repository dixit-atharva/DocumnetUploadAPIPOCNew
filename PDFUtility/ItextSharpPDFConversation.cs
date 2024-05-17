using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using PDFtoImage.Model;
using System.IO;

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
                ImageData imageData = ImageDataFactory.Create(sinaturePath);

                foreach (var item in pDFCoordinates.Pages)
                {
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

                //// Load the signature image
                //ImageData imageData = ImageDataFactory.Create(sinaturePath);

                //foreach (var item in pDFCoordinates.Pages)
                //{
                //    // Add the signature image to the specified page
                //    PdfPage pdfPage = pdfDocument.GetPage(item.PageNumber);
                //    PdfCanvas pdfCanvas = new PdfCanvas(pdfPage.NewContentStreamAfter(), pdfPage.GetResources(), pdfDocument);
                //    iText.Kernel.Geom.Rectangle mediaBox = pdfPage.GetMediaBox();

                //    foreach (var itemcordinate in item.cordinate)
                //    {
                //        // Calculate scaling factors to maintain aspect ratio
                //        float widthScale = itemcordinate.Width / imageData.GetWidth();
                //        float heightScale = itemcordinate.Height / imageData.GetHeight();

                //        // Adjust Y-coordinate
                //        float y = mediaBox.GetHeight() - itemcordinate.Top;

                //        // Apply scaling using transformation matrix
                //        pdfCanvas.SaveState();
                //        pdfCanvas.ConcatMatrix(AffineTransform.GetScaleInstance(widthScale, heightScale));

                //        // Add the image
                //        pdfCanvas.AddImageAt(imageData, 0, 0, false);

                //        // Restore canvas state
                //        pdfCanvas.RestoreState();

                //        // Translate the image to the correct position
                //        pdfCanvas.MoveTo(itemcordinate.Left, y);
                //    }
                //}
                pdfDocument.Close();
            }
        }
    }

}
