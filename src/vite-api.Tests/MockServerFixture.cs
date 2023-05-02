using System.Diagnostics;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using EmptyFiles;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NATS.Client;
using NATS.Client.JetStream;
using vite_api.Dto;

namespace vite_api.Tests;

[CollectionDefinition("MockServer collection")]
public class MockServerFixture : ICollectionFixture<MockServerFixture>
{
    
    private readonly List<string[]> _headers = new()
    {
        new[] { "Header1", "Value1" },
        new[] { "Header2", "Header2" },
        new[] { "Header3", "Value3" }
    };
    private readonly string[] _payloads = {
        @"A very long Payload that is over 200 characters. Lorem ipsum dolor sit amet, consectetur 
        adipiscing elit. Phasellus commodo varius malesuada. Pellentesque habitant morbi tristique 
        senectus et netus et malesuada fames ac turpis egestas. Etiam nec nisi iaculis, vehicula 
        lectus vel, lacinia odio. Nulla commodo augue mattis dui elementum interdum. Vivamus in leo 
        est. Etiam rutrum magna at elit tempus, ut dignissim mi elementum. Vestibulum varius laoreet 
        pharetra. Aliquam eros quam, tincidunt id metus eu, rutrum lobortis justo. Curabitur porttitor 
        orci a sapien sollicitudin pretium. In aliquet elit convallis nibh varius fermentum. Nunc 
        purus tellus, egestas in sollicitudin vitae, vehicula non ligula. Vivamus ac consectetur nunc.",
        "A payload that is not over 200 characters.",
        "The third payload."
    };
    public string StreamName => "Stream";
    public string InvalidStreamName => "InvalidStream";
    public string PrimarySubject => "PrimarySubject";
    public string SecondarySubject => "SecondarySubject";
    public string InvalidSubject => "InvalidSubject";
    public string[] ValidSubjects { get; }
    public List<MessageDataDto> MsgDataDtos { get; }
    public List<Msg> Messages { get; }
    public Mock<IServiceProvider> Provider { get; }
    public Mock<IConnection> IConnection { get; }
    public Mock<IJetStream> JetStream { get; }
    public Mock<IJetStreamPullSubscription> PullSubscription { get; }
    
    public MockServerFixture()
    {
        ValidSubjects = new[]
        {
            PrimarySubject,
            SecondarySubject,
            PrimarySubject + ".A",
            PrimarySubject + ".A.1",
            PrimarySubject + ".A.2",
            SecondarySubject + ".B",
            SecondarySubject + ".B.1",
            SecondarySubject + ".C",
            SecondarySubject + ".C.1",
            SecondarySubject + ".C.2"
        };
        
        MsgDataDtos = MockMessageDataDtos();
        Messages = MockMessages();
        
        PullSubscription = new Mock<IJetStreamPullSubscription>();
        PullSubscription.Setup(x => x.Fetch(It.IsAny<int>(), It.IsAny<int>()))
            .Returns<int, int>((batchSize, maxWaitMillis) => Messages);
        
        JetStream = new Mock<IJetStream>();
        JetStream.Setup(x => x.PullSubscribe(It.IsAny<string>(), It.IsAny<PullSubscribeOptions>())).Returns(PullSubscription.Object);
        
        IConnection = new Mock<IConnection>();
        IConnection.Setup(x => x.CreateJetStreamContext(It.IsAny<JetStreamOptions>())).Returns(JetStream.Object);

        Provider = new Mock<IServiceProvider>();
        Provider.Setup(x => x.GetService(typeof(IConnection))).Returns(IConnection.Object);
    }

    private List<MessageDataDto> MockMessageDataDtos()
    {
        var msgDataDtos = new List<MessageDataDto>();
        for (int i = 0; i < _payloads.Length; i++)
        {
            msgDataDtos.Add(new MessageDataDto()
            {
                Headers = new List<MessageHeaderDto>()
                {
                    new()
                    {
                        Name = _headers[i][0],
                        Value = _headers[i][1]
                    }
                },
                Payload = new MessagePayloadDto() { Data = _payloads[i] },
                Subject = PrimarySubject
            });
        }

        return msgDataDtos;
    }

    // public class MockMsg : Msg
    // {
    //     //https://github.com/nats-io/nats.net/blob/1da3602602b2e590f22c0be4a83c9b36ce053238/src/NATS.Client/JetStream/JetStreamMsg.cs#L34
    //     public override MetaData MetaData { get; }
    //     
    //     public MockMsg(ulong sequenceNumber)
    //     {
    //         MetaData = new MetaData("a.a.a.1." + sequenceNumber + ".1.1.1");
    //     }
    // }
    
    public virtual List<Msg> MockMessages()
    {
        var messages = new List<Msg>();
        
        for (int i = 0; i < MsgDataDtos?.Count; i++)
        {
            // var attribute = new
            // {
            //     StreamSequence = (ulong) i
            // };

            var header = new MsgHeader { { MsgDataDtos[i].Headers.First().Name, MsgDataDtos[i].Headers.First().Value } };
            var msg = new Mock<Msg>();
            msg.SetupAllProperties();
            msg.Object.Data = Encoding.UTF8.GetBytes(MsgDataDtos[i].Payload.Data!);
            msg.Object.Subject = MsgDataDtos[i].Subject;
            msg.Object.Header = header;
            
            // var msg = new Mock<Msg>
            // {
            //     Object =
            //     {
            //         Subject = MsgDataDtos[i].Subject,
            //         Header = header,
            //         Data = Encoding.UTF8.GetBytes(MsgDataDtos[i].Payload.Data!)
            //     }
            // };
            //var test = new ParameterInfo();
            //TypeDescriptor.AddAttributes(msg.Object.MetaData,  Attribute.GetCustomAttributes());
            //var test = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(MetaData));
            //test.GetType().GetProperty("StreamSequence").SetValue(test, (ulong)i, null);
            //var metaData = MockMetaData((ulong) i);
            //msg.Setup(x => x. StreamSequence).Returns((ulong) i);

            messages.Add(msg.Object);
        }
        
        // MsgDataDtos?.ForEach(x =>
        // {
        //     var header = new MsgHeader { { x.Headers.First().Name, x.Headers.First().Value } };
        //     var msg = new Mock<Msg>();
        //     msg.Setup(x => x).Returns(new Msg(x.Subject, header, Encoding.UTF8.GetBytes(x.Payload.Data!)));
        //     msg.Setup(x => x.MetaData.StreamSequence).Returns();
        //
        //     messages.Add(msg);
        // });

        return messages;
    }

    
    // public interface IMetaData
    // {
    //     string Stream { get; }
    //     string Consumer { get; }
    //     ulong NumDelivered{ get; }
    //     ulong StreamSequence { get; }
    //     ulong ConsumerSequence { get; }
    //     ulong NumPending { get; }
    //     string Domain { get; }
    //     string Prefix { get; }
    //     ulong TimestampNanos { get; }
    //     DateTime Timestamp { get; }
    // }
    //
    // public class MetaDataWrapper : IMetaData
    // {
    //     private readonly MetaData _metaData;
    //
    //     public MetaDataWrapper(MetaData metaData)
    //     {
    //         _metaData = metaData;
    //     }
    //
    //     public string Stream => _metaData.Stream;
    //     public string Consumer => _metaData.Consumer;
    //     public ulong NumDelivered => _metaData.NumDelivered;
    //     public ulong StreamSequence => _metaData.StreamSequence;
    //     public ulong ConsumerSequence => _metaData.ConsumerSequence;
    //     public ulong NumPending => _metaData.NumPending;
    //     public string Domain => _metaData.Domain;
    //     public string Prefix => _metaData.Prefix;
    //     public ulong TimestampNanos => _metaData.TimestampNanos;
    //     public DateTime Timestamp => _metaData.Timestamp;
    // }
    
    
    private Mock<MetaData> MockMetaData(ulong sequenceNumber)
    {
        var metaData = new Mock<MetaData>();
        
        metaData.SetupGet(m => m.Stream).Returns(PrimarySubject);
        metaData.SetupGet(m => m.Consumer).Returns("");
        metaData.SetupGet(m => m.NumDelivered).Returns(10);
        metaData.SetupGet(m => m.StreamSequence).Returns(sequenceNumber);
        metaData.SetupGet(m => m.ConsumerSequence).Returns(3);
        metaData.SetupGet(m => m.NumPending).Returns(5);
        metaData.SetupGet(m => m.Timestamp).Returns(DateTime.Now);
        metaData.SetupGet(m => m.Domain).Returns("");
        metaData.SetupGet(m => m.Prefix).Returns("");
        metaData.SetupGet(m => m.TimestampNanos).Returns(100);

        return metaData;
    }
    
    // private void PublishTestMessages()
    // {
    //     MsgDataDtos?.ForEach(x =>
    //     {
    //         var header = new MsgHeader { { x.Headers.First().Name, x.Headers.First().Value } };
    //         IConnection.Publish(new Msg(x.Subject, header, Encoding.UTF8.GetBytes(x.Payload.Data!)));
    //     });
    // }

    // public List<Msg> GetAllJetStreamMessages(string subject)
    // {
    //     using var connection = Provider.GetRequiredService<IConnection>();
    //     var js = connection.CreateJetStreamContext();
    //     var pullOptions = PullSubscribeOptions.Builder().WithStream(StreamName).Build();
    //     var sub = js.PullSubscribe(subject, pullOptions);
    //
    //     return sub.Fetch(100, 1000).ToList();
    // }
    //
    // public List<Msg> GetAllJetStreamMessages()
    // {
    //     using var connection = Provider.GetRequiredService<IConnection>();
    //
    //     var allMessages = new List<Msg>();
    //     var js = connection.CreateJetStreamContext();
    //     var pullOptions = PullSubscribeOptions.Builder().WithStream(StreamName).Build();
    //     foreach (var subject in ValidSubjects)
    //     {
    //         var sub = js.PullSubscribe(subject, pullOptions);
    //         allMessages.AddRange(sub.Fetch(100, 1000).ToList());
    //     }
    //
    //     return allMessages;
    // }
    //
    //
    // public void Dispose()
    // {
    //     _process.Kill();
    // }
    //
    // static IConnection NatsConnectionFactory(IServiceProvider provider)
    // {
    //     // Arrange
    //     // var mockConnectionFactory = new Mock<IConnectionFactory>();
    //     // var mockConnection = new Mock<IConnection>();
    //     //
    //     // mockConnectionFactory.Setup(factory => factory.CreateConnection()).Returns(mockConnection.Object);
    //     //
    //     // // Act
    //     // var natsClient = new NatsClient(mockConnectionFactory.Object);
    //     // var connection = natsClient.CreateConnection();
    //     //
    //     // // Assert
    //     // Assert.NotNull(connection);
    //     // mockConnectionFactory.Verify(factory => factory.CreateConnection(), Times.Once);
    //     return new ConnectionFactory().CreateConnection("localhost:9000");
    // }
}