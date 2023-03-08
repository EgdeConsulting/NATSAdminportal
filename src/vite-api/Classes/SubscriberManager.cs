using System.Text.Json;
using Microsoft.Extensions.Options;
using NATS.Client;
using NATS.Client.JetStream;
using vite_api.Config;
using vite_api.Dto;
using vite_api.Internal;

namespace vite_api.Classes
{
    public class SubscriberManager
    {
        private readonly IOptions<AppConfig> _appConfig;
        private readonly ILogger _logger;
        private readonly List<JetStreamSubscriber> _allSubscribers;
        private string Url => _appConfig.Value.NatsServerUrl ?? Defaults.Url;
        
        public SubscriberManager(IOptions<AppConfig> appConfig, ILogger<SubscriberManager> logger)
        {
            _appConfig = appConfig;
            _logger = logger;
            _allSubscribers = new List<JetStreamSubscriber>();
            InitializeSubscribers();
        }

        /// <summary>
        /// Creates and stores a JetStreamSubscriber-object for each stream on the specified NATS-server.
        /// </summary>
        private void InitializeSubscribers()
        {
            using var c = new ConnectionFactory().CreateConnection(Url);
            var streamInfo = c.CreateJetStreamManagementContext().GetStreams().ToList<StreamInfo>();
            streamInfo.ForEach(a => _allSubscribers.Add(new JetStreamSubscriber(Url, a.Config.Name, a.Config.Subjects)));
        }

        /// <summary>
        /// Gets an object representation of all the messages contained within all streams (JetStreamSubscribers).
        /// </summary>
        /// <returns>List containing Dto's of all messages</returns>
        public List<MessageDto> GetAllMessages()
        {
            _logger.LogInformation("{} > {} viewed all messages",
            DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), UserAccount.Name);

            var allMessages = new List<MessageDto>();
            foreach (var sub in _allSubscribers)
                allMessages.AddRange(sub.GetMessages());
            
            return allMessages;
        }
   
        /// <summary>
        /// Gets an object representation of the contents of a specific message on a specific stream.
        /// </summary>
        /// <param name="streamName">The name or identifier of the stream</param>
        /// <param name="sequenceNumber">The identification number of the message</param>
        /// <returns>A Dto containing the message payload and headers</returns>
        /// <exception cref="ArgumentException">If there isn't any message that matches the provided parameters</exception>
        public MessageDataDto? GetSpecificMessage(string streamName, ulong sequenceNumber)
        {
            _logger.LogInformation("{} > {} viewed message (stream, sequence number): {}, {}",
            DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), UserAccount.Name, streamName, sequenceNumber);

            foreach (var sub in _allSubscribers.Where(sub => sub.StreamName == streamName))
            {
                return sub.GetMessageData(sequenceNumber);
            }

            throw new ArgumentException(
                "There exists no message that matches provided stream name and sequence number!");
        }
    }
}