using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using NATS.Client;
using NATS.Client.JetStream;

namespace Backend.Logic
{
    public class StreamManager
    {
        private string? url = Defaults.Url;

        public StreamManager(string? url)
        {
            this.url = url;
        }

        public List<string[]> GetStreamSubjects()
        {
            List<StreamInfo> streamInfo;
            List<string> subjects = new List<string>();
            List<string[]> listOfSubjectArray = new List<string[]>();

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

        public bool DeleteMessage(string streamName, ulong sequenceNumber, bool erase)
        {
            using (IConnection c = new ConnectionFactory().CreateConnection(url))
            {
                IJetStreamManagement jsm = c.CreateJetStreamManagementContext();
                return jsm.DeleteMessage(streamName, sequenceNumber, erase);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GetStreamNames()
        {
            List<string> streamNames;
            string json = "[";

            using (IConnection c = new ConnectionFactory().CreateConnection(url))
            {
                IJetStreamManagement jsm = c.CreateJetStreamManagementContext();
                streamNames = GetStreamNamesArray(jsm).ToList<string>();

                for (int i = 0; i < streamNames.Count; i++)
                {
                    json += JsonSerializer.Serialize(
                        new
                        {
                            name = streamNames[i]
                        }
                    );
                    json = i < streamNames.Count - 1 ? json + "," : json;
                }
            }
            return json + "]";
        }

        public string GetBasicStreamInfo()
        {
            string json = "[";
            List<StreamInfo> streamInfo;

            using (IConnection c = new ConnectionFactory().CreateConnection(url))
            {
                IJetStreamManagement jsm = c.CreateJetStreamManagementContext();
                streamInfo = GetStreamInfoArray(jsm).ToList<StreamInfo>();

                for (int i = 0; i < streamInfo.Count; i++)
                {
                    json += JsonSerializer.Serialize(
                        new
                        {
                            name = streamInfo[i].Config.Name,
                            subjectsCount = streamInfo[i].State.SubjectCount,
                            consumersCount = streamInfo[i].State.ConsumerCount,
                            messageCount = streamInfo[i].State.Messages,

                        }
                    );
                    json = i < streamInfo.Count - 1 ? json + "," : json;
                }
            }
            return json + "]";
        }
        public string GetExtendedStreamInfo(string streamName)
        {
            string json = "[";
            List<Dictionary<string, string>> policies = new List<Dictionary<string, string>>();
            Dictionary<string, string> discPol = new Dictionary<string, string>();
            Dictionary<string, string> retPol = new Dictionary<string, string>();
            StreamInfo streamInfo;

            using (IConnection c = new ConnectionFactory().CreateConnection(url))
            {
                IJetStreamManagement jsm = c.CreateJetStreamManagementContext();
                streamInfo = jsm.GetStreamInfo(streamName);
                discPol.Add("DiscardPolicy", streamInfo.Config.DiscardPolicy.GetString());
                retPol.Add("RetentionPolicy", streamInfo.Config.RetentionPolicy.GetString());
                policies.Add(new Dictionary<string, string>(discPol));
                policies.Add(new Dictionary<string, string>(retPol));
            }
            json += JsonSerializer.Serialize(
                new
                {
                    Name = streamName,
                    Subjects = streamInfo.Config.Subjects,
                    Consumers = Consumers.GetConsumerNamesForAStream(url, streamName), // NEED TO GET THIS FROM CONSUMER.CS
                    Description = streamInfo.Config.Description,
                    Messages = streamInfo.State.Messages, //Also need to get this from somewhere..... CLI: nats stream view -s ip:port, check https://github.com/nats-io/nats.net/blob/master/src/Samples/JetStreamManageStreams/JetStreamManageStreams.cs
                    Deleted = streamInfo.State.DeletedCount,
                    Policies = policies,
                }
            );
            return json + "]";
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
                    using (IConnection c = new ConnectionFactory().CreateConnection(url))
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