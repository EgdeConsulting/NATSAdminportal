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
    public static class Streams
    {
        public static string GetStreamNames(string? url)
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
                            StreamName = streamNames[i]
                        }
                    );
                    json = i < streamNames.Count - 1 ? json + "," : json;
                }
            }
            return json + "]";
        }

        public static string GetStreamSubjects(string? url) //Maybe better to collect the subjects from consumers?
        {
            List<StreamInfo> streamInfo;
            List<string> subjects = new List<string>();
            List<string[]> listOfSubjectArray = new List<string[]>();
            List<string> subjectsArr = new List<string>();
            List<string> numOfFirstOrderSub = new List<string>();
            List<string> numOfSecondOrderSub = new List<string>();
            List<string> numOfThirdOrderSub = new List<string>();
            List<string> numOfFourthOrderSub = new List<string>();
            string json = "[";

            using (IConnection c = new ConnectionFactory().CreateConnection(url))
            {
                IJetStreamManagement jsm = c.CreateJetStreamManagementContext();
                streamInfo = GetStreamInfoArray(jsm).ToList<StreamInfo>();

                for (int i = 0; i < streamInfo.Count; i++)
                {
                    subjects.AddRange(streamInfo[i].Config.Subjects); // Gets all subjects in form ["Subject.A.1", "Subject.A.2", ....]
                }
            }

            for (int i = 0; i < subjects.Count; i++)
            {
                listOfSubjectArray.Add(subjects[i].Split(".")); // Gets all subjects in form [[Subject, A, 1], [Subject, A, 2], ....]
            }

            string? secondLevel = null;
            string? temp = null;
            SubjectHierarchy.ClearSubjects();
            foreach (string[] element in listOfSubjectArray)
            {
                secondLevel = element[1];
                for (int i = 0; i < element.Length; i++)
                {
                    string? parent = i - 1 < 0 ? null : element[i - 1];
                    string? child = i + 1 > element.Length - 1 ? null : element[i + 1];
                    Subject sub = new Subject(element[i], parent!, child!);
                    // if (!SubjectHierarchy.SubjectExists(sub))
                    // {
                    SubjectHierarchy.AddSubject(sub);
                    // }
                }
                //Console.WriteLine(temp == element[1] || temp == null ? secondLevel + element[2] : "");
                temp = element[1];
            }
            SubjectHierarchy.GetHierarchy();


            // foreach (string[] element in listOfSubjectArray) //j = 0 on first order, j = 1 on second order ....
            // {
            //     if (!numOfFirstOrderSub.Contains(element[0]))
            //     {
            //         numOfFirstOrderSub.Add(element[0]);
            //     }
            // }

            // foreach (string firstOrderSub in numOfFirstOrderSub)
            // {
            //     foreach (string[] element in listOfSubjectArray)
            //     {
            //         if (element[0].Equals(firstOrderSub))
            //         {
            //             if (!numOfSecondOrderSub.Contains(element[1]))
            //             {
            //                 numOfSecondOrderSub.Add(element[1]);
            //             }
            //         }
            //     }
            // }


            // for (int i = 0; i < listOfSubjectArray.Count; i++)
            // {
            //     subjectsArr = listOfSubjectArray[i]; // [subject, A, 1, subject, A, 2, ....]

            //     // Desired outcome? [[subject, [[A, [1, 2]], [B, [1, 2]]]], [subject2, [A, C], [1, 2]]]] ?? 1 array per first order: chain1, chain2

            //     for (int j = 0; j < subjectsArr.Length; j++) //j = 0 on first order, j = 1 on second order ....
            //     {
            //         if (j == 0 && !numOfFirstOrderSub.Contains(subjectsArr[j]))
            //         {
            //             numOfFirstOrderSub.Add(subjectsArr[j]);
            //         }
            //         if (j == 1) //second order
            //         {

            //         }

            //         if (true) //potential
            //         {
            //             // string fourthRoot = "[";
            //             // string thirdRoot = "[";
            //             // string secondRoot = "[";
            //             // string firstRoot = "[";
            //             // Console.WriteLine(j + ": " + subjectsArr[j]);
            //             // if (j == 0)
            //             // {
            //             //     firstRoot += JsonSerializer.Serialize(
            //             //     new
            //             //     {
            //             //         name = subjectsArr[j],
            //             //         subSubjects = secondRoot + "]"
            //             //     }
            //             //     );
            //             // }
            //             // if (j == 1)
            //             // {
            //             //     secondRoot += JsonSerializer.Serialize(
            //             //     new
            //             //     {
            //             //         name = subjectsArr[j],
            //             //         subSubjects = thirdRoot + "]"
            //             //     }
            //             //     );
            //             // }
            //             // if (j == 2)
            //             // {
            //             //     thirdRoot += JsonSerializer.Serialize(
            //             //     new
            //             //     {
            //             //         name = subjectsArr[j],
            //             //         subSubjects = fourthRoot + "]"
            //             //     }
            //             //     );
            //             // }
            //             // if (j == 3)
            //             // {
            //             //     fourthRoot += JsonSerializer.Serialize(
            //             //     new
            //             //     {
            //             //         name = subjectsArr[j]
            //             //     }
            //             //     );
            //             // }
            //             // Console.WriteLine("Firstroot " + firstRoot);
            //             // Console.WriteLine("secondroot " + secondRoot);
            //             // Console.WriteLine("thirdroot " + thirdRoot);
            //             // Console.WriteLine("fourthroot " + fourthRoot);
            //         }
            //     }
            //     for (int j = 0; j < numOfFirstOrderSub.Count; j++)
            //     {
            //         // Console.WriteLine("COUNT: " + numOfFirstOrderSub.Count);
            //         // Console.WriteLine(i + "  " + j);
            //     }
            // }

            return json + "]";
        }

        public static string GetStreamInfo(string? url)
        {
            List<StreamInfo> streamInfo;
            string json = "[";

            using (IConnection c = new ConnectionFactory().CreateConnection(url))
            {
                IJetStreamManagement jsm = c.CreateJetStreamManagementContext();
                streamInfo = GetStreamInfoArray(jsm).ToList<StreamInfo>();

                for (int i = 0; i < streamInfo.Count; i++)
                {
                    json += JsonSerializer.Serialize(
                        new
                        {
                            StreamInfo = streamInfo[i]
                        }
                    );
                    json = i < streamInfo.Count - 1 ? json + "," : json;
                }
            }

            return json + "]";
        }

        public async static void CreateStreamFromRequest(HttpRequest request, string? url)
        {
            string content = "";

            using (StreamReader stream = new StreamReader(request.Body))
            {
                content = await stream.ReadToEndAsync();
            }

            var jsonObject = JsonNode.Parse(content);

            if (jsonObject != null && jsonObject["StreamName"] != null)
            {
                var streamName = jsonObject["StreamName"];
                //var subject = jsonObject["Subject"]!;

                if (streamName != null && !string.IsNullOrWhiteSpace(streamName.ToString()))
                {
                    using (IConnection c = new ConnectionFactory().CreateConnection(url))
                    {
                        IJetStreamManagement jsm = c.CreateJetStreamManagementContext();
                        Streams.CreateStreamWhenDoesNotExist(jsm, StorageType.File, streamName.ToString(), "Daniel");
                    }
                }
            }
        }
        public static StreamInfo? GetStreamInfoOrNullWhenNotExist(IJetStreamManagement jsm, string streamName)
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

        public static bool StreamExists(IJetStreamManagement jsm, string streamName)
        {
            return GetStreamInfoOrNullWhenNotExist(jsm, streamName) != null;
        }

        public static void ExitIfStreamExists(IJetStreamManagement jsm, string streamName)
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

        public static StreamInfo CreateStream(IJetStreamManagement jsm, string streamName, StorageType storageType, params string[] subjects)
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

        public static void CreateStreamWhenDoesNotExist(IJetStreamManagement jsm, StorageType storageType, string stream, params string[] subjects)
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

        public static StreamInfo CreateStreamOrUpdateSubjects(IJetStreamManagement jsm, string streamName, StorageType storageType, params string[] subjects)
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

        public static IList<string> GetStreamNamesArray(IJetStreamManagement jsm)
        {
            return jsm.GetStreamNames();
        }
        public static IList<StreamInfo> GetStreamInfoArray(IJetStreamManagement jsm)
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