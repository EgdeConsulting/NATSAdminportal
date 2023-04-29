using System.Data.SqlTypes;
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
            using var connection = _provider.GetRequiredService<IConnection>();
            var jsm = connection.CreateJetStreamManagementContext();
            try
            {
                jsm.GetMessage(streamName, sequenceNumber);
            }
            catch
            {
                throw new ArgumentException("Given stream name or sequence number does not exist on the server");
            }
            _logger.LogInformation("{} > {} deleted message (stream name, sequence number): {}, {}",
                DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), UserAccount.Name, streamName, sequenceNumber);
            return jsm.DeleteMessage(streamName, sequenceNumber, erase);
        }

        /// <summary>
        /// Gets basic information about all streams on the NATS-server.
        /// </summary>
        /// <returns>A collection of BasicStreamInfoDto's containing the basic information.</returns>
        public List<BasicStreamInfoDto> GetAllStreams()
        {
            using var connection = _provider.GetRequiredService<IConnection>();
            var jsm = connection.CreateJetStreamManagementContext();

            return jsm.GetStreams()
               .Select(x => new BasicStreamInfoDto
               {
                   Name = x.Config.Name,
                   SubjectCount = x.State.SubjectCount,
                   ConsumerCount = FilterConsumers(jsm.GetConsumers(x.Config.Name).ToList()).Count,
                   MessageCount = x.State.Messages
               }).ToList();
        }

        /// <summary>
        /// Gets extended information about a stream.
        /// </summary>
        /// <param name="streamName">The name of the stream.</param>
        /// <returns>Dto containing extended information about the stream.</returns>
        public ExtendedStreamInfoDto GetSpecificStream(string streamName)
        {
            try
            {
                using var connection = _provider.GetRequiredService<IConnection>();
                var jsm = connection.CreateJetStreamManagementContext();
                var streamInfo = jsm.GetStreamInfo(streamName);

                return new ExtendedStreamInfoDto
                {
                    Name = streamInfo.Config.Name,
                    Subjects = streamInfo.Config.Subjects,
                    Consumers = FilterConsumers(jsm.GetConsumers(streamName).ToList()).Select(x => x.Name).ToList(),
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
            catch
            {
                throw new ArgumentException("Given stream name or sequence number does not exist on the server");
            }
        }

        /// <summary>
        /// Filters consumers based on the date they where created. Only consumers older than 10 minutes are being kept.
        /// </summary>
        /// <param name="consumers">List of consumers.</param>
        /// <returns>Filtered list.</returns>
        private static List<ConsumerInfo> FilterConsumers(IEnumerable<ConsumerInfo> consumers)
        {
            return consumers.Where(x =>
                DateTime.Now.Date > x.Created ||
                (DateTime.Now.ToUniversalTime() - x.Created.ToUniversalTime()).TotalHours >= 1 ||
                (DateTime.Now.ToUniversalTime() - x.Created.ToUniversalTime()).TotalMinutes > 10).ToList();
        }
    }
}
