//#define dontwrite

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using ConsoleApplication1.Game;

namespace ConsoleApplication1
{
    public class RunGame
    {

        public static Tuple<SpokeQuestion, string, GameBoard> StartGame(string gameName, Dictionary<string, string> playersInGame)
        {


            Dictionary<string, SpokeType> vs;
            Dictionary<string, Func<SpokeObject[], SpokeObject>> rv = getIncludedMethods(out vs, playersInGame);


            return new RunApp().Begin("Applications\\" + gameName + ".spoke", rv, vs, "", 0);
        }

        private static Dictionary<string, Func<SpokeObject[], SpokeObject>> getIncludedMethods(out Dictionary<string, SpokeType> vs, Dictionary<string, string> playersInGame)
        {
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
                                                                                            return new SpokeObject(Console.ReadLine());
                                                                                                    }
                                                                                        },
                                                                                    {
                                                                                        "read", (a) => {
                                                                                                    return new SpokeObject(Console
                                                                                                                                     .Read()
                                                                                                                                     .
                                                                                                                                     ToString
                                                                                                                                     ())  ;
                                                                                                }
                                                                                        },
                                                                                    {
                                                                                        "stringToInt", (a) => {
                                                                                                           return new SpokeObject(int
                                                                                                                                            .
                                                                                                                                            Parse
                                                                                                                                            (a
                                                                                                                                                 [
                                                                                                                                                     1
                                                                                                                                                 ]
                                                                                                                                                 .
                                                                                                                                                 StringVal))  ;
                                                                                                       }
                                                                                        },
                                                                                    {
                                                                                        "floatToInt", (a) => {
                                                                                            return new SpokeObject(
                                                                                                                             (
                                                                                                                             int
                                                                                                                             )
                                                                                                                             a
                                                                                                                                 [
                                                                                                                                     1
                                                                                                                                 ]
                                                                                                                                 .
                                                                                                                                 FloatVal)
                                                                                                                                  ;
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
                                                                                            return new SpokeObject(a
                                                                                                                             [
                                                                                                                                 1
                                                                                                                             ]
                                                                                                                             .
                                                                                                                             StringVal
                                                                                                                             .
                                                                                                                             Length);
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
                                                                                                           return new SpokeObject(Math.Abs(c));

                                                                                                           break;
                                                                                                       case ObjectType.Float:
                                                                                                           var cd = a[1].FloatVal;
                                                                                                           return new SpokeObject(Math.Abs(
                                                                                                                                                cd))
                                                                                                                                                 ;

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
                                                                                                              return new SpokeObject(rad
                                                                                                                                               .
                                                                                                                                               Next
                                                                                                                                               (a
                                                                                                                                                    [
                                                                                                                                                        1
                                                                                                                                                    ]
                                                                                                                                                    .
                                                                                                                                                    IntVal))  ;

                                                                                                          }
                                                                                                          return new SpokeObject(rad
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
                                                                                                                                                IntVal));
                                                                                                          return null;
                                                                                                      }
                                                                                        },
                                                                                    {
                                                                                        "rand", (a) => {
                                                                                            var vfd = new SpokeObject((
                                                                                                                                float
                                                                                                                                )
                                                                                                                                rad
                                                                                                                                    .
                                                                                                                                    NextDouble
                                                                                                                                    ())
                                                                                                                                     ;
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
                                                                                        },
                                                                                    {
                                                                                        "populateUsers", (a) => {

                                                                                                             SpokeObject rt =
                                                                                                                 new SpokeObject(
                                                                                                                     new List<SpokeObject>());
                                                                                                             foreach (
                                                                                                                 var player in playersInGame) {
                                                                                                                 rt.AddArray(new SpokeObject(player.Value));
                                                                                                             }
                                                                                                             return rt;
                                                                                                         }
                                                                                        },
                                                                                    {
                                                                                        "askQuestion", (a) => {


                                                                                                           return new SpokeObject(0);

                                                                                                       }
                                                                                        }
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
                                                                   "populateUsers", new SpokeType(ObjectType.Array)
                                                                   },{
                                                                         "askQuestion", new SpokeType(ObjectType.Int)
                                                                         }
                                                     };
            return rv;
        }


        public static Tuple<SpokeQuestion, string, GameBoard> ResumeGame(string gameName, string stack, int returnIndex, Dictionary<string, string> playersInGame)
        {


            Dictionary<string, SpokeType> vs;
            Dictionary<string, Func<SpokeObject[], SpokeObject>> rv = getIncludedMethods(out vs, playersInGame);


            return new RunApp().Begin("Applications\\" + gameName + ".spoke", rv, vs, stack, returnIndex);

        }
        static Random rad = new Random();
    }
}