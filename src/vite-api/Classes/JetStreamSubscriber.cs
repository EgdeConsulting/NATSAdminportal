using System.Collections;
using System.Runtime.ExceptionServices;
using System.Text;
using NATS.Client;
using NATS.Client.JetStream;
using vite_api.Dto;

namespace vite_api.Classes
{
    public class JetStreamSubscriber
    {
        private readonly List<string> _subjects;
        // The amount of messages which max should be pulled from a stream at the same time
        private const int BatchSize = 1000;
        private const int MaxPayloadLength = 200;
        private readonly string? _url = Defaults.Url;
        private readonly string? _creds = null;
        public string StreamName { get; }

        public JetStreamSubscriber(string url, string streamName, List<string> subjects)
        {
            _url = url;
            StreamName = streamName;
            _subjects = subjects;
        }

        // /// <summary>
        // /// Starts the subscribers so that is fetches all previous, current and future messages on a stream. 
        // /// </summary>
        // /// 
        // public void Run()
        // {
        //     Options opts = ConnectionFactory.GetDefaultOptions();
        //     opts.Url = _url;
        //     if (_creds != null)
        //         opts.SetUserCredentials(_creds);
        //
        //     using (IConnection c = new ConnectionFactory().CreateConnection(opts))
        //     {
        //         //TimeSpan elapsed = receiveJetStreamPullSubscribe(c);
        //     }
        // }

        // The following method was created based on: https://stackoverflow.com/questions/75181157/pull-last-batch-of-messages-from-a-nats-jetstream
        /// <summary>
        /// Bulk pulls all messages on all subjects from a stream. 
        /// </summary>
        /// <param name="c">Nats connection</param>
        /// <returns>List of all message objects</returns>
        private IEnumerable<Msg> ReceiveJetStreamPullSubscribe()
        {
            var currentMessages = new List<Msg>();
            var opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = _url;
            
            // The default handlers write a newline for each event, pretty annoying.
            opts.ClosedEventHandler += (sender, args) => { };
            opts.DisconnectedEventHandler += (sender, args) => { };
            
            if (_creds != null)
                opts.SetUserCredentials(_creds);

            using (var c = new ConnectionFactory().CreateConnection(opts))
            {
                var js = c.CreateJetStreamContext();
                var pullOptions = PullSubscribeOptions.Builder().WithStream(StreamName).Build();

                foreach (var subject in _subjects)
                {
                    var sub = js.PullSubscribe(subject, pullOptions);
                    currentMessages = new List<Msg>(currentMessages.Concat(sub.Fetch(BatchSize, 1000)));
                }
            }
            return currentMessages.OrderByDescending(x => x.MetaData.StreamSequence).ToList();
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
                    Payload = GetData(msg.Data),
                    Subject = msg.Subject
                };

            
            static string GetData(byte[] data)
            {
                return data.All(x => char.IsAscii((char)x)) ? Encoding.ASCII.GetString(data) : Convert.ToBase64String(data);
            }
        }

        /// <summary>
        /// Gets an object representation of all the messages that the subscribers holds.
        /// </summary>
        /// <returns>List containing all of the message-objects</returns>
        public IEnumerable<MessageDto> GetMessages()
        {
            return ReceiveJetStreamPullSubscribe().Select(x => new MessageDto
            {
                SequenceNumber = x.MetaData.StreamSequence,
                Timestamp = x.MetaData.Timestamp,
                Stream = StreamName,
                Subject = x.Subject,
            }).ToList();
        }
    }
}