using System;
using System.Linq;
using System.Text;

namespace ConsoleApplication1.Game
{
    public class Card
    {
        public Card(int number, int type) {
            CardType = type;
            CardNumber = number;
        }

        public int CardNumber { get; set; }
        public int CardType{ get; set; }
    }
}
