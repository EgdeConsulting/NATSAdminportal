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

using System.Text;
using Microsoft.Extensions.Options;
using NATS.Client;
using vite_api.Config;
using vite_api.Dto;
using Options = NATS.Client.Options;

namespace vite_api.Classes
{
    public class Publisher
    {
        private readonly ILogger _logger;
        private readonly IOptions<AppConfig> _appConfig;
        private readonly IServiceProvider _provider;
        private readonly int _count = 1;

        public Publisher(ILogger<Publisher> logger, IOptions<AppConfig> appConfig, IServiceProvider provider)
        {
            _logger = logger;
            _appConfig = appConfig;
            _provider = provider;
        }

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


        // private void printStats(IConnection c)
        // {
        //     IStatistics s = c.Stats;
        //     Console.WriteLine("Statistics:  ");
        //     Console.WriteLine("   Outgoing Payload Bytes: {0}", s.OutBytes);
        //     Console.WriteLine("   Outgoing Messages: {0}", s.OutMsgs);
        // }

        // private void usage()
        // {
        //     Console.Error.WriteLine(
        //         "Usage:  Publish [-url url] [-subject subject] " +
        //         "[-count count] [-creds file] [-payload payload]");

        //     Environment.Exit(-1);
        // }

        // private void parseArgs(string[] args)
        // {
        //     if (args == null)
        //         return;

        //     for (int i = 0; i < args.Length; i++)
        //     {
        //         if (i + 1 == args.Length)
        //             usage();

        //         parsedArgs.Add(args[i], args[i + 1]);
        //         i++;
        //     }

        //     if (parsedArgs.ContainsKey("-count"))
        //         count = Convert.ToInt32(parsedArgs["-count"]);

        //     if (parsedArgs.ContainsKey("-url"))
        //         url = parsedArgs["-url"];

        //     if (parsedArgs.ContainsKey("-subject"))
        //         subject = parsedArgs["-subject"];

        //     if (parsedArgs.ContainsKey("-payload"))
        //         payload = Encoding.UTF8.GetBytes(parsedArgs["-payload"]);

        //     if (parsedArgs.ContainsKey("-creds"))
        //         creds = parsedArgs["-creds"];
        // }

        // private void banner()
        // {
        //     Console.WriteLine("Publishing {0} messages on subject {1}",
        //         count, subject);
        //     Console.WriteLine("  Url: {0}", url);
        //     Console.WriteLine("  Subject: {0}", subject);
        //     Console.WriteLine("  Count: {0}", count);
        //     Console.WriteLine("  Payload is {0} bytes.",
        //         payload != null ? payload.Length : 0);
        // }
    }
}
