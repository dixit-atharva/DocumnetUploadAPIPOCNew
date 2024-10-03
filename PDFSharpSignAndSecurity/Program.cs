// PDFsharp - A .NET library for processing PDF
// See the LICENSE file in the solution root for more information.

/*
  This sample demonstrates how to create and open a PDF 1.6 document with 
  AES 128 bit encryption.
*/

using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Signatures;
using PdfSharp.Quality;
using System.Security.Cryptography.X509Certificates;



// Create a new PDF document.
var document = new PdfDocument();
document.Info.Title = "AES 128 bit encryption demonstration";
document.PageLayout = PdfPageLayout.SinglePage;

//// Create an empty page in this document.
//var page = document.AddPage();

//// Draw some text.
//var gfx = XGraphics.FromPdfPage(page);
//var font = new XFont("Times New Roman", 20, XFontStyleEx.BoldItalic);
//gfx.DrawString("SignCare Permission By TM", font, XBrushes.Black,
//    new XRect(0, 0, page.Width.Point, page.Height.Point), XStringFormats.Center);

AppendPages(document);
Sign(document);

//var filename = PdfFileUtility.GetTempPdfFileName("Rental");
//document.Save(filename);

//using FileStream filestream = AppendPage(filename);
//using var finalDoc = PdfReader.Open(Path.Combine(Path.GetTempPath(), filename), PdfDocumentOpenMode.Modify);


//var cert = new X509Certificate2(@"C:\Test Digital Certificate Password is 123456.pfx", "123456");
////for (var i = 1; i <= 2; i++)
////{
//var options = new DigitalSignatureOptions
//{
//    //Certificate = cert,
//    //FieldName = "Signature-" + Guid.NewGuid().ToString("N"),
//    //PageIndex = 0,
//    //Rectangle = new XRect(120 * i, 40, 100, 60),
//    //Location = "My PC",
//    //Reason = "Approving Rev #" + i,

//    ContactInfo = "SignCare",
//    Location = "India",
//    Reason = "License Agreement",
//    Rectangle = new XRect(120, 40, 100, 60),
//    AppearanceHandler = new DefaultSigner.SignatureAppearanceHandler()

//    // Signature appearances can also consist of an image (Rectangle should be adapted to image's aspect ratio)
//    //Image = XImage.FromFile(@"C:\Data\stamp.png")
//};

//// Specify the URI of a timestamp server if you want a signature with timestamp.
//var pdfSignatureHandler = DigitalSignatureHandler.ForDocument(finalDoc,
//    new PdfSharpDefaultSigner(cert, PdfMessageDigestType.SHA256, new Uri("http://timestamp.apple.com/ts01")),
//    options);



//}

// =====================================
// Part 1 - Create an encrypted PDF file
// =====================================

// Set document encryption.
SecureDoc(document);

var filename = PdfFileUtility.GetTempPdfFileName("Rental");
document.Save(filename);
// Save the document...
//var filename = PdfFileUtility.GetTempPdfFullFileName("samples-PDFsharp/AES128");
//finalDoc.Save(filename);
PdfFileUtility.ShowDocument(filename);




static PdfDocument Sign(PdfDocument doc)
{

    var cert = new X509Certificate2(@"C:\Test Digital Certificate Password is 123456.pfx", "123456");
    var index = 0;
    foreach (PdfPage page in doc.Pages)
    {

        var options = new DigitalSignatureOptions
        {
            //Certificate = cert,
            //FieldName = "Signature-" + Guid.NewGuid().ToString("N"),
            //PageIndex = 0,
            //Rectangle = new XRect(120 * i, 40, 100, 60),
            //Location = "My PC",
            //Reason = "Approving Rev #" + i,

            ContactInfo = "SignCare Address",
            AppName = "SignCare Pro",
            Location = "India",
            Reason = "License Agreement" + index,
            PageIndex = index,
            Rectangle = new XRect(120 * index, 40, 100, 60),
            AppearanceHandler = new DefaultSigner.SignatureAppearanceHandler()

            // Signature appearances can also consist of an image (Rectangle should be adapted to image's aspect ratio)
            //Image = XImage.FromFile(@"C:\Data\stamp.png")
        };
        // Specify the URI of a timestamp server if you want a signature with timestamp.
        var pdfSignatureHandler = DigitalSignatureHandler.ForDocument(doc,
            new PdfSharpDefaultSigner(cert, PdfMessageDigestType.SHA256, new Uri("http://timestamp.apple.com/ts01")),
            options);
        index++;
    }




    return doc;
}



static PdfDocument AppendPages(PdfDocument doc)
{
    //var fs = File.Open(filename, FileMode.Open, FileAccess.ReadWrite);
    //var doc = PdfReader.Open(fs, PdfDocumentOpenMode.Modify);
    //var page2 = doc.AddPage();

    // Draw some text.
    //using var gfx2 = XGraphics.FromPdfPage(page2);
    //var font = new XFont("Times New Roman", 20, XFontStyleEx.BoldItalic);
    //gfx2.DrawString("I am 2nd page", font, XBrushes.Black,
    //    new XRect(0, 0, page2.Width.Point, page2.Height.Point), XStringFormats.Center);



    for (int i = 0; i < 5; i++)
    {
        doc.AddPage();
    }
    var numPages = doc.PageCount;
    var numContentsPerPage = new List<int>();

    foreach (PdfPage page in doc.Pages)
    {
        // remember count of existing contents
        numContentsPerPage.Add(page.Contents.Elements.Count);
        // add new content
        using var gfx = XGraphics.FromPdfPage(page);
        gfx.DrawString("I was added", new XFont("Arial", 16), new XSolidBrush(XColors.Red), 40, 40);
    }
    return doc;

    //doc.Save(fs, true);
    //return fs;
}





static FileStream AppendPage(string filename)
{
    var fs = File.Open(filename, FileMode.Open, FileAccess.ReadWrite);
    var doc = PdfReader.Open(fs, PdfDocumentOpenMode.Modify);
    var page2 = doc.AddPage();

    // Draw some text.
    //using var gfx2 = XGraphics.FromPdfPage(page2);
    //var font = new XFont("Times New Roman", 20, XFontStyleEx.BoldItalic);
    //gfx2.DrawString("I am 2nd page", font, XBrushes.Black,
    //    new XRect(0, 0, page2.Width.Point, page2.Height.Point), XStringFormats.Center);

    var numPages = doc.PageCount;
    var numContentsPerPage = new List<int>();
    foreach (PdfPage page in doc.Pages)
    {
        // remember count of existing contents
        numContentsPerPage.Add(page.Contents.Elements.Count);
        // add new content
        using var gfx = XGraphics.FromPdfPage(page);
        gfx.DrawString("I was added", new XFont("Arial", 16), new XSolidBrush(XColors.Red), 40, 40);
    }

    doc.Save(fs, true);
    return fs;
}

static void SecureDoc(PdfDocument finalDoc)
{
    //document.SecuritySettings.UserPassword = "test1";
    finalDoc.SecuritySettings.OwnerPassword = "ownpwd";

    finalDoc.SecuritySettings.PermitPrint = true;
    finalDoc.SecuritySettings.PermitExtractContent = false;
    finalDoc.SecuritySettings.PermitFormsFill = false;

    //document.SecuritySettings.PermitAccessibilityExtractContent = false;
    finalDoc.SecuritySettings.PermitAnnotations = false;
    finalDoc.SecuritySettings.PermitAssembleDocument = false;
    finalDoc.SecuritySettings.PermitExtractContent = false;
    finalDoc.SecuritySettings.PermitFormsFill = false;
    finalDoc.SecuritySettings.PermitFullQualityPrint = false;
    finalDoc.SecuritySettings.PermitModifyDocument = false;
    finalDoc.SecuritySettings.PermitPrint = false;
    var securityHandler = finalDoc.SecurityHandler;
    securityHandler.SetEncryptionToV2With128Bits();
}




// ===================================
// Part 2 - Open an encrypted PDF file
// ===================================

// Open the PDF document.
// After opening the document with the correct password it is not protected anymore.
// You must set the security handler again to save it encrypted.
//document = PdfReader.Open(filename, userPassword, PdfDocumentOpenMode.Modify);

//// Draw some text on 2nd page.
//page = document.AddPage();
//gfx = XGraphics.FromPdfPage(page);
//gfx.DrawString("2nd page", font, XBrushes.Black,
//    new XRect(0, 0, page.Width.Point, page.Height.Point), XStringFormats.Center);

//// Save the document with new name...
//filename = PdfFileUtility.GetTempPdfFullFileName("samples-PDFsharp/AES128-unprotected");
//document.Save(filename);
//PdfFileUtility.ShowDocument(filename);