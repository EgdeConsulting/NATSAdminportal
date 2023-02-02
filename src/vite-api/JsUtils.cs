// Copyright 2021 The NATS Authors
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NATS.Client;
using NATS.Client.JetStream;

namespace Backend.Logic
{
    public static class JsUtils
    {

        //     // ----------------------------------------------------------------------------------------------------
        //     // READ MESSAGES
        //     // ----------------------------------------------------------------------------------------------------
        //     public static IList<Msg> ReadMessagesAck(ISyncSubscription sub, bool verbose = true, int timeout = 1000)
        //     {
        //         if (verbose)
        //         {
        //             Console.Write("Read/Ack ->");
        //         }
        //         IList<Msg> messages = new List<Msg>();
        //         bool keepGoing = true;
        //         while (keepGoing)
        //         {
        //             try
        //             {
        //                 Msg msg = sub.NextMessage(timeout);
        //                 messages.Add(msg);
        //                 msg.Ack();
        //                 if (verbose)
        //                 {
        //                     Console.Write(" " + msg.Subject + " / " + Encoding.UTF8.GetString(msg.Data));
        //                 }
        //             }
        //             catch (NATSTimeoutException) // timeout means there are no messages available
        //             {
        //                 keepGoing = false;
        //             }
        //         }

        //         if (verbose)
        //         {
        //             Console.Write(messages.Count == 0 ? " No messages available <-\n" : " <-\n");
        //         }

        //         return messages;
        //     }

        //     // ----------------------------------------------------------------------------------------------------
        //     // REPORT
        //     // ----------------------------------------------------------------------------------------------------
        //     public static void Report(IList<Msg> list)
        //     {
        //         Console.Write("Fetch ->");
        //         foreach (Msg m in list)
        //         {
        //             Console.Write(" " + Encoding.UTF8.GetString(m.Data));
        //         }
        //         Console.Write(" <- \n");
        //     }

        //     private static readonly Random Random = new Random();
        //     private const string RandomChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
        //     private const string RandomIdChars = "0123456789abcdef";

        //     public static string RandomText()
        //     {
        //         return new string(Enumerable.Repeat(RandomChars, 20)
        //             .Select(s => s[Random.Next(s.Length)]).ToArray());
        //     }

        //     public static string RandomId()
        //     {
        //         return new string(Enumerable.Repeat(RandomIdChars, 6)
        //             .Select(s => s[Random.Next(s.Length)]).ToArray());
        //     }
    }
}