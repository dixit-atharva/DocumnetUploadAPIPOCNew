using HtmlRendererCore.PdfSharp;
using Newtonsoft.Json;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using PDFtoImage.Model;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

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

        double leftInPoints = ConvertPixelsToPoints(292);
        double topInPoints = ConvertPixelsToPoints(106);

        // Define the coordinate where you want to add the signature
        //XPoint signaturePosition = new XPoint(leftInPoints, topInPoints); // Adjust coordinates as needed

        // Load the signature image
        XImage signatureImage = XImage.FromFile(sinaturePath);

        // Draw the signature image onto the page
        XGraphics gfx = XGraphics.FromPdfPage(page);
        gfx.DrawImage(signatureImage, leftInPoints, topInPoints);

        // Save the modified document
        document.Save($"{outputDirectory}/{fileName}");
        document.Close();
    }

    // Function to convert pixels to points
    private static double ConvertPixelsToPoints(double pixels)
    {
        // Conversion factor from pixels to points
        const double PixelsPerPoint = 0.75;
        return pixels;// / PixelsPerPoint;
    }

    public static void SignedPdfByCoordinates(string documentPath, string sinaturePath, string outputDirectory, string fileName, PDFCoordinates pDFCoordinates)
    {
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

            foreach (var item in pDFCoordinates.Pages)
            {
                // Create an empty page
                PdfPage page = document.Pages[item.PageNumber - 1];
                // Get an XGraphics object for drawing
                XGraphics gfx = XGraphics.FromPdfPage(page);
                if (!string.IsNullOrWhiteSpace(item.ImageBase64))
                {
                    byte[] imageBytes = Convert.FromBase64String(Regex.Replace(item.ImageBase64, @"^data:image\/[a-zA-Z]+;base64,", string.Empty));
                    foreach (var itemcordinate in item.cordinate)
                    {
                        // Define the coordinates and dimensions from the provided data
                        double left = itemcordinate.Left;
                        double top = itemcordinate.Top;
                        double width = itemcordinate.Width;
                        double height = itemcordinate.Height;
                        // Load the image
                        if (imageBytes != null && imageBytes.Length > 0)
                        {
                            using (MemoryStream ms = new MemoryStream(imageBytes))
                            {
                                // Create an XImage from the memory stream
                                XImage image = XImage.FromStream(() => ms);

                                // Now you can use 'imageXObject' as your XImage object in PDFsharp
                                // For example, you can add it to a PDF document
                                gfx.DrawImage(image, left, top, width, height);


                                
                                string text = "GoDigitel Esigned";
                                XFont font = new XFont("Arial", 12); // Font for the text
                                XBrush brush = XBrushes.Black; // Brush for the text color

                                // Calculate the width of the text
                                double textWidth = gfx.MeasureString(text, font).Width;

                                // Calculate the X coordinate to center the text below the image
                                double textX = left + (width - textWidth) / 2; // Centering the text horizontally

                                // Y coordinate below the image
                                double textY = top + height+15; // Adjust as needed

                                // Draw the text
                                gfx.DrawString(text, font, brush, textX, textY);



                                
                                string text1 = "IP Address";
                                XFont font1 = new XFont("Arial", 12); // Font for the text
                                XBrush brush1 = XBrushes.Black; // Brush for the text color

                                // Calculate the width of the text
                                double textWidth1 = gfx.MeasureString(text1, font1).Width;

                                // Calculate the X coordinate to center the text below the image
                                double textX1 = left-5  ; // Centering the text horizontally

                                // Y coordinate below the image
                                double textY1 = top ; // Adjust as needed

                                // Rotate the graphics context 90 degrees clockwise around the text position
                                gfx.RotateAtTransform(90, new XPoint(textX1, textY1));

                                // Draw the rotated text
                                gfx.DrawString(text1, font1, brush1, textX1, textY1);

                                gfx.RotateAtTransform(-90, new XPoint(textX1, textY1));


                                // Reset the transformation to avoid affecting subsequent drawing operations

                            }
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(item.SignText))
                {

                    foreach (var itemcordinate in item.cordinate)
                    {
                        // Define the coordinates and dimensions from the provided data
                        double left = itemcordinate.Left;
                        double top = itemcordinate.Top;
                        double width = itemcordinate.Width;
                        double height = itemcordinate.Height;
                        // Load the image



                        // Add text below and to the left of the image
                        
                        XFont font2 = new XFont(item.SignTextFont, 15); // Font for the text
                        XSize textSize = gfx.MeasureString(item.SignText, font2);
                        double textWidthMain = textSize.Width;
                        double textHeightMain = textSize.Height;
                        XBrush brush11 = XBrushes.Black; // Brush for the text color

                        // Calculate the X coordinate for horizontal centering
                        double textX2 = left + (width - textWidthMain) / 2;

                        // Calculate the Y coordinate for vertical centering
                        double textY2 = top + (height - textHeightMain) / 2 + textHeightMain;

                        // Draw the string at the calculated position
                        gfx.DrawString(item.SignText, font2, brush11, textX2, textY2);

                        //double textWidthMain = gfx.MeasureString(item.SignText, font2).Width;
                        //double textHeightMain = gfx.MeasureString(item.SignText, font2).Height;
                        //XBrush brush11 = XBrushes.Black; // Brush for the text color
                        //double textX2 = left + (width - textWidthMain) / 2; ; // X coordinate of the text (same as image)
                        //double textY2 = top + (height - textHeightMain) / 2; // Y coordinate below the image
                        //gfx.DrawString(item.SignText, font2, brush11, textX2, textY2);



                        string text = "GoDigitel Esigned";
                        XFont font = new XFont("Arial", 12); // Font for the text
                        XBrush brush = XBrushes.Black; // Brush for the text color

                        // Calculate the width of the text
                        double textWidth = gfx.MeasureString(text, font).Width;

                        // Calculate the X coordinate to center the text below the image
                        double textX = left + (width - textWidth) / 2; // Centering the text horizontally

                        // Y coordinate below the image
                        double textY = top + height + 15; // Adjust as needed

                        // Draw the text
                        gfx.DrawString(text, font, brush, textX, textY);




                        string text1 = "IP Address";
                        XFont font1 = new XFont("Arial", 12); // Font for the text
                        XBrush brush1 = XBrushes.Black; // Brush for the text color

                        // Calculate the width of the text
                        double textWidth1 = gfx.MeasureString(text1, font1).Width;

                        // Calculate the X coordinate to center the text below the image
                        double textX1 = left - 5; // Centering the text horizontally

                        // Y coordinate below the image
                        double textY1 = top; // Adjust as needed

                        // Rotate the graphics context 90 degrees clockwise around the text position
                        gfx.RotateAtTransform(90, new XPoint(textX1, textY1));

                        // Draw the rotated text
                        gfx.DrawString(text1, font1, brush1, textX1, textY1);

                        gfx.RotateAtTransform(-90, new XPoint(textX1, textY1));

                    }
                }
            }

            // Save the modified document
            document.Save($"{outputDirectory}/{fileName}");
            document.Close();
        }


    }

    public static double ConvertToPDFCoordinates(double value, double maxValue, double defaultValue, double imageMaxValue)
    {

        return Convert.ToDouble(value) * (maxValue / imageMaxValue);


    }

    public static void SignedPdfByCoordinates1(string documentPath, string sinaturePath, string outputDirectory, string fileName)
    {
        int x = 572;
        int y = 19;

        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        // Load the PDF document
        PdfDocument document = PdfReader.Open(documentPath, PdfDocumentOpenMode.Modify);

        // Assuming we want to add signature to the first page
        PdfPage page = document.Pages[0];

        // Load the signature image
        XImage signatureImage = XImage.FromFile(sinaturePath);

        // Define the position for the signature
        XPoint signaturePosition = new XPoint(ConvertToPdfXCoordinate(x), ConvertToPdfYCoordinate(page, y));

        // Create a graphics object to draw on the page
        XGraphics gfx = XGraphics.FromPdfPage(page);

        // Draw the signature image at the specified position
        gfx.DrawImage(signatureImage, signaturePosition.X, signaturePosition.Y, signatureImage.PixelWidth, signatureImage.PixelHeight);


        // Save the changes to the PDF document
        document.Save($"{outputDirectory}/{fileName}");


    }

    public static void GenearteHTML(string outputDirectory, string fileName, string htmlContent)
    {
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        var filePath = $"{outputDirectory}/{fileName}";


        // Act
        var result = PdfGenerator.GeneratePdf(htmlContent, PdfSharpCore.PageSize.A4);

        result.Save(filePath);

    }


    // Function to convert Angular X coordinate to PDF X coordinate
    private static double ConvertToPdfXCoordinate(int angularX)
    {
        // Adjust as necessary based on the differences in coordinate systems
        // You may need to consider scaling, margins, or any offsets
        return angularX; // Placeholder, replace with actual conversion logic
    }

    // Function to convert Angular Y coordinate to PDF Y coordinate
    private static double ConvertToPdfYCoordinate(PdfPage page, int angularY)
    {
        // Adjust as necessary based on the differences in coordinate systems
        // PDF coordinate system has origin at bottom-left corner of the page
        return page.Height.Point - angularY; // Invert Y coordinate and adjust for page height
    }
}
