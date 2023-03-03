using NATS.Client;
using NATS.Client.JetStream;

namespace vite_api.Classes
{
    public static class Consumers
    {
        public static List<string> GetConsumerNamesForAStream(string? url, string streamname)
        {
            List<string> consumerInfo;

            using (IConnection c = new ConnectionFactory().CreateConnection(url))
            {
                IJetStreamManagement jsm = c.CreateJetStreamManagementContext();
                consumerInfo = jsm.GetConsumerNames(streamname).ToList<string>();
            }
            return consumerInfo;
        }
    }
}