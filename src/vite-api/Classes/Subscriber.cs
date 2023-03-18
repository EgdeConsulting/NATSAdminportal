// Copyright 2015-2018 The NATS Authors
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using NATS.Client;
using vite_api.Config;
using vite_api.Dto;
using vite_api.Repositories;
using Options = NATS.Client.Options;

namespace vite_api.Classes
{
    public class Subscriber
    {
        private readonly ILogger _logger;

        private readonly IOptions<AppConfig> _appConfig;

        // Use > to subscribe to all subjects
        public string _subject = ">";
        private int _count = 1000000;
        private SubjectManager subjectManager;
        private readonly MessageRepository _msgRepo;
        private bool _sync = true;
        private int _received = 0;
        private bool _verbose = true;
        private string? _creds = null;
        private List<Msg> _allMessages => _msgRepo.GetMessages();
        private List<DateTime> _timestamps => _msgRepo.GetTimestamps();

        public string MessageSubject
        {
            get; set;
        }

        public Subscriber(ILogger<Subscriber> logger, IOptions<AppConfig> appConfig, SubjectManager subjectManager, MessageRepository msgRepo)
        {
            this._logger = logger;
            this._appConfig = appConfig;
            MessageSubject = ">";
            this.subjectManager = subjectManager;
            _msgRepo = msgRepo;
        }

        public void Run()
        {
            Options opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = _appConfig.Value.NatsServerUrl ?? Defaults.Url;
            if (_creds != null)
                opts.SetUserCredentials(_creds);

            using (IConnection c = new ConnectionFactory().CreateConnection(opts))
            {
                TimeSpan elapsed = _sync ? receiveSyncSubscriber(c) : receiveAsyncSubscriber(c);
            }
        }

        private TimeSpan receiveAsyncSubscriber(IConnection c)
        {
            Stopwatch sw = new Stopwatch();
            Object testLock = new Object();

            EventHandler<MsgHandlerEventArgs> msgHandler = (sender, args) =>
            {
                if (_received == 0)
                    sw.Start();

                _received++;

                if (_verbose)
                    Console.WriteLine("Received: " + args.Message);

                if (_received >= _count)
                {
                    sw.Stop();
                    lock (testLock)
                        Monitor.Pulse(testLock);
                }
            };

            using (IAsyncSubscription s = c.SubscribeAsync(_subject, msgHandler))
            {
                // just wait until we are done.
                lock (testLock)
                    Monitor.Wait(testLock);
            }

            return sw.Elapsed;
        }

        private void AddMessage(Msg lastMessage)
        {
            if (subjectManager.SubjectExists(lastMessage.Subject))
            {
                // _allMessages.Insert(0, lastMessage);
                // _timestamps.Insert(0, DateTime.Now);
            }
        }

        // public List<MessageDto> GetMessages2()
        // {
        //     var res = _msgRepo.GetAll().Select(x => new MessageDto
        //     {
        //         SequenceNumber = 2323,
        //         Timestamp = DateTime.Now,
        //         Stream = "Test",
        //         Subject = "Subject",
        //     }).ToList();
        //
        //     return res;
        //
        //     static string GetData(byte[] data)
        //     {
        //         if (data.All(x => char.IsAscii((char) x)))
        //             return Encoding.ASCII.GetString(data);
        //
        //         return Convert.ToBase64String(data);
        //     }
        // }


        public string GetMessages()
        {
            _logger.LogInformation("{} > {} viewed all messages",
            DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), UserAccount.Name);

            string json = "[";
            for (int i = 0; i < _allMessages.Count; i++)
            {
                Msg msg = _allMessages[i];
                string timestamp = _timestamps[i].ToString("MM/dd/yyyy HH:mm:ss");

                string headerData = "[";

                var enu = msg.Header.GetEnumerator();
                int index = 0;

                while (enu.MoveNext())
                {
                    headerData += JsonSerializer.Serialize(
                        new
                        {
                            name = enu.Current,
                            value = msg.Header[enu.Current.ToString()],
                        }
                    );
                    headerData = index < msg.Header.Count - 1 ? headerData + "," : headerData;
                    index++;
                }

                headerData += "]";

                Console.WriteLine(msg.MetaData);
                json += JsonSerializer.Serialize(
                    new
                    {
                        subject = msg.Subject,
                        timestamp = timestamp,
                        acknowledgement = msg.LastAck,
                        headers = headerData, //msg.MetaData has potential but throws objectreference error
                        // Checks if any characters are not ASCII.
                        payload = msg.Data.All(b => b >= 32 && b <= 127) ? Encoding.ASCII.GetString(msg.Data) : msg.Data.ToString()
                    }
                );

                json = i < _allMessages.Count - 1 ? json + "," : json;
            }
            
            return json + "]";
        }

        private TimeSpan receiveSyncSubscriber(IConnection c)
        {
            using (ISyncSubscription s = c.SubscribeSync(_subject))
            {
                Stopwatch sw = new Stopwatch();

                while (_received < _count)
                {
                    if (_received == 0)
                        sw.Start();

                    Msg m = s.NextMessage();
                    _received++;

                    if (string.Equals(MessageSubject, ">") || string.Equals(MessageSubject, m.Subject))
                        AddMessage(m);
                }

                sw.Stop();

                return sw.Elapsed;
            }
        }
    }
}
