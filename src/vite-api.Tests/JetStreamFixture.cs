using System.Text;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;
using NATS.Client.JetStream;
using vite_api.Dto;

namespace vite_api.Tests;

[CollectionDefinition("JetStream collection")]
public class JetStreamFixture : IDisposable, ICollectionFixture<JetStreamFixture>
{
    private readonly ServiceCollection _services = new();
    private const string Url = "nats://demo.nats.io";
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
    public string StreamName => "xUnitStream";
    public string Subject => "xUnitSubject";
    public List<MessageDataDto> MsgDataDtos { get; }
    public ServiceProvider Provider { get; }

    public JetStreamFixture()
    {
        _services.AddTransient(NatsConnectionFactory);
        MsgDataDtos = new List<MessageDataDto>();
        Provider = _services.BuildServiceProvider();
        
        InitializeTestMessageDataDtos();
        
        using var connection = Provider.GetRequiredService<IConnection>();
        SetUpTestStream(connection);
        PublishTestMessages(connection);
    }

    private void InitializeTestMessageDataDtos()
    {
        for (int i = 0; i < _payloads.Length; i++)
        {
            MsgDataDtos.Add(new MessageDataDto()
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
                Subject = Subject
            });
        }
    }

    private void SetUpTestStream(IConnection connection)
    {
        var jsm = connection.CreateJetStreamManagementContext();
        
        StreamConfiguration streamConfig = StreamConfiguration.Builder()
            .WithName(StreamName)
            .WithSubjects(Subject)
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
    
    public void Dispose()
    {
        using var connection = Provider.GetRequiredService<IConnection>();
        var jsm = connection.CreateJetStreamManagementContext();
        jsm.DeleteStream(StreamName);
    }
    
    static IConnection NatsConnectionFactory(IServiceProvider provider)
    {
        return new ConnectionFactory().CreateConnection(Url);
    }
}