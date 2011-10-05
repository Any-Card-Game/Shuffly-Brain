using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    public class HeartParser
    {
        private string Input;

        public HeartParser(string input)
        {
            Input = input;
        }
        public class PieceTokenizer
        {
            Dictionary<Guid, int> tracking = new Dictionary<Guid, int>();
            private string[] charz;
            private int curIndex = 0;
            public PieceTokenizer(string[] f)
            {
                charz = f;
            }
            public string GetNextPiece()
            {
                return charz[curIndex++];
            }

            public string CurrentPiece(int ind = 0)
            {
                if (charz.Length <= curIndex + ind)
                    return "";
                return charz[curIndex + ind];
            }
            public Guid reStart()
            {
                Guid f;
                tracking.Add(f = Guid.NewGuid(), curIndex);
                return f;
            }
            public void goBack(Guid g)
            {
                curIndex = tracking[g];
                tracking.Remove(g);
            }

            public void Progress()
            {
                curIndex++;
            }

            public bool done()
            {
                return charz.Length == curIndex;
            }
        }

        public class VariableTracker{
            private string[] Variables = new string[1024];
            private int index = 0;
            public string NewVariable()
            {
                return Variables[index++] = newv();
            }

            public string LastVariable()
            {
                return Variables[index--] ;
            }

            private int curC = 0;
            private string newv() {
                return "vvv" + (curC++);
            }
        }

        public void Run()
        {

            var toparse = getParsable().Distinct();
            var s = getToks().ToArray();

            var f = new PieceTokenizer(s);

            D_Document doc;
            if ((doc = makeDocument(f)) == null)
            {
                throw new Exception("bad document");
            }

            StringBuilder classes = new StringBuilder();
            StringBuilder methods = new StringBuilder();

            VariableTracker vt=new VariableTracker();
            foreach (var dItem in doc.Items)
            {
                var v = buildPiece(vt,dItem.Piece, dItem);

                classes.AppendFormat(
                    @"public class D_{0}
        {{
            {1}
        }}", dItem.Word.Value, v.Item1);
                
                methods.AppendFormat(
                    @"
        private D_{0} make{0}(PieceTokenizer f)
        {{
            var g = f.reStart();
            D_{0} e = new D_{0}();
            {1}
        }}",
                    dItem.Word.Value, v.Item2);
                
            }
        }

        private Tuple<string, string> buildPiece(VariableTracker vt, D_Piece piece, D_Item item)
        {
            StringBuilder cls = new StringBuilder();
            StringBuilder method = new StringBuilder();
            Tuple<string, string> v;

            switch (piece.PieceClause)
            {
                case D_Piece.D_PieceClause.PieceWrap:
                    v = buildPieceWrap(vt, piece.PieceWrap, item);
                    cls.Append(v.Item1);
                    method.Append(v.Item2);
                    break;
                case D_Piece.D_PieceClause.PieceKernalOr:
                    v = buildPieceKernalOr(vt, piece.PieceKernalOr, item);
                    cls.Append(v.Item1);
                    method.Append(v.Item2); 
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            return new Tuple<string, string>(cls.ToString(), method.ToString());
        }

        private Tuple<string, string> buildPieceWrap(VariableTracker vt, D_PieceWrap pieceWrap, D_Item item)
        {



            StringBuilder cls = new StringBuilder();
            StringBuilder method = new StringBuilder();
            Tuple<string, string> v;

            if (pieceWrap.HasPieceExtras)
            {
                switch (pieceWrap.PieceExtras.PieceExtrasClause)
                {
                    case D_PieceExtras.D_PieceExtrasClause.QuestionMark:
                        break;
                    case D_PieceExtras.D_PieceExtrasClause.DotDot:
                        method.AppendLine("while(true) {");
                        v = buildPieceInformation(vt, pieceWrap.PieceInformation, item);

                        cls.AppendLine(v.Item1);
                        method.AppendLine(v.Item2);

                        method.AppendLine("}");

                        cls.AppendLine("public List<D_" + item.Word.Value + "> " + item.Word.Value + "s = new List<D_" + item.Word.Value + ">();");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return new Tuple<string, string>(cls.ToString(), method.ToString());
        }

        private Tuple<string, string> buildPieceInformation(VariableTracker vt, D_PieceInformation pieceInformation, D_Item item)
        {
            StringBuilder cls = new StringBuilder();
            StringBuilder method = new StringBuilder();
            Tuple<string, string> v;

            switch (pieceInformation.PieceInformationClause)
            {
                case D_PieceInformation.D_PieceInformationClause.JustPiece:
                    v = buildJustPiece(vt, pieceInformation.JustPiece, item);
                    cls.AppendLine(v.Item1);
                    method.AppendLine(v.Item2);
                    break;
                case D_PieceInformation.D_PieceInformationClause.PieceWithParam:
                    v = buildPieceWithParam(vt, pieceInformation.PieceWithParam, item);
                    cls.AppendLine(v.Item1);
                    method.AppendLine(v.Item2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new Tuple<string, string>(cls.ToString(), method.ToString());
        }

        private Tuple<string, string> buildPieceWithParam(VariableTracker vt, D_PieceWithParam pieceWrap, D_Item item)
        {
            StringBuilder cls = new StringBuilder();
            StringBuilder method = new StringBuilder();
            Tuple<string, string> v;
            v = buildPiece(vt, pieceWrap.Piece1, item);
            cls.AppendLine(v.Item1);
            method.AppendLine(v.Item2);


            v = buildPiece(vt, pieceWrap.Piece2, item);
            cls.AppendLine(v.Item1);
            method.AppendLine(v.Item2 + "conitnue; else break;");


            return new Tuple<string, string>(cls.ToString(), method.ToString());
        }

        private Tuple<string, string> buildJustPiece(VariableTracker vt, D_JustPiece justPiece, D_Item item)
        {
            StringBuilder cls = new StringBuilder();
            StringBuilder method = new StringBuilder();
            Tuple<string, string> v = buildPiece(vt, justPiece.Piece, item);
            cls.AppendLine(v.Item1);
            method.AppendLine(v.Item2);

            return new Tuple<string, string>(cls.ToString(), method.ToString());
        }

        private Tuple<string, string> buildPieceKernalOr(VariableTracker vt, D_PieceKernalOr pieceWrap, D_Item item)
        {
            StringBuilder cls = new StringBuilder();
            StringBuilder method = new StringBuilder();
            Tuple<string, string> v;
            foreach (var addition in pieceWrap.PieceKernalAdditions)
            {
                v = buildPieceKernalAddition(vt, addition, item);
                cls.AppendLine(v.Item1);
                method.AppendLine(v.Item2);
            } 

            return new Tuple<string, string>(cls.ToString(), method.ToString());
        }

        private Tuple<string, string> buildPieceKernalAddition(VariableTracker vt, D_PieceKernalAddition pieceWrap, D_Item item)
        {
            StringBuilder cls = new StringBuilder();
            StringBuilder method = new StringBuilder();
            Tuple<string, string> v;
            foreach (var contents in pieceWrap.PieceContents)
            {
                v = buildPieceContents(vt, contents, item);
                cls.AppendLine(v.Item1);
                method.AppendLine(v.Item2);
            }
            for (int i = 0; i < pieceWrap.PieceContents.Count; i++) {
                method.AppendLine("}");
            }
            return new Tuple<string, string>(cls.ToString(), method.ToString());
        }
        private Tuple<string, string> buildPieceContents(VariableTracker vt, D_PieceContent pieceWrap, D_Item item)
        {
            StringBuilder cls = new StringBuilder();
            StringBuilder method = new StringBuilder();
            Tuple<string, string> v;

            switch (pieceWrap.PieceContentClause) {
                case D_PieceContent.D_PieceContentClause.Word:

                    var d = vt.NewVariable();
                    method.AppendLine("D_" + pieceWrap.Word.Value + " "+d+";\r\n if(("+d+" = make" + pieceWrap.Word.Value + "(f))!=null){");
                    break;
                case D_PieceContent.D_PieceContentClause.String:
                    method.AppendLine("if(isToken(f,\""+pieceWrap.String.Value+"\")){");
                    break;
                case D_PieceContent.D_PieceContentClause.Piece:

                    v = buildPiece(vt, pieceWrap.Piece, item);
                    method.AppendLine("{");
                    cls.AppendLine(v.Item1);
                    method.AppendLine(v.Item2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new Tuple<string, string>(cls.ToString(), method.ToString());
        }

        public class D_Document
        {
            public List<D_Item> Items = new List<D_Item>();
        }
        public class D_Item
        {
            public D_Word Word;
            public D_Piece Piece;
        }
        public class D_Piece
        {
            public enum D_PieceClause
            {
                PieceWrap, PieceKernalOr
            }
            public D_PieceClause PieceClause;
            public D_PieceWrap PieceWrap;
            public D_PieceKernalOr PieceKernalOr;
        }
        public class D_PieceWrap
        {
            public D_PieceInformation PieceInformation;
            public bool HasPieceExtras;
            public D_PieceExtras PieceExtras;

        }
        public class D_PieceExtras
        {

            public enum D_PieceExtrasClause
            {
                QuestionMark, DotDot
            }
            public D_PieceExtrasClause PieceExtrasClause;
        }
        public class D_PieceInformation
        {
            public enum D_PieceInformationClause
            {
                JustPiece, PieceWithParam
            }
            public D_PieceInformationClause PieceInformationClause;

            public D_JustPiece JustPiece;
            public D_PieceWithParam PieceWithParam;
        }
        public class D_JustPiece
        {
            public D_Piece Piece;
        }
        public class D_PieceWithParam
        {
            public D_Piece Piece1; public D_Piece Piece2;
        }
        public class D_PieceKernalOr
        {
            public List<D_PieceKernalAddition> PieceKernalAdditions = new List<D_PieceKernalAddition>();
        }
        public class D_PieceKernalAddition
        {
            public List<D_PieceContent> PieceContents = new List<D_PieceContent>();
        }
        public class D_PieceContent
        {
            public enum D_PieceContentClause
            {
                Word, String, Piece
            }
            public D_PieceContentClause PieceContentClause;

            public D_Word Word;
            public D_String String;
            public D_Piece Piece;
        }

        private D_Document makeDocument(PieceTokenizer f)
        {
            var g = f.reStart();
            D_Document doc = new D_Document();
            while (true)
            {
                D_Item c;
                if ((c = makeItem(f)) != null)
                {
                    doc.Items.Add(c);
                    if (isToken(f, "\n"))
                        continue;
                    else continue;
                    return doc;
                }
                else
                    return doc;
            }
        }

        private D_Item makeItem(PieceTokenizer f)
        {
            var g = f.reStart();
            D_Item item = new D_Item();
            string w1;
            if ((w1 = getWord(f)) != null)
            {
                item.Word = new D_Word();
                item.Word.Value = w1;

                if (isToken(f, "="))
                {
                    if ((item.Piece = makePiece(f)) != null)
                    {
                        return item;
                    }
                }
            }
            f.goBack(g);
            return null;
        }

        private D_Piece makePiece(PieceTokenizer f)
        {
            var g = f.reStart();

            D_Piece piece = new D_Piece();
            D_PieceWrap d; D_PieceKernalOr c;
            if ((d = makePieceWrap(f)) != null)
            {
                piece.PieceClause = D_Piece.D_PieceClause.PieceWrap;
                piece.PieceWrap = d;
            }
            else if ((c = makePieceKernalOr(f)) != null)
            {
                piece.PieceClause = D_Piece.D_PieceClause.PieceKernalOr;
                piece.PieceKernalOr = c;
            }
            else return null;
            return piece;

        }
        private D_PieceWrap makePieceWrap(PieceTokenizer f)
        {
            var g = f.reStart();

            D_PieceWrap piece = new D_PieceWrap();
            D_PieceInformation c;
            D_PieceExtras d;
            if ((c = makePieceInformation(f)) != null)
            {
                piece.PieceInformation = c;
                if ((d = makePieceExtras(f)) != null)
                {
                    piece.HasPieceExtras = true;
                    piece.PieceExtras = d;
                }
                else
                    piece.HasPieceExtras = false;

            }
            else
            {
                f.goBack(g);
                return null;
            }
            return piece;
        }
        private D_PieceInformation makePieceInformation(PieceTokenizer f)
        {
            var g = f.reStart();
            D_JustPiece c;
            D_PieceWithParam d;
            D_PieceInformation piece = new D_PieceInformation();
            if ((c = makeJustPiece(f)) != null)
            {
                piece.PieceInformationClause = D_PieceInformation.D_PieceInformationClause.JustPiece;
                piece.JustPiece = c;
            }
            else if ((d = makePieceWithParam(f)) != null)
            {
                piece.PieceInformationClause = D_PieceInformation.D_PieceInformationClause.PieceWithParam;
                piece.PieceWithParam = d;
            }
            else
            {
                f.goBack(g);
                return null;
            }
            return piece;

        }
        private D_JustPiece makeJustPiece(PieceTokenizer f)
        {
            var g = f.reStart();

            D_JustPiece piece = new D_JustPiece();
            D_Piece c;
            if (isToken(f, "("))
            {
                if ((c = makePiece(f)) != null)
                {
                    piece.Piece = c;
                    if (isToken(f, ")"))
                    {
                        return piece;
                    }
                }
            }
            f.goBack(g);
            return null;
        }
        private D_PieceWithParam makePieceWithParam(PieceTokenizer f)
        {
            var g = f.reStart();

            D_PieceWithParam piece = new D_PieceWithParam();
            D_Piece c;
            D_Piece d;
            if (isToken(f, "("))
            {
                if ((c = makePiece(f)) != null)
                {
                    piece.Piece1 = c;
                    if (isToken(f, ","))
                    {
                        if ((d = makePiece(f)) != null)
                        {
                            piece.Piece2 = d;
                            if (isToken(f, ")"))
                            {
                                return piece;
                            }
                        }
                    }
                }
            }
            f.goBack(g);
            return null;
        }

        private D_PieceExtras makePieceExtras(PieceTokenizer f)
        {
            var g = f.reStart();

            D_PieceExtras piece = new D_PieceExtras();
            if (isToken(f, "?"))
            {
                piece.PieceExtrasClause = D_PieceExtras.D_PieceExtrasClause.QuestionMark;
            }
            else if (isToken(f, ".") && isToken(f, "."))
            {
                piece.PieceExtrasClause = D_PieceExtras.D_PieceExtrasClause.DotDot;
            }
            else
            {
                f.goBack(g);
                return null;
            }
            return piece;
        }

        private D_PieceKernalOr makePieceKernalOr(PieceTokenizer f)
        {
            var g = f.reStart();

            D_PieceKernalOr piece = new D_PieceKernalOr();

            while (true)
            {
                D_PieceKernalAddition c;
                if ((c = makePieceKernalAddition(f)) != null)
                {
                    piece.PieceKernalAdditions.Add(c);
                    if (isToken(f, "|"))
                    {
                        continue;
                    }
                    return piece;
                }
                else
                {
                    f.goBack(g);
                    return null;
                }
            }


        }
        private D_PieceKernalAddition makePieceKernalAddition(PieceTokenizer f)
        {
            var g = f.reStart();

            D_PieceKernalAddition piece = new D_PieceKernalAddition();

            while (true)
            {
                D_PieceContent c;
                if ((c = makePieceContent(f)) != null)
                {
                    piece.PieceContents.Add(c);
                    if (isToken(f, "+"))
                    {
                        continue;
                    }
                    return piece;
                }
                else
                {
                    f.goBack(g);
                    return null;
                }
            }


        }

        private D_PieceContent makePieceContent(PieceTokenizer f)
        {
            var g = f.reStart();

            D_PieceContent piece = new D_PieceContent();
            D_Piece c;

            string w1;
            if ((w1 = getWord(f)) != null)
            {
                piece.PieceContentClause = D_PieceContent.D_PieceContentClause.Word;
                piece.Word = new D_Word();
                piece.Word.Value = w1;
            }
            else if (isString(f))
            {
                piece.PieceContentClause = D_PieceContent.D_PieceContentClause.String;
                piece.String = new D_String();
                piece.String.Value = f.CurrentPiece();
            }
            else if ((c = makePiece(f)) != null)
            {
                piece.PieceContentClause = D_PieceContent.D_PieceContentClause.Piece;
                piece.Piece = c;
            }
            else
            {
                f.goBack(g);
                return null;
            }
            return piece;
        }

        public class D_Word
        {
            public string Value;
        }
        public class D_String
        {
            public string Value;
        }


        public bool isBool(PieceTokenizer f, bool advance = true)
        {
            bool d;
            bool fc = bool.TryParse(f.CurrentPiece(), out d);
            if (fc && advance)
                f.Progress();
            return fc;
        }

        public bool isString(PieceTokenizer f, bool advance = true)
        {


            bool fc = f.CurrentPiece().StartsWith("\"") && f.CurrentPiece().EndsWith("\"");
            if (fc && advance)
                f.Progress();
            return fc;

        }
        public bool isNumber(PieceTokenizer f, bool advance = true)
        {
            int d;
            bool fc = int.TryParse(f.CurrentPiece(), out d);
            if (fc && advance)
                f.Progress();
            return fc;

        }
        public bool isToken(PieceTokenizer f, string tok, bool advance = true)
        {
            int d;
            bool fc = f.CurrentPiece() == tok;
            if (fc && advance)
                f.Progress();
            return fc;

        }
        private bool isAToken(PieceTokenizer f, bool advance = true)
        {
            bool j = isToken(f, "+", false) || isToken(f, "(", false) || isToken(f, ")", false) || isToken(f, "|", false) || isToken(f, "?", false);
            if (j)
            {
                if (advance)
                    f.Progress();
            }
            return j;
        }

        public string getWord(PieceTokenizer f, bool advance = true)
        {
            bool j = !isAToken(f, false) && !isBool(f, false) && !isNumber(f, false) && !isString(f, false);
            if (j)
            {
                string d;
                Console.WriteLine(d = f.CurrentPiece());
                if (advance)
                    f.Progress();
                return d;
            }
            return null;
        }

        public IEnumerable<string> getToks()
        {
            string cur = "";
            char[] toks = new char[] { '.', '=', '+', '|', ')', '(', ',' };
            char lastTok = ' ';
            bool wasNewLine = false;
            bool inStr = false;
            foreach (var v in Input)
            {
                if (!inStr && toks.Contains(v))
                {
                    if (cur != "") yield return cur;
                    cur = "";
                    lastTok = v;
                    yield return v.ToString();
                    continue;
                }

                if (v == '\"')
                {
                    inStr = !inStr;
                }

                if (v == ' ' || v == '\t')
                {
                    if (cur == "")
                        continue;
                    else
                    {
                        yield return cur;
                        cur = "";
                        continue;
                    }
                }

                else if (v == '\r')
                {
                    if (cur != "")
                    {
                        yield return cur;
                    }

                    cur = v.ToString();
                    continue;
                }
                else
                {
                    cur += v;
                }



                if (cur == "\r\n")
                {
                    if (lastTok == '|')
                    {
                        cur = "";
                        continue;
                    }
                    else
                    {
                        if (!wasNewLine)
                        {
                            yield return "\n";
                        }
                        wasNewLine = true;
                        cur = "";
                        continue;
                    }
                }
                else wasNewLine = false;

            }


        }

        private string[] getParsable()
        {
            var sts = new List<string>();
            sts.Add("\t");
            sts.Add("\r\n");

            int inString = -1;
            for (int i = 0; i < Input.Length; i++)
            {
                if (Input[i] == '\"')
                {
                    if (inString == -1)
                    {
                        inString = i + 1;
                    }
                    else
                    {
                        sts.Add(Input.Substring(inString, (i - inString)));
                        inString = -1;
                    }
                }
            }
            return sts.ToArray();
        }

        public class PToken
        {
            public PTokenType Type;
        }
    }

    public enum PTokenType { }
}




/*
     

        public bool isItem(PieceTokenizer f, bool advance = true)
        {
            var g = f.reStart();
            bool good = false;
            if (isWord(f))
            {
                if (isToken(f, "="))
                {
                    if (isPiece(f))
                    {
                        good = true;
                    }

                }
            }
            if (good)
            {
            }
            else
            {
                f.goBack(g);
            }
            return good;
        }

        private bool isPiece(PieceTokenizer f, bool advance = true)
        {
            Console.WriteLine(f.CurrentPiece());
            var g = f.reStart();
            bool good = false;


            if ((isPieceOr1(f) || isPieceOr2(f)) && (isToken(f, "?") || (isToken(f, ".") && isToken(f, ".")) || true))
            {
                good = true;
            }

            if (!good || isToken(f, "+") || isToken(f, "|"))
            {
                while (true)
                {
                    good = false;
                    bool g1 = false;
                    while (true)
                    {
                        g1 = false;
                        if (isWord(f) || isString(f) || isPiece(f))
                        {
                            g1 = true;
                            if (isToken(f, "+"))
                            {
                                continue;
                            }

                            else break;
                        }
                        else break;
                    }


                    if (g1)
                    {
                        good = true;
                        if (isToken(f, "|"))
                        {
                            continue;
                        }
                        else break;
                    }
                    else break;
                }



            }

            if (good)
            {

            }
            else
                f.goBack(g);
            return good;
        }


        private bool isPieceOr1(PieceTokenizer f, bool advance = true)
        {
            var g = f.reStart();
            bool good = false;

            if (isToken(f, "("))
                if (isPiece(f))
                    if (isToken(f, ")"))
                        good = true;
            if (good)
            {

            }
            else
                f.goBack(g);
            return good;
        }
        private bool isPieceOr2(PieceTokenizer f, bool advance = true)
        {
            var g = f.reStart();
            bool good = false;

            if (isToken(f, "("))
                if (isPiece(f))
                    if (isToken(f, ","))
                        if (isPiece(f))
                            if (isToken(f, ")"))
                                good = true;
            if (good)
            {

            }
            else
                f.goBack(g);
            return good;
        }
*/