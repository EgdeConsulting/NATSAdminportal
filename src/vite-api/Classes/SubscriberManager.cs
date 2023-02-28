using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.Json;
using NATS.Client.JetStream;
using NATS.Client;

namespace Backend.Logic
{
    public class SubscriberManager
    {
        private readonly ILogger logger;
        private List<JetStreamSubscriber> allSubscribers;

        public SubscriberManager(ILogger<SubscriberManager> logger, string? url)
        {
            this.logger = logger;
            allSubscribers = new List<JetStreamSubscriber>();
            InitializeSubscribers(url);
        }

        /// <summary>
        /// Creates and stores a JetStreamSubscriber-object for each stream on the specified NATS-server.
        /// </summary>
        /// <param name="url">Url of the NATS-server</param>
        private void InitializeSubscribers(string? url)
        {
            using (IConnection c = new ConnectionFactory().CreateConnection(url))
            {
                List<StreamInfo> streamInfo = c.CreateJetStreamManagementContext().GetStreams().ToList<StreamInfo>();
                streamInfo.ForEach(a => allSubscribers.Add(new JetStreamSubscriber(url, a.Config.Name, a.Config.Subjects)));
            }

            allSubscribers.ForEach(a => new Thread(a.Run).Start());
        }

        /// <summary>
        /// Gets a JSON representation of all the messages contained within all streams (JetStreamSubscribers).
        /// </summary>
        /// <returns>String containing all of the JSON-objects</returns>
        public string GetAllMessages()
        {
            logger.LogInformation("{} > {} viewed all messages",
            DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), UserAccount.Name);

            string json = "[";
            for (int i = 0; i < allSubscribers.Count; i++)
            {
                JetStreamSubscriber sub = allSubscribers[i];
                json += sub.GetMessages();
                json = i < allSubscribers.Count - 1 ? json + "," : json;
            }

            return json + "]";
        }

        /// <summary>
        /// Gets a JSON representation of the contents of a specific message on a specific stream.
        /// </summary>
        /// <param name="streamName">The name or identifier of the stream</param>
        /// <param name="sequenceNumber">The identification number of the message</param>
        /// <returns>String of a JSON-object containing the message payload and headers</returns>
        public string GetSpecificMessage(string streamName, ulong sequenceNumber)
        {
            logger.LogInformation("{} > {} viewed message (stream, sequence number): {}, {}",
            DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), UserAccount.Name, streamName, sequenceNumber);

            foreach (JetStreamSubscriber sub in allSubscribers)
            {
                if (sub.StreamName == streamName)
                    return sub.GetMessageData(sequenceNumber);
            }

            return "";
        }
    }
}