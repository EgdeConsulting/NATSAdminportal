using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using NATS.Client;
using NATS.Client.JetStream;
using vite_api.Config;
using vite_api.Dto;

namespace vite_api.Classes
{
    public class StreamManager
    {
        private readonly ILogger _logger;
        private readonly IOptions<AppConfig> _appConfig;
        private readonly IServiceProvider _provider;

        private string Url => _appConfig.Value.NatsServerUrl ?? Defaults.Url;

        public StreamManager(ILogger<StreamManager> logger, IOptions<AppConfig> appConfig, IServiceProvider provider)
        {
            _logger = logger;
            _appConfig = appConfig;
            _provider = provider;
        }

        /// <summary>
        /// Gets the subject names of all subjects that reside within all streams.
        /// </summary>
        public List<string[]> GetStreamSubjects()
        {
            List<StreamInfo> streamInfo;
            List<string> subjects = new List<string>();
            List<string[]> listOfSubjectArray = new List<string[]>();

            var url = _appConfig.Value.NatsServerUrl ?? Defaults.Url;
            using (IConnection c = new ConnectionFactory().CreateConnection(url))
            {
                IJetStreamManagement jsm = c.CreateJetStreamManagementContext();
                streamInfo = GetStreamInfoArray(jsm).ToList<StreamInfo>();

                for (int i = 0; i < streamInfo.Count; i++)
                    // Gets all subjects in form ["Subject.A.1", "Subject.A.2", ....]
                    subjects.AddRange(streamInfo[i].Config.Subjects);

                subjects.Sort();
            }

            for (int i = 0; i < subjects.Count; i++)
                // Gets all subjects in form [[Subject, A, 1], [Subject, A, 2], ....]
                listOfSubjectArray.Add(subjects[i].Split("."));

            return listOfSubjectArray;
        }

        /// <summary>
        /// Deletes a message from a stream based on the message sequence number.
        /// </summary>
        public bool DeleteMessage(string streamName, ulong sequenceNumber, bool erase)
        {
            _logger.LogInformation("{} > {} deleted message (stream name, sequence number): {}, {}",
            DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), UserAccount.Name, streamName, sequenceNumber);

            using (IConnection c = new ConnectionFactory().CreateConnection(Url))
            {
                IJetStreamManagement jsm = c.CreateJetStreamManagementContext();
                return jsm.DeleteMessage(streamName, sequenceNumber, erase);
            }
        }
        
        /// <summary>
        /// Gets extended information about a all stream on a NATS-server. Returns JSON.
        /// </summary>
        public BasicStreamInfoDto[] GetBasicStreamInfo()
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
        /// Creates a stream from a HttpRequest.         
        /// </summary>
        /// <param name="request">This request contains the name of the stream and its subjects.</param>
        public ExtendedStreamInfoDto GetExtendedStreamInfo(string streamName)
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

        public async void CreateStreamFromRequest(HttpRequest request)
        {
            string content = "";

            using (StreamReader stream = new StreamReader(request.Body))
            {
                content = await stream.ReadToEndAsync();
            }

            var jsonObject = JsonNode.Parse(content);

            if (jsonObject != null && jsonObject["name"] != null)
            {
                var streamName = jsonObject["name"];
                //var subject = jsonObject["Subject"]!;

                if (streamName != null && !string.IsNullOrWhiteSpace(streamName.ToString()))
                {
                    using (IConnection c = new ConnectionFactory().CreateConnection(Url))
                    {
                        IJetStreamManagement jsm = c.CreateJetStreamManagementContext();
                        CreateStreamWhenDoesNotExist(jsm, StorageType.File, streamName.ToString(), "Daniel");
                    }
                }
            }
        }

        public StreamInfo? GetStreamInfoOrNullWhenNotExist(IJetStreamManagement jsm, string streamName)
        {
            try
            {
                return jsm.GetStreamInfo(streamName);
            }
            catch (NATSJetStreamException e)
            {
                if (e.ErrorCode == 404)
                {
                    return null;
                }
                throw;
            }
        }

        // public static bool StreamExists(IConnection c, string streamName)
        // {
        //     return GetStreamInfoOrNullWhenNotExist(c.CreateJetStreamManagementContext(), streamName) != null;
        // }

        public bool StreamExists(IJetStreamManagement jsm, string streamName)
        {
            return GetStreamInfoOrNullWhenNotExist(jsm, streamName) != null;
        }

        public void ExitIfStreamExists(IJetStreamManagement jsm, string streamName)
        {
            if (StreamExists(jsm, streamName))
            {
                Console.WriteLine($"\nThe example cannot run since the stream '{streamName}' already exists.\n" +
                                  "It depends on the stream being in a new state. You can either:\n" +
                                  "  1) Change the stream name in the example.\n  2) Delete the stream.\n  3) Restart the server if the stream is a memory stream.");
                Environment.Exit(-1);
            }
        }

        // public static void ExitIfStreamNotExists(IConnection c, string streamName)
        // {
        //     if (!StreamExists(c, streamName))
        //     {
        //         Console.WriteLine("\nThe example cannot run since the stream '" + streamName + "' does not exist.\n" +
        //                            "It depends on the stream existing and having data.");
        //         Environment.Exit(-1);
        //     }
        // }

        public StreamInfo CreateStream(IJetStreamManagement jsm, string streamName, StorageType storageType, params string[] subjects)
        {
            // Create a stream, here will use a file storage type, and one subject,
            // the passed subject.
            StreamConfiguration sc = StreamConfiguration.Builder()
                .WithName(streamName)
                .WithStorageType(storageType)
                .WithSubjects(subjects)
                .Build();

            // Add or use an existing stream.
            StreamInfo si = jsm.AddStream(sc);
            Console.WriteLine("Created stream '{0}' with subject(s) [{1}]\n", streamName, string.Join(",", si.Config.Subjects));
            return si;
        }

        // public static StreamInfo CreateStream(IJetStreamManagement jsm, string stream, params string[] subjects)
        // {
        //     return CreateStream(jsm, stream, StorageType.Memory, subjects);
        // }

        // public static StreamInfo CreateStream(IConnection c, string stream, params string[] subjects)
        // {
        //     return CreateStream(c.CreateJetStreamManagementContext(), stream, StorageType.Memory, subjects);
        // }

        // public static StreamInfo CreateStreamExitWhenExists(IConnection c, string streamName, params string[] subjects)
        // {
        //     return CreateStreamExitWhenExists(c.CreateJetStreamManagementContext(), streamName, subjects);
        // }

        // public static StreamInfo CreateStreamExitWhenExists(IJetStreamManagement jsm, string streamName, params string[] subjects)
        // {
        //     ExitIfStreamExists(jsm, streamName);
        //     return CreateStream(jsm, streamName, StorageType.Memory, subjects);
        // }

        public void CreateStreamWhenDoesNotExist(IJetStreamManagement jsm, StorageType storageType, string stream, params string[] subjects)
        {
            try
            {
                jsm.GetStreamInfo(stream); // this throws if the stream does not exist
                Console.WriteLine("Stream already exists");
                return;
            }
            catch (NATSJetStreamException)
            {
                /* stream does not exist */
            }

            StreamConfiguration sc = StreamConfiguration.Builder()
                .WithName(stream)
                .WithStorageType(storageType)
                .WithSubjects(subjects)
                .Build();
            jsm.AddStream(sc);
        }

        // public static void CreateStreamWhenDoesNotExist(IConnection c, string stream, params string[] subjects)
        // {
        //     CreateStreamWhenDoesNotExist(c.CreateJetStreamManagementContext(), stream, subjects);
        // }

        public StreamInfo CreateStreamOrUpdateSubjects(IJetStreamManagement jsm, string streamName, StorageType storageType, params string[] subjects)
        {

            StreamInfo? si = GetStreamInfoOrNullWhenNotExist(jsm, streamName);
            if (si == null)
            {
                return CreateStream(jsm, streamName, storageType, subjects);
            }

            // check to see if the configuration has all the subject we want
            StreamConfiguration sc = si.Config;
            bool needToUpdate = false;
            foreach (string sub in subjects)
            {
                if (!sc.Subjects.Contains(sub))
                {
                    needToUpdate = true;
                    sc.Subjects.Add(sub);
                }
            }

            if (needToUpdate)
            {
                si = jsm.UpdateStream(sc);
                Console.WriteLine("Existing stream '{0}' was updated, has subject(s) [{1}]\n",
                    streamName, string.Join(",", si.Config.Subjects));
                // Existing stream 'scratch'  [sub1, sub2]
            }
            else
            {
                Console.WriteLine("Existing stream '{0}' already contained subject(s) [{1}]\n",
                    streamName, string.Join(",", si.Config.Subjects));
            }

            return si;
        }

        public IList<string> GetStreamNamesArray(IJetStreamManagement jsm)
        {
            return jsm.GetStreamNames();
        }
        public IList<StreamInfo> GetStreamInfoArray(IJetStreamManagement jsm)
        {
            return jsm.GetStreams();
        }

        // public static StreamInfo CreateStreamOrUpdateSubjects(IJetStreamManagement jsm, string streamName, params string[] subjects)
        // {
        //     return CreateStreamOrUpdateSubjects(jsm, streamName, StorageType.Memory, subjects);
        // }

        // public static StreamInfo CreateStreamOrUpdateSubjects(IConnection c, string stream, params string[] subjects)
        // {
        //     return CreateStreamOrUpdateSubjects(c.CreateJetStreamManagementContext(), stream, StorageType.Memory, subjects);
        // }
    }
}
