using NATS.Client;
using NATS.Client.JetStream;
using vite_api.Dto;

namespace vite_api.Classes
{
    public class StreamManager
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _provider;

        public StreamManager(ILogger<StreamManager> logger, IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        /// <summary>
        /// Deletes a message from a specific stream based on the sequence number of the message.
        /// </summary>
        /// <param name="streamName">Name of the stream which contains the message.</param>
        /// <param name="sequenceNumber">The sequence number of the message.</param>
        /// <param name="erase">Whether to erase the message (overwriting with garbage) or only mark it as erased.</param>
        /// <returns>True if the message was deleted successfully.</returns>
        public bool DeleteMessage(string streamName, ulong sequenceNumber, bool erase)
        {
            _logger.LogInformation("{} > {} deleted message (stream name, sequence number): {}, {}",
            DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), UserAccount.Name, streamName, sequenceNumber);

            using var connection = _provider.GetRequiredService<IConnection>();
            var jsm = connection.CreateJetStreamManagementContext();

            return jsm.DeleteMessage(streamName, sequenceNumber, erase);
        }

        /// <summary>
        /// Gets basic information about all streams on the NATS-server.
        /// </summary>
        /// <returns>A collection of Dto's containing the basic information.</returns>
        public BasicStreamInfoDto[] GetAllStreams()
        {
            using var connection = _provider.GetRequiredService<IConnection>();
            var jsm = connection.CreateJetStreamManagementContext();

            return jsm.GetStreams()
               .Select(x => new BasicStreamInfoDto
               {
                   Name = x.Config.Name,
                   SubjectCount = x.State.SubjectCount,
                   ConsumerCount = x.State.ConsumerCount,
                   MessageCount = x.State.Messages
               }).ToArray();
        }

        /// <summary>
        /// Gets extended information about a stream.
        /// </summary>
        /// <param name="streamName">The name of the stream.</param>
        /// <returns>Dto containing extended information about the stream.</returns>
        public ExtendedStreamInfoDto GetSpecificStream(string streamName)
        {
            using var connection = _provider.GetRequiredService<IConnection>();
            var jsm = connection.CreateJetStreamManagementContext();
            var streamInfo = jsm.GetStreamInfo(streamName);

            return new ExtendedStreamInfoDto
            {
                Name = streamInfo.Config.Name,
                Subjects = streamInfo.Config.Subjects,
                Consumers = jsm.GetConsumerNames(streamName).ToList(),
                Description = streamInfo.Config.Description,
                Messages = streamInfo.State.Messages,
                Deleted = streamInfo.State.DeletedCount,
                Policies = new PoliciesDto
                {
                    DiscardPolicy = streamInfo.Config.DiscardPolicy.GetString(),
                    RetentionPolicy = streamInfo.Config.RetentionPolicy.GetString()
                }
            };
        }

    }
}
