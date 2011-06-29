using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace ConsoleApplication1
{
    public enum ObjectType
    {
        Unset, Null, Int, Float, String, Bool, Array, Object, Method,
        Void
    }
    [DataContract]
    public class SpokeObjectMethod
    {
        [DataMember]
        public bool HasYield;
        [DataMember]
        public bool HasYieldReturn;
        [DataMember]
        public bool HasReturn;
        [IgnoreDataMember]
        public SpokeLine[] Lines;
        [IgnoreDataMember]
        public ParamEter[] Parameters;
        [DataMember]
        public SpokeInstruction[] Instructions;
        [IgnoreDataMember]
        public SpokeVariable ReturnYield;
    }
    [DataContract]
    public class SpokeObject{
        private static int ids;
        [DataMember] public int ID;

        [DataMember]
        public int IntVal;
        [DataMember]
        public string StringVal;
        [DataMember]
        public bool BoolVal;
        [DataMember]
        public float FloatVal;
        [DataMember]
        public ObjectType Type;
        [DataMember]
        public SpokeObject[] Variables;
        [DataMember]
        public List<SpokeObject> ArrayItems;
        [DataMember]
        public string ClassName;
         [DataMember]
        public SpokeObjectMethod AnonMethod;
         [DataMember]
        public bool ByRef;

        public SpokeObject()
        {

        }
        public SpokeObject(int inde)
        {
            IntVal = inde;
            Type = ObjectType.Int; ID = ids++;
        }
        public SpokeObject(SpokeObject[] inde, string className)
        {
            Array.Resize(ref inde, 20);

            Variables = inde;
            Type = ObjectType.Object;
            ClassName = className; ID = ids++;

        }
        public SpokeObject(List<SpokeObject> inde)
        {
            ArrayItems = inde; Type = ObjectType.Array; ID = ids++;


        }
        public SpokeObject(float inde)
        {
            FloatVal = inde; Type = ObjectType.Float; ID = ids++;



        }
        public SpokeObject(string inde)
        {
            StringVal = inde; Type = ObjectType.String; ID = ids++;


        }
        public SpokeObject(bool inde) {
            BoolVal = inde; Type = ObjectType.Bool; ID = ids++;


        }


        public SpokeObject(ObjectType type)
        {

            Type = type; 
            ID=ids++;

        }

        public void SetVariable(int name, SpokeObject obj)
        {
            Variables[name] = obj;

        }
        public SpokeObject GetVariable(int name, bool forSet)
        {
            SpokeObject g = Variables[name];

            if (g == null || forSet)
            {
                return Variables[name] = new SpokeObject();
            }
            return g;
        }
        public bool TryGetVariable(int name, out SpokeObject obj)
        {
            return (obj = Variables[name]) != null;
        }


        public bool Compare(SpokeObject obj) {
            return Compare(this, obj);
        }


        internal static bool Compare(SpokeObject left, SpokeObject right)
        {
            if (left == null)
            {
                if (right == null)
                {
                    return true;
                }
                if (right.Type == ObjectType.Null)
                {
                    return true;
                }
            }
            else
            {
                if (left.Type == ObjectType.Null && right == null)
                {
                    return true;
                }
            }
            if (left.Type != right.Type)
            {
                return false;
            }
            switch (left.Type)
            {
                case ObjectType.Null:
                    return true;
                    break;
                case ObjectType.Unset:
                    return true;
                    break;
                case ObjectType.Int:
                    return left.IntVal == right.IntVal;
                    break;
                case ObjectType.Float:
                    return left.FloatVal == right.FloatVal;
                    break;
                case ObjectType.String:
                    return left.StringVal == right.StringVal;
                    break;
                case ObjectType.Bool:
                    return left.BoolVal == right.BoolVal;
                    break;

                case ObjectType.Object:
                    //                    throw new AbandonedMutexException("not yet cowbow");

                    for (int i = 0; i < left.Variables.Length; i++)
                    {
                        if (!Compare(left.Variables[i],right.Variables[i]))
                        {
                            return false;
                        }
                    
                    }
                    return true;
                    break;
                case ObjectType.Array:

                    if (left.ArrayItems.Count != right.ArrayItems.Count) return false;

                    for (int i = 0; i < left.ArrayItems.Count; i++)
                    {
                        if (!Compare(right.ArrayItems[i], left.ArrayItems[i]))
                        {
                            return false;
                        }
                    }
                    return true;
                    break;

                case ObjectType.Method:
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        public override string ToString()
        {
            StringBuilder sb;
            switch (Type)
            {
                case ObjectType.Unset:
                    return "fail";
                    break;
                case ObjectType.Null:
                    return "NULL";
                    break;
                case ObjectType.Int:
                    return IntVal.ToString();
                    break;
                case ObjectType.Float:
                    return FloatVal.ToString();
                    break;
                case ObjectType.String:
                    return StringVal;
                    break;
                case ObjectType.Bool:
                    return BoolVal.ToString();
                    break;

                case ObjectType.Object:
                    sb = new StringBuilder();
                    sb.Append(" " + this.ClassName ?? "");
                    for (int index = 0; index < Variables.Length; index++)
                    {
                        var spokeObject = Variables[index];
                        sb.Append(",  " + index + ": " + Variables[index]);
                    }
                    return sb.ToString();

                case ObjectType.Array:

                    sb = new StringBuilder();

                    sb.Append("[");

                    foreach (var spokeObject in ArrayItems)
                    {
                        sb.Append("  " + spokeObject.ToString());
                    }
                    sb.Append("]");

                    return sb.ToString();
                case ObjectType.Method:
                default:
                    //throw new ArgumentOutOfRangeException();
                    break;
            }

            return base.ToString();
        }

        public void AddRangeArray(SpokeObject lastStack)
        {
            ArrayItems.AddRange(lastStack.ArrayItems);
        }
        public SpokeObject AddArray(SpokeObject lastStack)
        {

            ArrayItems.Add(lastStack);
            return this;
        }

        public SpokeObject AddArrayRange(SpokeObject lastStack)
        {
            ArrayItems.AddRange(lastStack.ArrayItems);
            return this;
        }
    }
}