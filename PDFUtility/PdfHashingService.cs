using System;
using System.Security.Cryptography;

namespace PDFtoImage
{
    public static class PdfHashingService
    {
        public static string ComputeHash(byte[] fileBytes)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(fileBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool CompareHashes(string hash1, string hash2)
        {
            return hash1 == hash2;
        }
    }
}
