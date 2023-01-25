// Copyright 2015-2018 The NATS Authors
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Text.Json;
using System.Text;
using NATS.Client;

namespace Backend.Logic
{
    class Subscriber
    {
        Dictionary<string, string> parsedArgs = new Dictionary<string, string>();

        // Use > to subscribe to all subjects
        public string subject = ">";
        int count = 1000000;
        string url;
        bool sync = true;
        int received = 0;
        bool verbose = true;
        string? creds = null;
        List<Msg> latestMessages;
        List<DateTime> timestamps;

        public string MessageSubject
        {
            get; set;
        }

        public Subscriber(string url = "nats://demo.nats.io")
        {
            latestMessages = new List<Msg>();
            timestamps = new List<DateTime>();
            MessageSubject = ">";
            this.url = url;
        }

        public void Run()
        {
            //parseArgs(args);
            //banner();

            Options opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = url;
            if (creds != null)
            {
                opts.SetUserCredentials(creds);
            }

            using (IConnection c = new ConnectionFactory().CreateConnection(opts))
            {
                TimeSpan elapsed;

                if (sync)
                {
                    elapsed = receiveSyncSubscriber(c);
                }
                else
                {
                    elapsed = receiveAsyncSubscriber(c);
                }

                // Console.Write("Received {0} msgs in {1} seconds ", received, elapsed.TotalSeconds);
                // Console.WriteLine("({0} msgs/second).",
                //     (int)(received / elapsed.TotalSeconds));
                // printStats(c);

            }
        }

        // private void printStats(IConnection c)
        // {
        //     IStatistics s = c.Stats;
        //     Console.WriteLine("Statistics:  ");
        //     Console.WriteLine("   Incoming Payload Bytes: {0}", s.InBytes);
        //     Console.WriteLine("   Incoming Messages: {0}", s.InMsgs);
        // }

        private TimeSpan receiveAsyncSubscriber(IConnection c)
        {
            Stopwatch sw = new Stopwatch();
            Object testLock = new Object();

            EventHandler<MsgHandlerEventArgs> msgHandler = (sender, args) =>
            {
                if (received == 0)
                    sw.Start();

                received++;

                if (verbose)
                    Console.WriteLine("Received: " + args.Message);

                if (received >= count)
                {
                    sw.Stop();
                    lock (testLock)
                    {
                        Monitor.Pulse(testLock);
                    }
                }
            };

            using (IAsyncSubscription s = c.SubscribeAsync(subject, msgHandler))
            {
                // just wait until we are done.
                lock (testLock)
                {
                    Monitor.Wait(testLock);
                }
            }

            return sw.Elapsed;
        }

        private void AddMessage(Msg lastMessage)
        {
            if (latestMessages.Count == 100)
            {
                latestMessages.RemoveAt(latestMessages.Count - 1);
                timestamps.RemoveAt(timestamps.Count - 1);
            }
            latestMessages.Insert(0, lastMessage);
            timestamps.Insert(0, DateTime.Now);
        }

        public string GetLatestMessages()
        {
            string json = "[";
            for (int i = 0; i < latestMessages.Count; i++)
            {
                Msg msg = latestMessages[i];
                string timestamp = timestamps[i].ToString("MM/dd/yyyy HH:mm:ss");

                json += JsonSerializer.Serialize(
                    new
                    {
                        messageSubject = msg.Subject,
                        messageTimestamp = timestamp,
                        messageAck = msg.LastAck,
                        // Checks if any characters are not ASCII.
                        messagePayload = msg.Data.All(b => b >= 32 && b <= 127) ? Encoding.ASCII.GetString(msg.Data) : msg.Data.ToString()
                    }
                );

                json = i < latestMessages.Count - 1 ? json + "," : json;
            }

            return json + "]";
        }

        private TimeSpan receiveSyncSubscriber(IConnection c)
        {
            using (ISyncSubscription s = c.SubscribeSync(subject))
            {
                Stopwatch sw = new Stopwatch();

                while (received < count)
                {
                    if (received == 0)
                        sw.Start();

                    Msg m = s.NextMessage();
                    received++;

                    if (string.Equals(MessageSubject, ">") || string.Equals(MessageSubject, m.Subject))
                        AddMessage(m);

                    // if (verbose)
                    //     Console.WriteLine("Received MSG on Subject: " + m.Subject + ", with Payload: " + Encoding.UTF8.GetString(m.Data));
                }

                sw.Stop();

                return sw.Elapsed;
            }
        }

        // private void usage()
        // {
        //     Console.Error.WriteLine(
        //         "Usage:  Subscribe [-url url] [-subject subject] " +
        //         "[-count count] [-creds file] [-sync] [-verbose]");

        //     Environment.Exit(-1);
        // }

        // private void parseArgs(string[] args)
        // {
        //     if (args == null)
        //         return;

        //     for (int i = 0; i < args.Length; i++)
        //     {
        //         if (args[i].Equals("-sync") ||
        //             args[i].Equals("-verbose"))
        //         {
        //             parsedArgs.Add(args[i], "true");
        //         }
        //         else
        //         {
        //             if (i + 1 == args.Length)
        //                 usage();

        //             parsedArgs.Add(args[i], args[i + 1]);
        //             i++;
        //         }

        //     }

        //     if (parsedArgs.ContainsKey("-count"))
        //         count = Convert.ToInt32(parsedArgs["-count"]);

        //     if (parsedArgs.ContainsKey("-url"))
        //         url = parsedArgs["-url"];

        //     if (parsedArgs.ContainsKey("-subject"))
        //         subject = parsedArgs["-subject"];

        //     if (parsedArgs.ContainsKey("-sync"))
        //         sync = true;

        //     if (parsedArgs.ContainsKey("-verbose"))
        //         verbose = true;

        //     if (parsedArgs.ContainsKey("-creds"))
        //         creds = parsedArgs["-creds"];
        // }

        // private void banner()
        // {
        //     Console.WriteLine("Receiving {0} messages on subject {1}",
        //         count, subject);
        //     Console.WriteLine("  Url: {0}", url);
        //     Console.WriteLine("  Subject: {0}", subject);
        //     Console.WriteLine("  Receiving: {0}",
        //         sync ? "Synchronously" : "Asynchronously");
        // }
    }
}