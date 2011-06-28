using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using System.Threading;
using FM.WebSync.Core;
using System.Runtime.Serialization.Json;

namespace ConsoleApplication1
{
    class Program
    {
        private static Dictionary<string, string> playersInGame = new Dictionary<string, string>();
        private static string stackTrace;

        static void Main(string[] args)
        {
            string channel = "/consolepublisher";
            Console.WriteLine("Initializing...");

            Client client = new Client(new ClientArgs
            {
                // replace with your domain key
                DomainKey = "035d9877-9eaf-45ef-936f-80087fb3a221",
                // replace with your domain name
                DomainName = "localhost",
                Synchronous = true
            });

            Publisher publisher = new Publisher(new PublisherArgs
            {
                // replace with your domain key
                DomainKey = "035d9877-9eaf-45ef-936f-80087fb3a221",
                // replace with your domain name
                DomainName = "localhost",
                Synchronous = true
            });

            client.Connect(new ConnectArgs
            {
                OnSuccess = (successArgs) =>
                {
                    Console.WriteLine("The client connected and has ID " + successArgs.ClientId + ".");
                },
                OnFailure = (failureArgs) =>
                {
                    Console.WriteLine("The client could not connect... " + failureArgs.Exception.Message);
                },
                OnStreamFailure = (streamFailureArgs) =>
                {
                    Console.WriteLine("The client could not stream... " + streamFailureArgs.Exception.Message);
                    Console.WriteLine("The client " + (streamFailureArgs.WillReconnect ? "will" : "will not") + " automatically reconnect." + Environment.NewLine);
                }
            });

            // client is configured as "synchronous", so this
            // won't execute until the connect method is complete
            if (client.State != ClientState.Connected)
            {
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            client.Subscribe(new SubscribeArgs
            {
                Channel = channel,
                OnSuccess = (successArgs) =>
                {
                    Console.WriteLine("The client " + (successArgs.IsResubscribe ? "re-" : "") + "subscribed to " + channel + ".");
                },
                OnFailure = (failureArgs) =>
                {
                    Console.WriteLine("The client could not subscribe to " + channel + "... " + failureArgs.Exception.Message);
                },
                OnReceive = (receiveArgs) =>
                {


                    Payload payload = JSON.Deserialize<Payload>(receiveArgs.DataJson);




                    Tuple<SpokeQuestion, string> vf = null;
                    switch (payload.Type)
                    {
                        case SpokeMessageType.AskQuestion:
                            return;
                        case SpokeMessageType.JoinGame:
                            playersInGame.Add(receiveArgs.PublishingClient.Id,payload.PlayerName);
                            if (playersInGame.Count == 2)
                            {
                                vf = RunGame.StartGame("sevens", playersInGame);
                                stackTrace = vf.Item2;
                            }
                            else return;
                            break;
                        case SpokeMessageType.AnswerQuestion:
                            vf = RunGame.ResumeGame("sevens", stackTrace, payload.AnswerIndex, playersInGame);
                            stackTrace = vf.Item2;
                            Console.WriteLine(vf.Item1);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                    SpokeQuestion q = vf.Item1;

                    PublicationArgs fc = new PublicationArgs
                    {
                        Publication = new Publication
                        {
                            Channel = channel+"/"+q.User,
                            DataJson = JSON.Serialize(new Payload(q.Question, q.Answers))
                        },
                        OnComplete = (completeArgs) =>
                        {
                            if (completeArgs.Publication.Successful == true)
                            {
                                Console.WriteLine("The publisher published to " + channel + ".");
                            }
                            else
                            {
                                Console.WriteLine("The publisher could not publish to " + channel + "... " + completeArgs.Publication.Error);
                            }
                        },
                        OnException = (exceptionArgs) =>
                        {
                            Console.WriteLine("The publisher threw an exception... " + exceptionArgs.Exception.Message);
                        }
                    };
                    publisher.Publish(fc);


                    //       Console.WriteLine("The client received data... (text: " + payload.Text + ", time: " + payload.Time + ")");
                }
            });

            Console.WriteLine();
            Console.WriteLine("Press Enter to publish text from the publisher.  Press Escape to quit.");


            while (true)
            {
                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
                if (consoleKeyInfo.Key == ConsoleKey.Escape)
                {
                    return;
                }
                if (consoleKeyInfo.Key == ConsoleKey.Enter)
                {
                     
                }
            }




        }
    }

    [DataContract]
    public class Payload
    {
        [DataMember(Name = "PlayerName")]
        public string PlayerName;

        [DataMember(Name = "Type")]
        public SpokeMessageType Type { get; set; }
        [DataMember(Name = "AnswerIndex")]
        public int AnswerIndex { get; set; }
        [DataMember(Name = "Question")]
        public string Question { get; set; }
        [DataMember(Name = "Answers")]
        public string[] Answers { get; set; }
        public Payload()
        {

        }

        public Payload(string question, string[] answers) {
            Type = SpokeMessageType.AskQuestion;
            Question = question;
            Answers = answers;

        }
    }

    public enum SpokeMessageType : int
    {
        JoinGame = 0,
        AnswerQuestion = 1,
        AskQuestion=2
    }
}
