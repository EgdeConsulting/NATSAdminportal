using System.Diagnostics;
using System.Collections.Concurrent;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;
using NATS.Client.JetStream;
using vite_api.Dto;

namespace vite_api.Tests;

[CollectionDefinition("MockServer collection")]
public class MockServerFixture : IDisposable, ICollectionFixture<MockServerFixture>
{
    private readonly ServiceCollection _services = new();
    private readonly Process _process = new();

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
    public string PrimarySubject => "PrimarySubject";
    public string SecondarySubject => "SecondarySubject";
    public string InvalidSubject => "InvalidSubject";
    public string[] ValidSubjects { get; }
    public List<MessageDataDto> MsgDataDtos { get; }
    public ServiceProvider Provider { get; }

    public MockServerFixture()
    {
        // https://github.com/nats-io/nats.net/issues/426
        // Since on official test framework for NATS exists, a rudimentary mock-server
        // is being setup and started. The NATS-server runs only during testing in a
        // local process. 
        _process.StartInfo.FileName = @"config/nats-server.exe";
        _process.StartInfo.Arguments = @"-c config/server.conf";
        _process.Start();
        // Give the server enough time to start. 
        Thread.Sleep(3000);

        _services.AddTransient(NatsConnectionFactory);
        Provider = _services.BuildServiceProvider();

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

        MsgDataDtos = InitializeTestMessageDataDtos();

        using var connection = Provider.GetRequiredService<IConnection>();
        SetUpTestStream(connection);
        PublishTestMessages(connection);
    }

    private List<MessageDataDto> InitializeTestMessageDataDtos()
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

    private void SetUpTestStream(IConnection connection)
    {
        var jsm = connection.CreateJetStreamManagementContext();

        StreamConfiguration streamConfig = StreamConfiguration.Builder()
            .WithName(StreamName)
            .WithSubjects(ValidSubjects)
            .WithStorageType(StorageType.Memory)
            .Build();
        jsm.AddStream(streamConfig);
    }

    private void PublishTestMessages(IConnection connection)
    {
        MsgDataDtos?.ForEach(x =>
        {
            var header = new MsgHeader { { x.Headers.First().Name, x.Headers.First().Value } };
            connection.Publish(new Msg(x.Subject, header, Encoding.UTF8.GetBytes(x.Payload.Data!)));
        });
    }

    public List<Msg> GetAllJetStreamMessages(string subject)
    {
        using var connection = Provider.GetRequiredService<IConnection>();
        var js = connection.CreateJetStreamContext();
        var pullOptions = PullSubscribeOptions.Builder().WithStream(StreamName).Build();
        var sub = js.PullSubscribe(subject, pullOptions);

        return sub.Fetch(100, 1000).ToList();
    }

    public List<Msg> GetAllJetStreamMessages()
    {
        using var connection = Provider.GetRequiredService<IConnection>();

        var allMessages = new List<Msg>();
        var js = connection.CreateJetStreamContext();
        var pullOptions = PullSubscribeOptions.Builder().WithStream(StreamName).Build();
        foreach (var subject in ValidSubjects)
        {
            var sub = js.PullSubscribe(subject, pullOptions);
            allMessages.AddRange(sub.Fetch(100, 1000).ToList());
        }

        return allMessages;
    }


    public void Dispose()
    {
        _process.Kill();
    }

    static IConnection NatsConnectionFactory(IServiceProvider provider)
    {
        return new ConnectionFactory().CreateConnection("localhost:9000");
    }
}