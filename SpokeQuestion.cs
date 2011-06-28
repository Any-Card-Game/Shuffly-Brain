using System;
using System.Collections.Generic;

namespace ConsoleApplication1{
    public class SpokeQuestion{
        public string Question;
        public string User;
        public string[] Answers;

        public SpokeQuestion(string stringVal, string s, string[] toArray) {
            Question = s;
            User = stringVal;
            Answers = toArray;
        }
    }
}