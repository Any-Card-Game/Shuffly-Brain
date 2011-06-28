using System.Collections.Generic;

namespace ConsoleApplication1.Game{
    public class GameBoard
    {
        public TableArea MainArea { get; set; }
        public List<TableArea> UserAreas { get; set; }

    }
}