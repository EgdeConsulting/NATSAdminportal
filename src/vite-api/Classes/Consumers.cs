using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using NATS.Client;
using NATS.Client.JetStream;

namespace Backend.Logic
{
    public static class Consumers
    {
        public static string GetConsumerNamesForAStream(string? url, string streamname)
        {
            List<string> consumerInfo;
            string json = "[";

            using (IConnection c = new ConnectionFactory().CreateConnection(url))
            {
                IJetStreamManagement jsm = c.CreateJetStreamManagementContext();
                consumerInfo = jsm.GetConsumerNames(streamname).ToList<string>();
                for (int i = 0; i < consumerInfo.Count; i++)
                {
                    json += JsonSerializer.Serialize(
                        new
                        {
                            ConsumerInfo = consumerInfo[i]
                        }
                    );
                    json = i < consumerInfo.Count - 1 ? json + "," : json;
                }
            }

            return json + "]";
        }
    }
}