using System.Collections.Generic;

namespace ConsoleApplication1
{
    public class LineToken
    {
        public List<IToken> Tokens;

        public LineToken(List<IToken> toks)
        {
            Tokens = toks;
        }
        public LineToken()
        {
            Tokens = new List<IToken>();
        }
    }
}