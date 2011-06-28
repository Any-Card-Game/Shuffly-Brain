//#define dontwrite

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConsoleApplication1
{
    public class RunGame
    { 

        public static Tuple<SpokeQuestion,string> StartGame(string gameName)
        {
             
             
            Dictionary<string, SpokeType> vs;
            Dictionary<string, Func<SpokeObject[], SpokeObject>> rv = getIncludedMethods(out vs);


            return new RunApp().Begin("Applications\\" + gameName + ".spoke", rv, vs,"",0);
        }

        private static Dictionary<string, Func<SpokeObject[], SpokeObject>> getIncludedMethods(out Dictionary<string, SpokeType> vs) {
            var rv = new Dictionary<string, Func<SpokeObject[], SpokeObject>>() {
                                                                                    {
                                                                                        "write", (a) => {
                                                                                                     for (int index = 1;
                                                                                                          index < a.Length;
                                                                                                          index++) {
                                                                                                         var spokeObject = a[index];
#if !dontwrite
                                                                                                         Console.Write(
                                                                                                             spokeObject.ToString() + " ");
#endif
                                                                                                     }
                                                                                                     return null;
                                                                                                 }
                                                                                        }, 
                                                                    
                                                                                    {
                                                                                        "readLine", (a) => {
                                                                                                        return new SpokeObject() {
                                                                                                                                     Type =
                                                                                                                                         ObjectType
                                                                                                                                         .
                                                                                                                                         String,
#if !dontwrite
                                                                                                                                     StringVal
                                                                                                                                         =
                                                                                                                                         Console
                                                                                                                                         .
                                                                                                                                         ReadLine
                                                                                                                                         ()
#else
                                                                    StringVal=""
#endif
                                                                                                                                 };
                                                                                                    }
                                                                                        },
                                                                                    {
                                                                                        "read", (a) => {
                                                                                                    return new SpokeObject() {
                                                                                                                                 Type =
                                                                                                                                     ObjectType
                                                                                                                                     .
                                                                                                                                     String,
#if !dontwrite
                                                                                                                                 StringVal
                                                                                                                                     =
                                                                                                                                     Console
                                                                                                                                     .Read()
                                                                                                                                     .
                                                                                                                                     ToString
                                                                                                                                     ()
#else
                                                                StringVal =""
#endif
                                                                                                                             };
                                                                                                }
                                                                                        },
                                                                                    {
                                                                                        "stringToInt", (a) => {
                                                                                                           return new SpokeObject() {
                                                                                                                                        Type
                                                                                                                                            =
                                                                                                                                            ObjectType
                                                                                                                                            .
                                                                                                                                            Int,
                                                                                                                                        IntVal
                                                                                                                                            =
                                                                                                                                            int
                                                                                                                                            .
                                                                                                                                            Parse
                                                                                                                                            (a
                                                                                                                                                 [
                                                                                                                                                     1
                                                                                                                                                 ]
                                                                                                                                                 .
                                                                                                                                                 StringVal)
                                                                                                                                    };
                                                                                                       }
                                                                                        },
                                                                                    {
                                                                                        "floatToInt", (a) => {
                                                                                                          return new SpokeObject() {
                                                                                                                                       Type
                                                                                                                                           =
                                                                                                                                           ObjectType
                                                                                                                                           .
                                                                                                                                           Int,
                                                                                                                                       IntVal
                                                                                                                                           =
                                                                                                                                           (
                                                                                                                                           int
                                                                                                                                           )
                                                                                                                                           a
                                                                                                                                               [
                                                                                                                                                   1
                                                                                                                                               ]
                                                                                                                                               .
                                                                                                                                               FloatVal
                                                                                                                                   };
                                                                                                      }
                                                                                        },
                                                                                    {
                                                                                        "debug", (a) => {
                                                                                                     return null;
                                                                                                 }
                                                                                        },
                                                                                    {
                                                                                        "writeLine", (a) => {
                                                                                                         for (int index = 1;
                                                                                                              index < a.Length;
                                                                                                              index++) {
                                                                                                             var spokeObject = a[index];
#if !dontwrite
                                                                                                             Console.Write(
                                                                                                                 spokeObject.ToString() +
                                                                                                                 " ");
#endif
                                                                                                         }
#if !dontwrite
                                                                                                         Console.Write("\r\n");
#endif
                                                                                                         return null;
                                                                                                     }
                                                                                        },
                                                                                    {
                                                                                        "clearConsole", (a) => {
#if !dontwrite
                                                                                                            Console.Clear();
#endif
                                                                                                            return null;
                                                                                                        }
                                                                                        },
                                                                                    {
                                                                                        "stringLength", (a) => {
                                                                                                            return new SpokeObject() {
                                                                                                                                         Type
                                                                                                                                             =
                                                                                                                                             ObjectType
                                                                                                                                             .
                                                                                                                                             Int,
                                                                                                                                         IntVal
                                                                                                                                             =
                                                                                                                                             a
                                                                                                                                             [
                                                                                                                                                 1
                                                                                                                                             ]
                                                                                                                                             .
                                                                                                                                             StringVal
                                                                                                                                             .
                                                                                                                                             Length
                                                                                                                                     };
                                                                                                        }
                                                                                        },
                                                                                    {
                                                                                        "setConsolePosition", (a) => {
#if !dontwrite
                                                                                                                  Console.SetCursorPosition
                                                                                                                      (a[1].IntVal,
                                                                                                                       a[2].IntVal);
#endif
                                                                                                                  return null;
                                                                                                              }
                                                                                        },
                                                                                    {
                                                                                        "abs", (a) => {
                                                                                                   switch (a[1].Type) {
                                                                                                       case ObjectType.Int:
                                                                                                           var c = a[1].IntVal;
                                                                                                           return new SpokeObject() {
                                                                                                                                        IntVal = Math.Abs(c),
                                                                                                                                        Type = ObjectType.Int
                                                                                                                                    };

                                                                                                           break;
                                                                                                       case ObjectType.Float:
                                                                                                           var cd = a[1].FloatVal;
                                                                                                           return new SpokeObject() {
                                                                                                                                        FloatVal = Math.Abs(cd),
                                                                                                                                        Type = ObjectType.Float
                                                                                                                                    };

                                                                                                           break;
                                                                                                       default:
                                                                                                           throw new ArgumentOutOfRangeException
                                                                                                               ();
                                                                                                   }
                                                                                               }
                                                                                        },
                                                                                    {
                                                                                        "nextRandom", (a) => {

                                                                                                          if (a.Length == 2) {
                                                                                                              return new SpokeObject() {
                                                                                                                                           Type
                                                                                                                                               =
                                                                                                                                               ObjectType
                                                                                                                                               .
                                                                                                                                               Int,
                                                                                                                                           IntVal
                                                                                                                                               =
                                                                                                                                               rad
                                                                                                                                               .
                                                                                                                                               Next
                                                                                                                                               (a
                                                                                                                                                    [
                                                                                                                                                        1
                                                                                                                                                    ]
                                                                                                                                                    .
                                                                                                                                                    IntVal)
                                                                                                                                       };

                                                                                                          }
                                                                                                          return new SpokeObject() {
                                                                                                                                       Type
                                                                                                                                           =
                                                                                                                                           ObjectType
                                                                                                                                           .
                                                                                                                                           Int,
                                                                                                                                       IntVal
                                                                                                                                           =
                                                                                                                                           rad
                                                                                                                                           .
                                                                                                                                           Next
                                                                                                                                           (a
                                                                                                                                                [
                                                                                                                                                    1
                                                                                                                                                ]
                                                                                                                                                .
                                                                                                                                                IntVal,
                                                                                                                                            a
                                                                                                                                                [
                                                                                                                                                    2
                                                                                                                                                ]
                                                                                                                                                .
                                                                                                                                                IntVal)
                                                                                                                                   };
                                                                                                          return null;
                                                                                                      }
                                                                                        },
                                                                                    {
                                                                                        "rand", (a) => {
                                                                                                    var vfd = new SpokeObject() {
                                                                                                                                    Type =
                                                                                                                                        ObjectType
                                                                                                                                        .
                                                                                                                                        Float,
                                                                                                                                    FloatVal
                                                                                                                                        =
                                                                                                                                        (
                                                                                                                                        float
                                                                                                                                        )
                                                                                                                                        rad
                                                                                                                                            .
                                                                                                                                            NextDouble
                                                                                                                                            ()
                                                                                                                                };
                                                                                                    return vfd;
                                                                                                }
                                                                                        }, 
                                                                                    {
                                                                                        "wait", (a) => {
#if !dontwrite
                                                                                                    Thread.Sleep(a[1].IntVal);
#else
                                                                                                    //Console.WriteLine("Waiting for " + a[1].IntVal + " milliseconds");
#endif
                                                                                                    return null;
                                                                                                }
                                                                                        }, 
                                                                                    {
                                                                                        "clone", (a) => {
                                                                                                     return new SpokeObject() {
                                                                                                                                  AnonMethod
                                                                                                                                      =
                                                                                                                                      a[1].
                                                                                                                                      AnonMethod,
                                                                                                                                  BoolVal =
                                                                                                                                      a[1].
                                                                                                                                      BoolVal,
                                                                                                                                  IntVal =
                                                                                                                                      a[1].
                                                                                                                                      IntVal,
                                                                                                                                  FloatVal
                                                                                                                                      =
                                                                                                                                      a[1].
                                                                                                                                      FloatVal,
                                                                                                                                  StringVal
                                                                                                                                      =
                                                                                                                                      a[1].
                                                                                                                                      StringVal,
                                                                                                                                  Variables
                                                                                                                                      =
                                                                                                                                      a[1].
                                                                                                                                      Variables,
                                                                                                                                  ArrayItems
                                                                                                                                      =
                                                                                                                                      a[1].
                                                                                                                                      ArrayItems,
                                                                                                                                  Type =
                                                                                                                                      a[1].
                                                                                                                                      Type,
                                                                                                                                  ByRef =
                                                                                                                                      a[1].
                                                                                                                                      ByRef,
                                                                                                                                  ClassName
                                                                                                                                      =
                                                                                                                                      a[1].
                                                                                                                                      ClassName,
                                                                                                                              };
                                                                                                 }
                                                                                        },{
                                                                                              "populateUsers", (a) => {
                                                                                                                   a[1].AddArray(
                                                                                                                       new SpokeObject(new SpokeObject[] {
                                                                                                                                                             new SpokeObject(
                                                                                                                                                                 new List<SpokeObject>()),
                                                                                                                                                             new SpokeObject("Sal"),
                                                                                                                                                         }));
                                                                                                                   return null;
                                                                                                               }
                                                                                              },{"askQuestion",(a)=> {
                                                                                                  

                                                                                                                   return new SpokeObject(0);

                                                                                                               }}
                                                                                };
            vs = new Dictionary<string, SpokeType>() {
                                                         {
                                                             "write", new SpokeType(ObjectType.Void)
                                                             }, 
                                                         {
                                                             "readLine", new SpokeType(ObjectType.String)
                                                             },
                                                         {
                                                             "read", new SpokeType(ObjectType.String)
                                                             },
                                                         {
                                                             "stringToInt", new SpokeType(ObjectType.Int)
                                                             },
                                                         {
                                                             "floatToInt", new SpokeType(ObjectType.Int)
                                                             },
                                                         {
                                                             "debug", new SpokeType(ObjectType.Void)
                                                             },
                                                         {
                                                             "writeLine", new SpokeType(ObjectType.Void)
                                                             },
                                                         {
                                                             "clearConsole", new SpokeType(ObjectType.Void)
                                                             },
                                                         {
                                                             "stringLength", new SpokeType(ObjectType.Int)
                                                             },
                                                         {
                                                             "setConsolePosition", new SpokeType(ObjectType.Void)
                                                             },
                                                         {
                                                             "abs", new SpokeType(ObjectType.Int)
                                                             },
                                                         {
                                                             "nextRandom", new SpokeType(ObjectType.Int)
                                                             },
                                                         {
                                                             "rand", new SpokeType(ObjectType.Float)
                                                             }, 
                                                         {
                                                             "wait", new SpokeType(ObjectType.Void)
                                                             }, 
                                                         {
                                                             "clone", new SpokeType(ObjectType.Object)
                                                             },{
                                                                   "populateUsers", new SpokeType(ObjectType.Void)
                                                                   },{
                                                                         "askQuestion", new SpokeType(ObjectType.Int)
                                                                         }
                                                     };
            return rv;
        }


        public static Tuple<SpokeQuestion, string> ResumeGame(string gameName, string stack,int returnIndex)
        {
             

            Dictionary<string, SpokeType> vs;
            Dictionary<string, Func<SpokeObject[], SpokeObject>> rv = getIncludedMethods(out vs);


            return new RunApp().Begin("Applications\\" + gameName + ".spoke", rv, vs, stack, returnIndex);

        }
        static Random rad = new Random();
    }
}
