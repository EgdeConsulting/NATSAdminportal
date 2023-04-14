using System.Text;
using NATS.Client;
using vite_api.Dto;
using vite_api.Internal;

namespace vite_api.Classes
{
    public class Publisher
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _provider;
        private readonly int _count = 1;

        public Publisher(ILogger<Publisher> logger, IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        /// <summary>
        /// Creates and publishes a new message onto the NATS-server. 
        /// </summary>
        /// <param name="message">Message object containing all necessary information.</param>
        /// <exception cref="ArgumentException">The provided subject doesn't exist.</exception>
        public void SendMessage(MessageDataDto message)
        {
            MsgHeader msgHeader = new();
            foreach (var headerPair in message.Headers)
            {
                if (headerPair.Value == "")
                {
                    throw new ArgumentException("No header value was found for header name: " + headerPair.Name);
                }
                msgHeader.Add(headerPair.Name, headerPair.Value);
            }

            using var connection = _provider.GetRequiredService<IConnection>();
            
            Msg msg = new Msg(message.Subject, msgHeader, Encoding.UTF8.GetBytes(message.Payload.Data!));
            if (SubjectValidation.SubjectExists(connection, message.Subject!))
            {
                for (int i = 0; i < _count; i++)
                {
                    connection.Publish(msg);
                    _logger.LogInformation("{} > {} created a new message (subject, sequence number): {}, {}",
                        DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), UserAccount.Name, message.Subject, 1);
                }
            }
            else
            {
                throw new ArgumentException("Subject does not exist on server");
            }
            connection.Flush();
        }
        
        /// <summary>
        /// Creates a new message based on the contents of an existing message on a specified subject,
        /// and thereafter deletes the existing message.
        /// </summary>
        /// <param name="message">Message to be copied.</param>
        /// <param name="sequenceNumber">Sequence number of the copied message (used for logging).</param>
        /// <param name="newSubject">The subject under which the new message is created.</param>
        /// <exception cref="ArgumentException">The provided subject doesn't exist.</exception>
        public void CopyMessage(MessageDataDto message, ulong sequenceNumber, string newSubject)
        {
            MsgHeader msgHeader = new();
            foreach (var headerPair in message.Headers)
            {
                msgHeader.Add(headerPair.Name, headerPair.Value);
            }

            using var connection = _provider.GetRequiredService<IConnection>();

            Msg msg = new Msg(newSubject, msgHeader, Encoding.UTF8.GetBytes(message.Payload.Data!));
            if (SubjectValidation.SubjectExists(connection, newSubject))
            {
                for (int i = 0; i < _count; i++)
                {
                    connection.Publish(msg);
                }
                _logger.LogInformation("{} > {} copied message (old subject, old sequence number, new subject): {}, {}, {}",
                    DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), UserAccount.Name, message.Subject, sequenceNumber, newSubject);

            }
            else
            {
                throw new ArgumentException("Subject does not exist on server");
            }
            connection.Flush();
        }
    }
}
