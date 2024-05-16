using System.Collections.Generic;

namespace PDFtoImage.Model
{
    internal class PDFCoordinates
    {
        public List<Pages> Pages { get; set; } = new List<Pages>();
    }

    internal class Pages
    {
        public int PageNumber { get; set; }

        public List<Cordinates> cordinate { get; set; } = new List<Cordinates>();
    }

    internal class Cordinates
    {
        public float Left { get; set; }

        public float Top { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }
    }
}
