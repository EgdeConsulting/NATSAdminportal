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
    public class Subscriber
    {
        // Use > to subscribe to all subjects
        public string subject = ">";
        private int count = 1000000;
        private string? url = Defaults.Url;
        private SubjectManager subjectManager;
        private bool sync = true;
        private int received = 0;
        private bool verbose = true;
        private string? creds = null;
        private List<Msg> allMessages;
        private List<DateTime> timestamps;

        public string MessageSubject
        {
            get; set;
        }

        public Subscriber(string? url, SubjectManager subjectManager)
        {
            allMessages = new List<Msg>();
            timestamps = new List<DateTime>();
            MessageSubject = ">";
            this.url = url;
            this.subjectManager = subjectManager;
        }

        public void Run()
        {
            Options opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = url;
            if (creds != null)
                opts.SetUserCredentials(creds);

            using (IConnection c = new ConnectionFactory().CreateConnection(opts))
            {
                TimeSpan elapsed = sync ? receiveSyncSubscriber(c) : receiveAsyncSubscriber(c);
            }
        }

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
                        Monitor.Pulse(testLock);
                }
            };

            using (IAsyncSubscription s = c.SubscribeAsync(subject, msgHandler))
            {
                // just wait until we are done.
                lock (testLock)
                    Monitor.Wait(testLock);
            }

            return sw.Elapsed;
        }

        private void AddMessage(Msg lastMessage)
        {
            if (subjectManager.SubjectExists(lastMessage.Subject))
            {
                allMessages.Insert(0, lastMessage);
                timestamps.Insert(0, DateTime.Now);
            }
        }

        public string GetMessages()
        {
            string json = "[";
            for (int i = 0; i < allMessages.Count; i++)
            {
                Msg msg = allMessages[i];
                string timestamp = timestamps[i].ToString("MM/dd/yyyy HH:mm:ss");

                string headerData = "[";

                var enu = msg.Header.GetEnumerator();
                int index = 0;

                while (enu.MoveNext())
                {
                    headerData += JsonSerializer.Serialize(
                        new
                        {
                            name = enu.Current,
                            value = msg.Header[enu.Current.ToString()],
                        }
                    );
                    headerData = index < msg.Header.Count - 1 ? headerData + "," : headerData;
                    index++;
                }

                headerData += "]";

                json += JsonSerializer.Serialize(
                    new
                    {
                        subject = msg.Subject,
                        timestamp = timestamp,
                        acknowledgement = msg.LastAck,
                        headers = headerData,
                        // Checks if any characters are not ASCII.
                        payload = msg.Data.All(b => b >= 32 && b <= 127) ? Encoding.ASCII.GetString(msg.Data) : msg.Data.ToString()
                    }
                );

                json = i < allMessages.Count - 1 ? json + "," : json;
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
                }

                sw.Stop();

                return sw.Elapsed;
            }
        }
    }
}