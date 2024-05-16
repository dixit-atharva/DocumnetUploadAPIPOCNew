using System.Collections.Generic;

namespace PDFtoImage.Model
{
    public class PDFCoordinates
    {
        public string FileName { get; set; } = string.Empty;
        public List<Pages> Pages { get; set; } = new List<Pages>();
    }

    public class Pages
    {
        public int PageNumber { get; set; }

        public List<Cordinates> cordinate { get; set; } = new List<Cordinates>();
    }

    public class Cordinates
    {
        public float Left { get; set; }

        public float Top { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }
    }
}
