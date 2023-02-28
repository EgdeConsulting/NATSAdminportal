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

        private void InitializeSubscribers(string? url)
        {
            List<string> rawSubjects = new List<string>();
            List<string[]> refinedSubjects = new List<string[]>();

            using (IConnection c = new ConnectionFactory().CreateConnection(url))
            {
                List<StreamInfo> streamInfo = c.CreateJetStreamManagementContext().GetStreams().ToList<StreamInfo>();

                // Gets all subjects in form ["Subject.A.1", "Subject.A.2", ....]
                streamInfo.ForEach(a => allSubscribers.Add(new JetStreamSubscriber(url, a.Config.Name, a.Config.Subjects)));
            }

            allSubscribers.ForEach(a => new Thread(a.Run).Start());            
        }

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