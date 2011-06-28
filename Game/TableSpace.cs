using System.Collections.Generic;

namespace ConsoleApplication1.Game{
    public class TableSpace
    {
        public bool Visible { get; set; }
        public bool StackCards { get; set; }
        public bool DrawCardsBent { get; set; }
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<Card> Cards { get; set; }

    }
}