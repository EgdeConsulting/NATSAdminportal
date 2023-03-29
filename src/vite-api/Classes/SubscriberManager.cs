using System.Collections.Concurrent;
using NATS.Client;
using NATS.Client.JetStream;
using vite_api.Dto;

namespace vite_api.Classes
{
    public class SubscriberManager
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _provider;
        private readonly List<JetStreamSubscriber> _allSubscribers;

        public SubscriberManager(ILogger<SubscriberManager> logger, IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
            _allSubscribers = new List<JetStreamSubscriber>();
            InitializeSubscribers();
        }

        /// <summary>
        /// Creates and stores a JetStreamSubscriber-object for each stream on the specified NATS-server.
        /// </summary>
        private void InitializeSubscribers()
        {
            using var connection = _provider.GetRequiredService<IConnection>();
            var streamInfo = connection.CreateJetStreamManagementContext().GetStreams().ToList<StreamInfo>();
            streamInfo.ForEach(a => _allSubscribers.Add(new JetStreamSubscriber(_provider, a.Config.Name, a.Config.Subjects)));
        }

        /// <summary>
        /// Gets an object representation of all the messages contained within all streams (JetStreamSubscribers).
        /// </summary>
        /// <returns>List containing Dto's of all messages</returns>
        public List<MessageDto> GetAllMessages()
        {
            try
            {
                _logger.LogInformation("{} > {} viewed all messages",
                DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), UserAccount.Name);

                var allMessages = new ConcurrentBag<List<MessageDto>>();
                Parallel.ForEach(_allSubscribers, sub => { allMessages.Add(sub.GetMessages()); });

                return allMessages.SelectMany(x => x).ToList().OrderBy(x => x.Stream).ThenByDescending(x => x.SequenceNumber).ToList();
            }
            catch (Exception ex)
            {
                throw new AggregateException(ex.Message);
            }
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

            var sub = _allSubscribers.FirstOrDefault(sub => sub.StreamName == streamName);
            if (sub != null)
                return sub.GetMessageData(sequenceNumber);

            throw new ArgumentException("There exists no message that matches provided stream name and sequence number!");
        }
    }
}