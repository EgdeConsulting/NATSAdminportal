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
        private readonly ILogger<JetStreamSubscriber> logger;
        private List<JetStreamSubscriber> allSubscribers;

        public SubscriberManager(ILogger<JetStreamSubscriber> logger, string? url)
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
                streamInfo.ForEach(a => allSubscribers.Add(new JetStreamSubscriber(logger, url, a.Config.Name, a.Config.Subjects)));
            }

            allSubscribers.ForEach(a => new Thread(a.Run).Start());

            // rawSubjects.Sort();
            // rawSubjects.ForEach(a => refinedSubjects.Add(a.Split(".")));

            // for (int i = 0; i < refinedSubjects.Count; i++)
            // {
            //     // Adding all unique subjects to list.
            //     for (int k = 0; k < refinedSubjects[i].Length; k++)
            //     {
            //         string subjectName = refinedSubjects[i][k];
            //         if (allSubjects.FirstOrDefault(x => x.SubjectName.Equals(subjectName)) is null)
            //             allSubjects.Add(new Subject(subjectName));
            //     }

            //     // Initializing the hierarchy links between all the subjects. 
            //     for (int k = 0; k < refinedSubjects[i].Length; k++)
            //     {
            //         string? parentName = k - 1 < 0 ? null : refinedSubjects[i][k - 1];
            //         string? childName = k + 1 > refinedSubjects[i].Length - 1 ? null : refinedSubjects[i][k + 1];
            //         AddSubjectLinks(refinedSubjects[i][k], parentName, childName, i);
            //     }
            // }

            
        }

        public string GetAllMessages()
        {
            string json = "[";
            for (int i = 0; i < allSubscribers.Count; i++)
            {
                JetStreamSubscriber sub = allSubscribers[i];
                json += sub.GetMessages();
                json = i < allSubscribers.Count - 1 ? json + "," : json;
            }
            
            return json + "]";
        }
    }

}