using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace PDFUtility;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public static class ItextSharpPDFConversation
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
{

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static void SignedPdf(string documentPath, string sinaturePath, string outputDirectory, string fileName)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
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
        // Text to be added
        string text = "This is the text to be added.";

        // Position for image (x, y coordinates)
        float imageX = 100f;
        float imageY = 100f;

        // Position for text (x, y coordinates)
        float textX = 100f;
        float textY = 100f;

        using (FileStream fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            // Create a PdfReader object
            PdfReader reader = new PdfReader(inputFile);
            // Create a PdfStamper object to modify the PDF
            PdfStamper stamper = new PdfStamper(reader, fs);
            // Get the PdfContentByte object
            PdfContentByte cb = stamper.GetOverContent(1); // First page

            // Add image to the first page
            Image image = Image.GetInstance(imagePath);
            image.SetAbsolutePosition(imageX, imageY);
            cb.AddImage(image);

            // Go to the second page
            cb = stamper.GetOverContent(2); // Second page

            // Add text to the second page
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 20);
            cb.SetColorFill(BaseColor.RED);
            cb.SetTextMatrix(textX, textY);
            cb.ShowText(text);
            cb.EndText();

            // Close the PdfStamper
            stamper.Close();
            // Close the PdfReader
            reader.Close();
        }
    }
}
