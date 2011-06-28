using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using FM.WebSync.Core;


namespace ConsoleApplication1
{
    class Program
    {
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
                    Console.WriteLine("The client received data... (text: " + payload.Text + ", time: " + payload.Time + ")");
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
                    publisher.Publish(new PublicationArgs
                    {
                        Publication = new Publication
                        {
                            Channel = channel,
                            DataJson = JSON.Serialize(new Payload // custom class
                            {
                                Text = "Hello, world!",
                                Time = DateTime.Now
                            })
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
                    });
                }
            }




            RunGame.rungame();
        }
    }

    internal class Payload{
        public string Text { get; set; }
        public DateTime Time { get; set; }
    }
}
