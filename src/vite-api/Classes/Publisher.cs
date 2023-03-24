using System.Text;
using NATS.Client;
using vite_api.Dto;

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
        public void SendNewMessage(MessageDataDto message)
        {
            _logger.LogInformation("{} > {} created a new message (subject, sequence number): {}, {}", 
            DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), UserAccount.Name ,message.Subject, 1);
            
            MsgHeader msgHeader = new();
            foreach (var headerPair in message.Headers)
            {
                msgHeader.Add(headerPair.Name, headerPair.Value);
            }
            
            using var connection = _provider.GetRequiredService<IConnection>();

            if (message.Payload != null)
            {
                Msg msg = new Msg(message.Subject, msgHeader, Encoding.UTF8.GetBytes(message.Payload));
                for (int i = 0; i < _count; i++)
                {
                    connection.Publish(msg);
                }
            }

            connection.Flush();
        }
        
        /// <summary>
        /// Creates a new message based on the contents of an existing message on a specified subject,
        /// and thereafter deletes the existing message.
        /// </summary>
        /// <param name="message">Message to be copied.</param>
        /// <param name="newSubject">The subject under which the new message is created.</param>
        public void CopyMessage(MessageDataDto message, string newSubject)
        {
            _logger.LogInformation("{} > {} copied message (old subject, new subject, sequence number): {}, {}, {}", 
                DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), UserAccount.Name ,message.Subject, newSubject, 1);

            MsgHeader msgHeader = new();
            foreach (var headerPair in message.Headers)
            {
                msgHeader.Add(headerPair.Name, headerPair.Value);
            }
            
            using var connection = _provider.GetRequiredService<IConnection>();

            Msg msg = new Msg(newSubject, msgHeader, Encoding.UTF8.GetBytes(message.Payload!));
            for (int i = 0; i < _count; i++)
            {
                connection.Publish(msg);
            }
            connection.Flush();
        }
    }
}
