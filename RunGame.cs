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
 public   class RunGame
    {
        private static List<Tuple<PointF, PointF, Color>> lines = new List<Tuple<PointF, PointF, Color>>();
        private static int ticks = 0;

        private static int indes = 10000;


        private static void run(Dictionary<string, Func<SpokeObject[], SpokeObject>> rv, Dictionary<string, SpokeType> vs)
        {



            RunApp ra;
            //ra = new RunApp("Applications\\liquid.spoke", rv, vs);
            //ra = new RunApp("Applications\\grideas.spoke", rv, vs);
            //ra = new RunApp("Applications\\sevens.spoke", rv, vs);
            //ra = new RunApp("Applications\\fallsfromlord.spoke", rv, vs);
            //ra = new RunApp("Applications\\realSimple.spoke", rv, vs);
            ra = new RunApp("Applications\\easy2.spoke", rv, vs);
            ra = new RunApp("Applications\\easy.spoke", rv, vs);
            //         ra = new RunApp("Applications\\prime.spoke", rv, vs);
            ra = new RunApp("Applications\\tower.spoke", rv, vs);
            ra = new RunApp("Applications\\golf.spoke", rv, vs);
            ra = new RunApp("Applications\\simple.spoke", rv, vs);
            //ra = new RunApp("Applications\\isFo.spoke", rv, vs);
            //  ra = new RunApp("Applications\\tester.spoke", rv, vs);

        }


        public static void rungame()
        {

            // new MandelBrot();

            var ff = new Font(FontFamily.GenericMonospace, 9);
            var rv = new Dictionary
                <string, Func<SpokeObject[], SpokeObject>>() {
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
                                                                     "getMouseX", (a) => {
                                                                                      var vfd = new SpokeObject() {
                                                                                                                      Type = ObjectType.Int,
                                                                                                                      IntVal = 0
                                                                                                                  };
                                                                                      return vfd;
                                                                                  }
                                                                     },
                                                                 {
                                                                     "getMouseY", (a) => {
                                                                                      var vfd = new SpokeObject() {
                                                                                                                      Type = ObjectType.Int,
                                                                                                                      IntVal = 0
                                                                                                                  };
                                                                                      return vfd;
                                                                                  }
                                                                     },
                                                                 {
                                                                     "getMouseClicked", (a) => {
                                                                                            var vfd =
                                                                                                new SpokeObject() {
                                                                                                                      Type = ObjectType.Bool,
                                                                                                                      BoolVal = false
                                                                                                                  };
                                                                                            return vfd;
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
                                                                     "line", (a) => {
                                                                                 Color col = Color.Pink;
                                                                                 switch ((int) a[5].FloatVal) {
                                                                                     case 1:
                                                                                         col = Color.White;
                                                                                         break;
                                                                                     case 2:
                                                                                         col = Color.Blue;
                                                                                         break;
                                                                                     case 3:
                                                                                         col = Color.Green;
                                                                                         break;
                                                                                 }


                                                                                 lines.Add(
                                                                                     new Tuple<PointF, PointF, Color>(
                                                                                         new PointF(a[1].FloatVal + 10,
                                                                                                    a[2].FloatVal + 10),
                                                                                         new PointF(a[3].FloatVal + 10,
                                                                                                    a[4].FloatVal + 10),
                                                                                         col));
                                                                                 return null;
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
                                                                     "paintInternal", (a) => {

                                                                                          Bitmap bm = new Bitmap(850,
                                                                                                                 850);

                                                                                          var efd =
                                                                                              Graphics.FromImage(bm);

                                                                                          efd.FillRectangle(
                                                                                              Brushes.Black, 0, 0, 850,
                                                                                              850);
                                                                                          efd.DrawString(
                                                                                              ticks++ + " Ticks", ff,
                                                                                              Brushes.White, 0, 0);

                                                                                          if (lines != null) {

                                                                                              for (
                                                                                                  int index =
                                                                                                      lines.Count - 1;
                                                                                                  index >= 0;
                                                                                                  index--) {
                                                                                                  var line =
                                                                                                      lines[index];
                                                                                                  if (line.Item1 ==
                                                                                                      line.Item2) {
                                                                                                      efd.DrawLine(
                                                                                                          new Pen(
                                                                                                              Color.Red),
                                                                                                          line.Item1,
                                                                                                          new PointF(
                                                                                                              line.Item2
                                                                                                                  .X +
                                                                                                              0.01f,
                                                                                                              line.Item2
                                                                                                                  .Y +
                                                                                                              0.01f));

                                                                                                  }
                                                                                                  else
                                                                                                      efd.DrawLine(
                                                                                                          new Pen(
                                                                                                              line.Item3),
                                                                                                          line.Item1,
                                                                                                          line.Item2);
                                                                                              }
                                                                                              lines.Clear();

                                                                                          }
                                                                                          efd.Save();
                                                                                          Console.WriteLine("water" +
                                                                                                            indes +
                                                                                                            " Created");
                                                                                          bm.Save(
                                                                                              "C:\\water3\\" + indes++ +
                                                                                              ".png", ImageFormat.Png);

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
                                                                     }
                                                             };
            var vs = new Dictionary<string, SpokeType>() {
                                                             {
                                                                 "write", new SpokeType(ObjectType.Void)
                                                                 },
                                                             {
                                                                 "getMouseX", new SpokeType(ObjectType.Int)
                                                                 },
                                                             {
                                                                 "getMouseY", new SpokeType(ObjectType.Int)
                                                                 },
                                                             {
                                                                 "getMouseClicked", new SpokeType(ObjectType.Bool)
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
                                                                 "line", new SpokeType(ObjectType.Void)
                                                                 },
                                                             {
                                                                 "wait", new SpokeType(ObjectType.Void)
                                                                 },
                                                             {
                                                                 "paintInternal", new SpokeType(ObjectType.Void)
                                                                 },
                                                             {
                                                                 "clone", new SpokeType(ObjectType.Object)
                                                                 }
                                                         };



            run(rv, vs);
        }
        static Random rad = new Random();
    }
}
