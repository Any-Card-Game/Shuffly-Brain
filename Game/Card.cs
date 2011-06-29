using System;
using System.Linq;
using System.Text;

namespace ConsoleApplication1.Game
{
    public class Card
    {
        public Card(int number, int type, string cardName)
        {
            CardType = type;
            CardName = cardName;
            CardNumber = number;
        }
        public Card()
        {
        }

        public int CardNumber { get; set; }
        public int CardType{ get; set; }
        public string CardName { get; set; }
    }
}
