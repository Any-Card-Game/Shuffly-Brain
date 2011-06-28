using System;
using System.Collections.Generic;

namespace ConsoleApplication1{
    public class SpokeVariableInfo
    {
        public Dictionary<string, SpokeType> Variables;

        public SpokeVariableInfo(SpokeVariableInfo variables)
        {
            if (variables == null)
            {
                allVariables = new List<Tuple<string, int, SpokeType>>();
                Variables = new Dictionary<string, SpokeType>();
                return;
            }

            allVariables = variables.allVariables;
            index = variables.index;
            indeces = variables.indeces;
            Variables = new Dictionary<string, SpokeType>(variables.Variables);
        }
        public SpokeVariableInfo()
        {
            allVariables = new List<Tuple<string, int, SpokeType>>();
            Variables = new Dictionary<string, SpokeType>();
        }

        private Dictionary<string, int> indeces = new Dictionary<string, int>();
        public int index;


        public List<Tuple<string, int, SpokeType>> allVariables;

        public int Add(string s, SpokeType spokeType, SpokeVariable sv)
        {
            allVariables.Add(new Tuple<string, int, SpokeType>(s, index, spokeType));
            Variables.Add(s, spokeType);
            indeces.Add(s, index++);
            if (sv != null)
            {
                sv.VariableIndex = indeces[s];
            }

            return index - 1;
        }
        public void Remove(string s)
        {
            Variables.Remove(s);
            indeces.Remove(s);
        }
        public bool TryGetValue(string s, out SpokeType spokeType, SpokeVariable sv)
        {
            var def = Variables.TryGetValue(s, out spokeType);

            if (def && sv != null)
                sv.VariableIndex = indeces[s];

            return def;
        }

        public SpokeType Get(string variableName, SpokeVariable mv)
        {
            var def = Variables[variableName];
            if (def != null && mv != null)
                mv.VariableIndex = indeces[variableName];

            return def;
        }

        public SpokeType this[string s, SpokeVariable mv]
        {
            get { return Get(s, mv); }
        }

        public void IncreaseState()
        {
        }

        public void DecreaseState()
        {

        }

        public void Reset(string variableName, SpokeType spokeType, SpokeVariable sv)
        {
            if (Variables.ContainsKey(variableName))
            {
                Variables[variableName] = spokeType;
                if (sv != null)
                {
                    sv.VariableIndex = indeces[variableName];
                }
            }
        }

        public int Set(string s, SpokeType spokeType, SpokeVariable sv) {
            if (sv != null)
            {
                sv.VariableIndex = indeces[s];
            }

            return indeces[s];
        }
    }
}