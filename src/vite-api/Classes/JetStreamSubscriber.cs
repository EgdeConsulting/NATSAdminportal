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

        /// <summary>
        /// Starts the subscribers so that is fetches all previous, current and future messages on a stream. 
        /// </summary>
        public void Run()
        {
            Options opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = url;
            if (creds != null)
                opts.SetUserCredentials(creds);

            using (IConnection c = new ConnectionFactory().CreateConnection(opts))
            {
                TimeSpan elapsed = receiveJetStreamPullSubscribe(c);
            }
        }

        // The following method was created based on: https://stackoverflow.com/questions/75181157/pull-last-batch-of-messages-from-a-nats-jetstream
        /// <summary>
        /// Bulk pulls all messages on all subjects from a stream. 
        /// </summary>
        /// <param name="c">Nats connection</param>
        /// <returns></returns>
        private TimeSpan receiveJetStreamPullSubscribe(IConnection c)
        {
            IJetStream js = c.CreateJetStreamContext();
            PullSubscribeOptions pullOptions = PullSubscribeOptions.Builder().WithStream(StreamName).Build();

            while (true)
            {
                List<Msg> currentMessages = new List<Msg>();

                foreach (string subject in subjects)
                {
                    IJetStreamPullSubscription sub = js.PullSubscribe(subject, pullOptions);
                    currentMessages = new List<Msg>(currentMessages.Concat(sub.Fetch(batchSize, 1000)));
                }

                allMessages = currentMessages;
            }
        }

        /// <summary>
        /// Gets a JSON representation of the contents of a specific message which the subscriber holds.
        /// </summary>
        /// <param name="sequenceNumber">The identification number of the message</param>
        /// <returns>String of a JSON-object containing the message payload and headers</returns>
        public string GetMessageData(ulong sequenceNumber)
        {

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

        /// <summary>
        /// Gets a JSON representation of all the messages that the subscribers holds.
        /// </summary>
        /// <returns>String containing all of the JSON-objects</returns>
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