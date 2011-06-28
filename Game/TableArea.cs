using System.Collections.Generic;
using System.Drawing;

namespace ConsoleApplication1.Game{
    public class TableArea
    {
        public int NumberOfCardsHorizontal { get; set; }
        public int NumberOfCardsVertical { get; set; }
        public Rectangle Dimensions { get; set; }
        public List<TableSpace> Spaces { get; set; }
        public List<TableTextArea> TextAreas { get; set; }
    }
}