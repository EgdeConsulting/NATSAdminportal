using System.Collections.Concurrent;
using System.Text;
using NATS.Client;
using NATS.Client.JetStream;
using vite_api.Dto;

namespace vite_api.Classes
{
    public class JetStreamSubscriber
    {
        private readonly List<string> _subjects;
        // The amount of messages which max should be pulled from a stream at the same time.
        private const int BatchSize = 1000;
        // Maximum character limit of the message payload.
        private const int MaxPayloadLength = 200;
        private readonly IServiceProvider _provider;
        public string StreamName { get; }

        public JetStreamSubscriber(IServiceProvider provider, string streamName, List<string> subjects)
        {
            _provider = provider;
            StreamName = streamName;
            _subjects = subjects;
        }

        // The following method was created based on: https://stackoverflow.com/questions/75181157/pull-last-batch-of-messages-from-a-nats-jetstream
        /// <summary>
        /// Bulk pulls all messages on all subjects from a stream. 
        /// </summary>
        /// <returns>List of all message objects</returns>
        private IEnumerable<Msg> ReceiveJetStreamPullSubscribe()
        {
            using var connection = _provider.GetRequiredService<IConnection>();
            var js = connection.CreateJetStreamContext();
            var pullOptions = PullSubscribeOptions.Builder().WithStream(StreamName).Build();

            var currentMessages = new ConcurrentBag<IList<Msg>>();
            Parallel.ForEach(_subjects, subject =>
            {
                var sub = js.PullSubscribe(subject, pullOptions);
                currentMessages.Add(sub.Fetch(BatchSize, 1000));
            });
            
            return currentMessages.SelectMany(x => x).ToList().OrderBy(x => x.Subject).ToList();
        }
        
        /// <summary>
        /// Gets an object representation of the contents of a specific message which the subscriber holds.
        /// </summary>
        /// <param name="sequenceNumber">The identification number of the message</param>
        /// <returns>Dto of a message object containing the payload and headers</returns>
        public MessageDataDto GetMessageData(ulong sequenceNumber)
        {
            var msg = ReceiveJetStreamPullSubscribe().First(x => x.MetaData.StreamSequence == sequenceNumber);

            List<MessageHeaderDto> msgHeaders = new();

            foreach (string headerName in msg.Header)
            {
                msgHeaders.AddRange(msg.Header.GetValues(headerName).Select(headerValue =>
                    new MessageHeaderDto()
                    {
                        Name = headerName,
                        Value = headerValue
                    }));
            }
            return new MessageDataDto()
            {
                Headers = msgHeaders,
                Payload = new MessagePayloadDto()
                {
                    Data = GetData(msg.Data, true)
                },
                Subject = msg.Subject
            };

        }
        /// <summary>
        /// Helper method to get payload and conditionally limit the length of it.
        /// </summary>
        /// <param name="data">The bytestream data of the payload</param>
        /// <param name="isShortPayload">Boolean to decide if the payload is to be shortened or not</param>
        /// <returns>The payload in full length or shortened</returns>
        private string GetData(byte[] data, bool isShortPayload)
        {
            var result = data.All(x => char.IsAscii((char)x)) ? Encoding.ASCII.GetString(data) : Convert.ToBase64String(data);
            return result.Length > MaxPayloadLength && isShortPayload ? result.Substring(0, MaxPayloadLength)
            + " ..." // 3 dots to indicate that the message is shortened.
            : result;
        }

        /// <summary>
        /// Gets an object representation of the full payload of a specific message which the subscriber holds.
        /// </summary>
        /// <param name="sequenceNumber">The identification number of the message</param>
        /// <returns>Dto of a payload object</returns>
        public MessagePayloadDto GetPayload(ulong sequenceNumber)
        {
            var msg = ReceiveJetStreamPullSubscribe().First(x => x.MetaData.StreamSequence == sequenceNumber);
            return new MessagePayloadDto()
            {
                Data = GetData(msg.Data, false)
            };
        }

        /// <summary>
        /// Gets an object representation of all the messages that the subscribers holds.
        /// </summary>
        /// <returns>List containing all of the message-objects</returns>
        public List<MessageDto> GetMessages()
        {
            return ReceiveJetStreamPullSubscribe().Select(x => new MessageDto
            {
                SequenceNumber = x.MetaData?.StreamSequence ?? UInt64.MinValue,
                Timestamp = x?.MetaData?.Timestamp ?? DateTime.Now,
                Stream = StreamName,
                Subject = x.Subject ?? "sub",
            }).ToList();
        }
    }
}