﻿using static PDFUtility.Internals.NativeMethods;

namespace PDFUtility.Exceptions
{
    /// <summary>
    /// Thrown if the PDF file uses an unsupported security scheme.
    /// </summary>
    public class PdfUnsupportedSecuritySchemeException : PdfException
    {
        internal override FPDF_ERR Error => FPDF_ERR.SECURITY;

        /// <inheritdoc/>
        public PdfUnsupportedSecuritySchemeException() : base("Unsupported security scheme.") { }
    }
}