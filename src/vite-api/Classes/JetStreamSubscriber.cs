using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Text.Json;
using System.Text;
using NATS.Client;
using NATS.Client.JetStream;

namespace Backend.Logic
{
    public class JetStreamSubscriber
    {
        private List<string> subjects;
        // The amount of messages which max should be pulled from a stream at the same time
        private int batchSize = 1000;
        private int maxPayloadLength = 200;
        private string? url = Defaults.Url;
        private bool sync = true;
        private string? creds = null;
        private List<Msg> allMessages;

        public string StreamName { get; }

        public JetStreamSubscriber(string? url, string streamName, List<string> subjects)
        {
            allMessages = new List<Msg>();
            this.url = url;
            StreamName = streamName;
            this.subjects = subjects;
        }

        public void Run()
        {
            Options opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = url;
            if (creds != null)
                opts.SetUserCredentials(creds);

            using (IConnection c = new ConnectionFactory().CreateConnection(opts))
            {
                TimeSpan elapsed = receiveJetStreamPullSubscribe(c);
                //TimeSpan elapsed = sync ? receiveJetStreamPullSubscribe(c) : receiveAsyncSubscriber(c);
            }
        }

        //https://stackoverflow.com/questions/75181157/pull-last-batch-of-messages-from-a-nats-jetstream
        private TimeSpan receiveJetStreamPullSubscribe(IConnection c)
        {
            IJetStream js = c.CreateJetStreamContext();
            PullSubscribeOptions pullOptions = PullSubscribeOptions.Builder().WithStream(StreamName).Build();

            while (true) {
                List<Msg> currentMessages = new List<Msg>();

                foreach (string subject in subjects) {
                    IJetStreamPullSubscription sub = js.PullSubscribe(subject, pullOptions);
                    currentMessages = new List<Msg>(currentMessages.Concat(sub.Fetch(batchSize, 1000)));
                }
                
                allMessages = currentMessages;
            }
        }

        // private TimeSpan receiveAsyncSubscriber(IConnection c)
        // {
        //     Stopwatch sw = new Stopwatch();
        //     Object testLock = new Object();

        //     EventHandler<MsgHandlerEventArgs> msgHandler = (sender, args) =>
        //     {
        //         if (received == 0)
        //             sw.Start();

        //         received++;

        //         if (verbose)
        //             Console.WriteLine("Received: " + args.Message);

        //         if (received >= count)
        //         {
        //             sw.Stop();
        //             lock (testLock)
        //                 Monitor.Pulse(testLock);
        //         }
        //     };

        //     using (IAsyncSubscription s = c.SubscribeAsync(subject, msgHandler))
        //     {
        //         // just wait until we are done.
        //         lock (testLock)
        //             Monitor.Wait(testLock);
        //     }

        //     return sw.Elapsed;

        public string GetMessageData(ulong sequenceNumber) {

            string json = "[";
            for (int i = 0; i < allMessages.Count; i++)
            {
                Msg msg = allMessages[i];

                if (sequenceNumber == msg.MetaData.StreamSequence)
                {
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
                    
                    string? payload = msg.Data.ToString();

                    Console.WriteLine(msg.Data);

                    json += JsonSerializer.Serialize(
                    new
                        {
                            headers = headerData,
                            payload = payload?.Length > maxPayloadLength ? payload.Substring(0, maxPayloadLength) : payload,
                        }
                    );

                    break;
                }
            }
            
            return json + "]";
        }

        public string GetMessages()
        {
            List<Msg> allMessagesSorted = allMessages.OrderByDescending(a => a.MetaData.StreamSequence).ToList();

            string json = "";
            for (int i = 0; i < allMessagesSorted.Count; i++)
            {
                Msg msg = allMessagesSorted[i];

                json += JsonSerializer.Serialize(
                    new
                    {
                        sequenceNumber = msg.MetaData.StreamSequence,
                        timestamp = msg.MetaData.Timestamp,
                        stream = StreamName,
                        subject = msg.Subject,
                   }
                );

                json = i < allMessagesSorted.Count - 1 ? json + "," : json;
            }
            
            return json;
        }
    }
}