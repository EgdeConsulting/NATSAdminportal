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
    public Mock<IJetStreamManagement> JetStreamManagement { get; }
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

        var listStreamInfo = new Mock<IList<StreamInfo>>();
        //listStreamInfo.Setup(x => x.SelectMany<List<StreamInfo>, List<string>>(It.IsAny<IEnumerable<List<StreamInfo>>>(), It.IsAny<Func<List<StreamInfo>)).Returns(ValidSubjects.ToList());

        JetStreamManagement = new Mock<IJetStreamManagement>();
        JetStreamManagement.Setup(x => x.GetStreams()).Returns(listStreamInfo.Object);
        
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

    public virtual List<Msg> MockMessages()
    {
        var messages = new List<Msg>();
        
        for (int i = 0; i < MsgDataDtos?.Count; i++)
        {
            var header = new MsgHeader { { MsgDataDtos[i].Headers.First().Name, MsgDataDtos[i].Headers.First().Value } };
            
            //var msg = new Mock<Msg>();
            // msg.SetupAllProperties();
            // msg.Object.Data = Encoding.UTF8.GetBytes(MsgDataDtos[i].Payload.Data!);
            // msg.Object.Subject = MsgDataDtos[i].Subject;
            // msg.Object.Header = header;
            
            var msg = new Mock<Msg>
            {
                Object =
                {
                    Subject = MsgDataDtos[i].Subject,
                    Header = header,
                    Data = Encoding.UTF8.GetBytes(MsgDataDtos[i].Payload.Data!)
                }
            };

            //msg.SetupGet(x => x.MetaData.StreamSequence).Returns((ulong) i);

            messages.Add(msg.Object);
        }

        return messages;
    }

    // private void PublishTestMessages()
    // {
    //     MsgDataDtos?.ForEach(x =>
    //     {
    //         var header = new MsgHeader { { x.Headers.First().Name, x.Headers.First().Value } };
    //         IConnection.Publish(new Msg(x.Subject, header, Encoding.UTF8.GetBytes(x.Payload.Data!)));
    //     });
    // }

    public List<Msg> GetAllJetStreamMessages(string subject)
    {
        using var connection = Provider.Object.GetRequiredService<IConnection>();
        var js = connection.CreateJetStreamContext();
        var pullOptions = PullSubscribeOptions.Builder().WithStream(StreamName).Build();
        var sub = js.PullSubscribe(subject, pullOptions);
    
        return sub.Fetch(100, 1000).ToList();
    }
    
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