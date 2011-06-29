//#define stacktrace
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ConsoleApplication1.Game;

namespace ConsoleApplication1
{
    public class RunInstructions
    {
        private SpokeMethod[] Methods;
        private readonly Dictionary<string, string[]> myVariableLookup;
        private Func<SpokeObject[], SpokeObject>[] InternalMethods;


        private SpokeObject[] ints;
        private SpokeObject NULL = new SpokeObject(ObjectType.Null);


        private SpokeObject TRUE = new SpokeObject(ObjectType.Bool) { BoolVal = true };
        private SpokeObject FALSE = new SpokeObject(ObjectType.Bool) { BoolVal = false };

        public SpokeObject getVariableByName(SpokeObject so, string name)
        {
            return so.Variables[Array.IndexOf(myVariableLookup[so.ClassName], name)];
        }

        public RunInstructions(Func<SpokeObject[], SpokeObject>[] internalMethods, SpokeMethod[] mets, string stack, int returnIndex, Dictionary<string, string[]> variableLookup)
        {
            Methods = mets;
            myVariableLookup = variableLookup;
            InternalMethods = internalMethods;
            ints = new SpokeObject[100];
            for (int i = 0; i < 100; i++)
            {
                ints[i] = new SpokeObject(ObjectType.Int) { IntVal = i };
            }

            if (stack.Length != 0)
            {
                deserialize(stack);
                reprintStackTrace[reprintStackIndex - 1].Answer = new SpokeObject(returnIndex);
            }


        }

        private static List<StackTracer> stf;

        private string serialize()
        {
            XmlSerializer sr = new XmlSerializer(typeof(List<StackTracer>));
            StringWriter sw;

            stf = stackTrace;
            
            var fs = new fixStackTracer(stackTrace.ToArray());
            stackTrace = new List<StackTracer>(fs.Start(true));

            sr.Serialize(sw = new StringWriter(), stackTrace);
            return sw.ToString();
        }
        private void deserialize(string sz)
        {
            XmlSerializer sr = new XmlSerializer(typeof(List<StackTracer>));
            List<StackTracer> f = (List<StackTracer>)sr.Deserialize(new StringReader(sz));
            reprintStackTrace = f.ToArray();
            reprintStackIndex = reprintStackTrace.Length;

            var fs = new fixStackTracer(reprintStackTrace);
            reprintStackTrace = fs.Start(false);

            reprintStackTrace=stf.ToArray();


        }

        class fixStackTracer
        {
            private readonly StackTracer[] myTc;
            Dictionary<int, SpokeObject> ids = new Dictionary<int, SpokeObject>();
            public fixStackTracer(StackTracer[] tc)
            {
                myTc = tc;
            }

            public StackTracer[] Start(bool d)
            {

                foreach (var stackTracer in myTc)
                {
                    for (int index = 0; index < stackTracer.StackObjects.Length; index++)
                    {
                        stackTracer.StackObjects[index] = getAllVariables(stackTracer.StackObjects[index], d);
                    }
                }


                foreach (var stackTracer in myTc)
                {
                    for (int index = 0; index < stackTracer.StackObjects.Length; index++)
                    {
                        stackTracer.StackObjects[index] = getAllVariables(stackTracer.StackObjects[index], !d);
                    }
                }


                return myTc;
            }

            private SpokeObject getAllVariables(SpokeObject so, bool check)
            {
                if (so == null) return null;

                if (so.ArrayItems != null)
                    for (int index = 0; index < so.ArrayItems.Count; index++)
                    {
                        so.ArrayItems[index] = getAllVariables(so.ArrayItems[index], check);
                    }
                if (so.Variables != null)
                    for (int index = 0; index < so.Variables.Length; index++)
                    {

                        so.Variables[index] = getAllVariables(so.Variables[index], check);
                    }
                so = this.check(so, check);
                return so;
            }

            private SpokeObject check(SpokeObject so, bool check)
            {
                SpokeObject f;
                if (ids.TryGetValue(so.ID, out f))
                {
                    if (check)
                    {
                        if (f.Equals(so))
                        {
                            return f;
                        }
                        else
                        {
                            return so;
                        }
                    }

                    if (!so.Compare(f))
                        return f;
                    else
                    {

                        if (so.ArrayItems != null)
                        {
                            if (so.ArrayItems.Equals(f.ArrayItems))
                            {
                                return f;
                            }
                            else {
                                so.ArrayItems = f.ArrayItems;
                                return f;
                            }
                        }
                        if (so.Variables != null)
                        {
                            if (so.Variables.Equals(f.Variables))
                            {
                                return f;
                            }
                            else
                            {
                                so.Variables = f.Variables;
                                return f;
                            }
                        }
                        return f;
                    }
                }
                ids.Add(so.ID, so);
                return so;
            }
        }


        public Tuple<SpokeQuestion, string, GameBoard> Run(SpokeConstruct so)
        {
            var fm = Methods[so.MethodIndex];
            SpokeObject dm = new SpokeObject(new SpokeObject[so.NumOfVars], "Main");
            for (int index = 0; index < so.NumOfVars; index++)
            {
                dm.SetVariable(index, new SpokeObject());
            }
            try
            {
                evaluateMethod(fm, new SpokeObject[1] { dm });
            }
            catch (AskQuestionException aq)
            {
                return new Tuple<SpokeQuestion, string, GameBoard>(aq.Question, serialize(), aq.GameBoard);
            }

#if stacktrace
			
			catch (Exception er) {
				dfss.AppendLine(er.ToString());
				File.WriteAllText("C:\\ded.txt", dfss.ToString());
				throw er;
			}
#endif

            return null;


            // evaluate(fm, dm, new List<SpokeObject>() { dm });
        }
        private SpokeObject intCache(int index)
        {
            if (index > 0 && index < 100)
            {
                return ints[index];
            }
            return new SpokeObject(ObjectType.Int) { IntVal = index };
        }
        [Serializable]
        public class StackTracer
        {
            public SpokeObject[] StackObjects;
            public SpokeObject[] StackVariables;
            public int InstructionIndex;
            public int StackIndex;
            public SpokeObject Answer;

            public StackTracer(SpokeObject[] stack, SpokeObject[] variables)
            {
                StackObjects = stack;
                StackVariables = variables;
            }
            public StackTracer()
            {
            }
        }
        private List<StackTracer> stackTrace = new List<StackTracer>();
        public int reprintStackIndex = -1;
        public StackTracer[] reprintStackTrace = new StackTracer[0];

#if stacktrace
		private StringBuilder dfss = new StringBuilder();
#endif




        private SpokeObject evaluateMethod(SpokeMethod fm, SpokeObject[] paras)
        {

            SpokeObject[] variables;



#if stacktrace
			dfss.AppendLine( fm.Class.Name +" : : "+fm.MethodName+" Start");
#endif

            SpokeObject lastStack;
            int stackIndex = 0;
            SpokeObject[] stack = new SpokeObject[3000];
            int index = 0;
            StackTracer st;
            if (reprintStackIndex == -1)
            {
                variables = new SpokeObject[fm.NumOfVars];

                for (int i = 0; i < fm.Parameters.Length; i++)
                {
                    variables[i] = paras[i];
                }

                stackTrace.Add(st = new StackTracer(stack, variables));
            }
            else
            {
                StackTracer rp = reprintStackTrace[stackTrace.Count];
                stack = rp.StackObjects;
                variables = rp.StackVariables;
                index = rp.InstructionIndex;
                stackIndex = rp.StackIndex;
                stackTrace.Add(st = new StackTracer(stack, variables));
                if (stackTrace.Count == reprintStackIndex)
                {
                    stack[stackIndex++] = rp.Answer;
                    reprintStackIndex = -1;
                    reprintStackTrace = null;
                }
            }

            for (; index < fm.Instructions.Length; index++)
            {
                var ins = fm.Instructions[index];
#if stacktrace
				dfss.AppendLine(stackIndex + " ::  " + ins.ToString());
#endif


                //        var fs = new fixStackTracer(stackTrace.ToArray());
                //       stackTrace = new List<StackTracer>(fs.Start(true));


                SpokeObject[] sps;

                SpokeObject bm;
                switch (ins.Type)
                {
                    case SpokeInstructionType.CreateReference:
                        stack[stackIndex++] = new SpokeObject(new SpokeObject[ins.Index], ins.StringVal);
                        break;
                    case SpokeInstructionType.CreateArray:
                        stack[stackIndex++] = new SpokeObject(new List<SpokeObject>(20));
                        break;
                    case SpokeInstructionType.CreateMethod:
                        stack[stackIndex++] = NULL;//new SpokeObject(ObjectType.Method) { AnonMethod = ins.anonMethod };
                        break;
                    case SpokeInstructionType.Label:
                        //   throw new NotImplementedException("");
                        break;
                    case SpokeInstructionType.Goto:

                        index = ins.Index;

                        break;
                    case SpokeInstructionType.Comment:


                        break;
                    case SpokeInstructionType.CallMethod:
                        st.InstructionIndex = index;
                        st.StackIndex = stackIndex;
                        sps = new SpokeObject[ins.Index3];
                        for (int i = ins.Index3 - 1; i >= 0; i--)
                        {
                            sps[i] = stack[--stackIndex];
                        }
                        stack[stackIndex++] = evaluateMethod(Methods[ins.Index], sps);
                        break;
                    case SpokeInstructionType.CallMethodFunc:
                        st.InstructionIndex = index;
                        st.StackIndex = stackIndex;
                        sps = new SpokeObject[ins.Index3];
                        for (int i = ins.Index3 - 1; i >= 0; i--)
                        {
                            sps[i] = stack[--stackIndex];
                        }

                        stack[stackIndex++] = Methods[ins.Index].MethodFunc(sps);
                        break;
                    case SpokeInstructionType.CallInternal:
                        sps = new SpokeObject[ins.Index3];
                        for (int i = ins.Index3 - 1; i >= 0; i--)
                        {
                            sps[i] = stack[--stackIndex];
                        }
                        st.InstructionIndex = index + 1;
                        st.StackIndex = stackIndex;

                        SpokeObject l = InternalMethods[ins.Index](sps);

                        if (ins.Index == 16)
                        {
                            GameBoard d = buildBoard(sps[4]);


                            throw new AskQuestionException(stackTrace,
                                                           new SpokeQuestion(sps[1].Variables[0].StringVal, sps[2].StringVal,
                                                                             sps[3].ArrayItems.Select(a => a.StringVal).ToArray()), d);
                        }
                        else
                        {
                            stack[stackIndex++] = l;
                        }

                        break;
                    case SpokeInstructionType.BreakpointInstruction:
                        Console.WriteLine("BreakPoint");
                        break;
                    case SpokeInstructionType.Return:
#if stacktrace
						dfss.AppendLine(fm.Class.Name + " : : " + fm.MethodName + " End");
#endif
                        stackTrace.Remove(st);
                        return stack[--stackIndex];
                        break;
                    case SpokeInstructionType.IfTrueContinueElse:

                        if (stack[--stackIndex].BoolVal)
                            continue;

                        index = ins.Index;

                        break;
                    case SpokeInstructionType.Or:
                        stack[stackIndex - 2] = (stack[stackIndex - 2].BoolVal || stack[stackIndex - 1].BoolVal) ? TRUE : FALSE;
                        stackIndex--;
                        break;
                    case SpokeInstructionType.And:
                        stack[stackIndex - 2] = (stack[stackIndex - 2].BoolVal && stack[stackIndex - 1].BoolVal) ? TRUE : FALSE;
                        stackIndex--;
                        break;
                    case SpokeInstructionType.StoreLocalInt:
                        lastStack = stack[--stackIndex];
                        bm = variables[ins.Index];
                        variables[ins.Index] = new SpokeObject(ObjectType.Int) { IntVal = lastStack.IntVal };
                        break;
                    case SpokeInstructionType.StoreLocalFloat:
                        lastStack = stack[--stackIndex];
                        bm = variables[ins.Index];
                        variables[ins.Index] = new SpokeObject(ObjectType.Float) { FloatVal = lastStack.FloatVal };
                        break;
                    case SpokeInstructionType.StoreLocalBool:
                        lastStack = stack[--stackIndex];
                        bm = variables[ins.Index];
                        variables[ins.Index] = lastStack.BoolVal ? TRUE : FALSE;
                        break;
                    case SpokeInstructionType.StoreLocalString:
                        lastStack = stack[--stackIndex];
                        bm = variables[ins.Index];
                        variables[ins.Index] = new SpokeObject(ObjectType.String) { StringVal = lastStack.StringVal };
                        break;

                    case SpokeInstructionType.StoreLocalMethod:
                    case SpokeInstructionType.StoreLocalObject:
                        lastStack = stack[--stackIndex];
                        variables[ins.Index] = lastStack;
                        break;
                    case SpokeInstructionType.StoreLocalRef:
                        lastStack = stack[--stackIndex];
                        bm = variables[ins.Index];
                        bm.ClassName = lastStack.ClassName;
                        bm.Type = lastStack.Type;
                        //bm.Variables = lastStack.Variables;
                        //bm.ArrayItems = lastStack.ArrayItems;
                        bm.StringVal = lastStack.StringVal;
                        bm.IntVal = lastStack.IntVal;
                        bm.BoolVal = lastStack.BoolVal;
                        bm.FloatVal = lastStack.FloatVal;
                        break;



                    case SpokeInstructionType.StoreFieldBool:
                        lastStack = stack[--stackIndex];
                        lastStack.Variables[ins.Index] = stack[--stackIndex].BoolVal ? TRUE : FALSE;

                        break;
                    case SpokeInstructionType.StoreFieldInt:
                        lastStack = stack[--stackIndex];
                        lastStack.Variables[ins.Index] = new SpokeObject(ObjectType.Int) { IntVal = stack[--stackIndex].IntVal };

                        break;
                    case SpokeInstructionType.StoreFieldFloat:
                        lastStack = stack[--stackIndex];
                        lastStack.Variables[ins.Index] = new SpokeObject(ObjectType.Float) { FloatVal = stack[--stackIndex].FloatVal };

                        break;
                    case SpokeInstructionType.StoreFieldString:
                        lastStack = stack[--stackIndex];
                        lastStack.Variables[ins.Index] = new SpokeObject(ObjectType.String) { StringVal = stack[--stackIndex].StringVal };
                        break;

                    case SpokeInstructionType.StoreFieldMethod:
                    case SpokeInstructionType.StoreFieldObject:
                        lastStack = stack[--stackIndex];
                        lastStack.Variables[ins.Index] = stack[--stackIndex];

                        break;

                    case SpokeInstructionType.IfEqualsContinueAndPopElseGoto:

                        if (SpokeObject.Compare(stack[stackIndex - 2],
                                stack[stackIndex - 1]))
                        {
                            stackIndex = stackIndex - 2;
                            continue;
                        }

                        stackIndex = stackIndex - 1;
                        index = ins.Index;
                        break;
                    case SpokeInstructionType.StoreToReference:

                        lastStack = stack[--stackIndex];
                        stack[stackIndex - 1].Variables[ins.Index] = lastStack;
                        break;
                    case SpokeInstructionType.GetField:

                        stack[stackIndex - 1] = stack[stackIndex - 1].Variables[ins.Index];

                        break;
                    case SpokeInstructionType.GetLocal:

                        stack[stackIndex++] = variables[ins.Index];
                        break;
                    case SpokeInstructionType.PopStack:
                        stackIndex--;
                        break;
                    case SpokeInstructionType.Not:
                        stack[stackIndex - 1] = stack[stackIndex - 1].BoolVal ? FALSE : TRUE;
                        break;
                    case SpokeInstructionType.AddStringInt:
                        stack[stackIndex - 2] = new SpokeObject(ObjectType.String) { StringVal = stack[stackIndex - 2].StringVal + stack[stackIndex - 1].IntVal };
                        stackIndex--;

                        break;
                    case SpokeInstructionType.AddIntString:

                        stack[stackIndex - 2] = new SpokeObject(ObjectType.String) { StringVal = stack[stackIndex - 2].IntVal + stack[stackIndex - 1].StringVal };
                        stackIndex--;
                        break;
                    case SpokeInstructionType.IntConstant:
                        stack[stackIndex++] = intCache(ins.Index);
                        break;
                    case SpokeInstructionType.BoolConstant:

                        stack[stackIndex++] = ins.BoolVal ? TRUE : FALSE;
                        break;
                    case SpokeInstructionType.FloatConstant:
                        stack[stackIndex++] = new SpokeObject(ObjectType.Float) { FloatVal = ins.FloatVal };
                        break;
                    case SpokeInstructionType.StringConstant:

                        stack[stackIndex++] = new SpokeObject(ObjectType.String) { StringVal = ins.StringVal };
                        break;

                    case SpokeInstructionType.Null:
                        stack[stackIndex++] = NULL;
                        break;
                    case SpokeInstructionType.AddIntInt:
                        stack[stackIndex - 2] = intCache(stack[stackIndex - 2].IntVal + stack[stackIndex - 1].IntVal);
                        stackIndex--;
                        break;
                    case SpokeInstructionType.AddIntFloat:
                        break;
                    case SpokeInstructionType.AddFloatInt:
                        break;
                    case SpokeInstructionType.AddFloatFloat:
                        break;
                    case SpokeInstructionType.AddFloatString:
                        stack[stackIndex - 2] = new SpokeObject(ObjectType.String) { StringVal = stack[stackIndex - 2].FloatVal + stack[stackIndex - 1].StringVal };
                        stackIndex--;
                        break;
                    case SpokeInstructionType.AddStringFloat:
                        stack[stackIndex - 2] = new SpokeObject(ObjectType.String) { StringVal = stack[stackIndex - 2].StringVal + stack[stackIndex - 1].FloatVal };
                        stackIndex--;
                        break;
                    case SpokeInstructionType.AddStringString:
                        stack[stackIndex - 2] = new SpokeObject(ObjectType.String) { StringVal = stack[stackIndex - 2].StringVal + stack[stackIndex - 1].StringVal };
                        stackIndex--;
                        break;
                    case SpokeInstructionType.SubtractIntInt:
                        stack[stackIndex - 2] = intCache(stack[stackIndex - 2].IntVal - stack[stackIndex - 1].IntVal);
                        stackIndex--;

                        break;
                    case SpokeInstructionType.SubtractIntFloat:
                        break;
                    case SpokeInstructionType.SubtractFloatInt:
                        break;
                    case SpokeInstructionType.SubtractFloatFloat:
                        break;
                    case SpokeInstructionType.MultiplyIntInt:

                        stack[stackIndex - 2] = intCache(stack[stackIndex - 2].IntVal * stack[stackIndex - 1].IntVal);
                        stackIndex--;
                        break;
                    case SpokeInstructionType.MultiplyIntFloat:
                        break;
                    case SpokeInstructionType.MultiplyFloatInt:
                        break;
                    case SpokeInstructionType.MultiplyFloatFloat:
                        break;
                    case SpokeInstructionType.DivideIntInt:

                        stack[stackIndex - 2] = intCache(stack[stackIndex - 2].IntVal / stack[stackIndex - 1].IntVal);
                        stackIndex--;
                        break;
                    case SpokeInstructionType.DivideIntFloat:
                        break;
                    case SpokeInstructionType.DivideFloatInt:
                        break;
                    case SpokeInstructionType.DivideFloatFloat:
                        break;

                    case SpokeInstructionType.GreaterIntInt:
                        stack[stackIndex - 2] = (stack[stackIndex - 2].IntVal > stack[stackIndex - 1].IntVal) ? TRUE : FALSE;
                        stackIndex = stackIndex - 1;
                        break;
                    case SpokeInstructionType.GreaterIntFloat:
                        stack[stackIndex - 2] = (stack[stackIndex - 2].IntVal > stack[stackIndex - 1].FloatVal) ? TRUE : FALSE;
                        stackIndex = stackIndex - 1;
                        break;
                    case SpokeInstructionType.GreaterFloatInt:
                        stack[stackIndex - 2] = (stack[stackIndex - 2].FloatVal > stack[stackIndex - 1].IntVal) ? TRUE : FALSE;
                        stackIndex = stackIndex - 1;
                        break;
                    case SpokeInstructionType.GreaterFloatFloat:
                        stack[stackIndex - 2] = (stack[stackIndex - 2].FloatVal > stack[stackIndex - 1].FloatVal) ? TRUE : FALSE;
                        stackIndex = stackIndex - 1;
                        break;
                    case SpokeInstructionType.LessIntInt:
                        stack[stackIndex - 2] = (stack[stackIndex - 2].IntVal < stack[stackIndex - 1].IntVal) ? TRUE : FALSE;
                        stackIndex = stackIndex - 1;
                        break;
                    case SpokeInstructionType.LessIntFloat:
                        stack[stackIndex - 2] = (stack[stackIndex - 2].IntVal < stack[stackIndex - 1].FloatVal) ? TRUE : FALSE;
                        stackIndex = stackIndex - 1;
                        break;
                    case SpokeInstructionType.LessFloatInt:
                        stack[stackIndex - 2] = (stack[stackIndex - 2].FloatVal < stack[stackIndex - 1].IntVal) ? TRUE : FALSE;
                        stackIndex = stackIndex - 1;
                        break;
                    case SpokeInstructionType.LessFloatFloat:
                        stack[stackIndex - 2] = (stack[stackIndex - 2].FloatVal < stack[stackIndex - 1].FloatVal) ? TRUE : FALSE;
                        stackIndex = stackIndex - 1;
                        break;
                    case SpokeInstructionType.GreaterEqualIntInt:
                        stack[stackIndex - 2] = (stack[stackIndex - 2].IntVal >= stack[stackIndex - 1].IntVal) ? TRUE : FALSE;
                        stackIndex = stackIndex - 1;
                        break;
                    case SpokeInstructionType.GreaterEqualIntFloat:
                        stack[stackIndex - 2] = (stack[stackIndex - 2].IntVal >= stack[stackIndex - 1].FloatVal) ? TRUE : FALSE;
                        stackIndex = stackIndex - 1;
                        break;
                    case SpokeInstructionType.GreaterEqualFloatInt:
                        stack[stackIndex - 2] = (stack[stackIndex - 2].FloatVal >= stack[stackIndex - 1].IntVal) ? TRUE : FALSE;
                        stackIndex = stackIndex - 1;
                        break;
                    case SpokeInstructionType.GreaterEqualFloatFloat:
                        stack[stackIndex - 2] = (stack[stackIndex - 2].FloatVal >= stack[stackIndex - 1].FloatVal) ? TRUE : FALSE;
                        stackIndex = stackIndex - 1;
                        break;
                    case SpokeInstructionType.LessEqualIntInt:
                        stack[stackIndex - 2] = (stack[stackIndex - 2].IntVal <= stack[stackIndex - 1].IntVal) ? TRUE : FALSE;
                        stackIndex = stackIndex - 1;
                        break;
                    case SpokeInstructionType.LessEqualIntFloat:
                        stack[stackIndex - 2] = (stack[stackIndex - 2].IntVal <= stack[stackIndex - 1].FloatVal) ? TRUE : FALSE;
                        stackIndex = stackIndex - 1;
                        break;
                    case SpokeInstructionType.LessEqualFloatInt:
                        stack[stackIndex - 2] = (stack[stackIndex - 2].FloatVal <= stack[stackIndex - 1].IntVal) ? TRUE : FALSE;
                        stackIndex = stackIndex - 1;
                        break;
                    case SpokeInstructionType.LessEqualFloatFloat:
                        stack[stackIndex - 2] = (stack[stackIndex - 2].FloatVal <= stack[stackIndex - 1].FloatVal) ? TRUE : FALSE;
                        stackIndex = stackIndex - 1;
                        break;
                    case SpokeInstructionType.Equal:
                        stack[stackIndex - 2] = SpokeObject.Compare(stack[stackIndex - 2], stack[stackIndex - 1]) ? TRUE : FALSE;
                        stackIndex = stackIndex - 1;
                        break;
                    case SpokeInstructionType.InsertToArray:
                        break;
                    case SpokeInstructionType.RemoveToArray:
                        break;
                    case SpokeInstructionType.AddToArray:
                        lastStack = stack[--stackIndex];
                        stack[stackIndex - 1].AddArray(lastStack);
                        break;
                    case SpokeInstructionType.AddRangeToArray:
                        lastStack = stack[--stackIndex];
                        stack[stackIndex - 1].AddRangeArray(lastStack);
                        break;
                    case SpokeInstructionType.LengthOfArray:
                        break;

                    case SpokeInstructionType.ArrayElem:

                        lastStack = stack[--stackIndex];
                        stack[stackIndex - 1] = stack[stackIndex - 1].ArrayItems[lastStack.IntVal];

                        break;
                    case SpokeInstructionType.StoreArrayElem:

                        var indexs = stack[--stackIndex];
                        var ars = stack[--stackIndex];

                        ars.ArrayItems[indexs.IntVal] = stack[--stackIndex];


                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

#if stacktrace
			dfss.AppendLine(fm.Class.Name + " : : " + fm.MethodName + " End");
#endif
            stackTrace.Remove(st);
            return null;
        }

        private GameBoard buildBoard(SpokeObject spokeObject)
        {
            GameBoard gb = new GameBoard();

            var mainArea = getVariableByName(spokeObject, "mainArea");
            var userAreas = getVariableByName(spokeObject, "userAreas");


            gb.MainArea = buildArea(mainArea, getVariableByName(spokeObject, "piles"));
            gb.UserAreas = new List<TableArea>();
            foreach (var area in userAreas.ArrayItems)
            {
                gb.UserAreas.Add(buildArea(area, getVariableByName(spokeObject, "piles")));

            }

            return gb;
        }

        private TableArea buildArea(SpokeObject mainArea, SpokeObject cardgamePiles)
        {
            var tca = new TableArea();
            tca.Dimensions = getRect(getVariableByName(mainArea, "dimensions"));
            tca.NumberOfCardsHorizontal = getVariableByName(mainArea, "numberOfCardsHorizontal").IntVal;
            tca.NumberOfCardsVertical = getVariableByName(mainArea, "numberOfCardsVertical").IntVal;
            tca.Spaces = new List<TableSpace>();
            tca.TextAreas = new List<TableTextArea>();

            foreach (var space in getVariableByName(mainArea, "spaces").ArrayItems)
            {
                TableSpace ts;
                tca.Spaces.Add(ts = new TableSpace());
                ts.Visible = getVariableByName(space, "visible").BoolVal;
                ts.Width = getVariableByName(space, "width").IntVal;
                ts.Height = getVariableByName(space, "height").IntVal;
                ts.X = getVariableByName(space, "xPosition").IntVal;
                ts.Y = getVariableByName(space, "yPosition").IntVal;
                ts.DrawCardsBent = getVariableByName(space, "drawCardsBent").BoolVal;
                ts.Name = getVariableByName(space, "name").StringVal;
                ts.StackCards = getVariableByName(space, "stackCards").BoolVal;
                ts.Cards = new List<Card>();

                foreach (var cardgamePile in cardgamePiles.ArrayItems)
                {
                    if (getVariableByName(cardgamePile, "Name").StringVal == getVariableByName(space, "pileName").StringVal)
                    {
                        foreach (var card in getVariableByName(cardgamePile, "Cards").ArrayItems)
                            ts.Cards.Add(new Card(getVariableByName(card, "Value").IntVal, getVariableByName(card, "Type").IntVal, getVariableByName(card, "CardName").StringVal));
                        break;
                    }
                }

            }


            foreach (var textarea in getVariableByName(mainArea, "textAreas").ArrayItems)
            {
                TableTextArea ta;
                tca.TextAreas.Add(ta = new TableTextArea());
                ta.Name = getVariableByName(textarea, "name").StringVal;
                ta.RotateAngle = getVariableByName(textarea, "rotateAngle").IntVal;
                ta.Text = getVariableByName(textarea, "text").StringVal;
                ta.X = getVariableByName(textarea, "xPosition").IntVal;
                ta.Y = getVariableByName(textarea, "yPosition").IntVal;

            }
            return tca;
        }

        public Rectangle getRect(SpokeObject so)
        {
            return new Rectangle(getVariableByName(so, "x").IntVal, getVariableByName(so, "y").IntVal, getVariableByName(so, "width").IntVal, getVariableByName(so, "height").IntVal);
        }

    }

    internal class AskQuestionException : Exception
    {
        private readonly List<RunInstructions.StackTracer> myStackTrace;
        private readonly SpokeQuestion myQuestion;
        private readonly GameBoard myGameBoard;

        public AskQuestionException(List<RunInstructions.StackTracer> stackTrace, SpokeQuestion question, GameBoard gameBoard)
        {
            myStackTrace = stackTrace;
            myQuestion = question;
            myGameBoard = gameBoard;
        }

        public GameBoard GameBoard
        {
            get { return myGameBoard; }
        }

        public SpokeQuestion Question
        {
            get { return myQuestion; }
        }

        public List<RunInstructions.StackTracer> StackTrace
        {
            get { return myStackTrace; }
        }

    }
}
