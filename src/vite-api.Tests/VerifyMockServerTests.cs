using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;
using Moq;
using vite_api.Classes;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;
using NATS.Client.JetStream;
using vite_api.Dto;

namespace vite_api.Tests;

[UsesVerify]
public class VerifyMockServerTests
{
    private readonly ServiceProvider _provider;
    private readonly Process _process;
    
    public VerifyMockServerTests()
    {
        // ProcessStartInfo startInfo = new ProcessStartInfo();
        // startInfo.CreateNoWindow = false;
        // startInfo.UseShellExecute = false;
        // startInfo.FileName = @".\bin\Debug\net7.0\config\nats-server.exe";
        // startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        _process = new Process();
        _process.StartInfo.FileName = @".\config\nats-server.exe";
        _process.StartInfo.Arguments = @"-c .\config\server.conf";
        _process.Start();
        
        Thread.Sleep(10000);
        
        ServiceCollection services = new();
        services.AddTransient(NatsConnectionFactory);
        _provider = services.BuildServiceProvider();
    }
    
    static IConnection NatsConnectionFactory(IServiceProvider provider)
    {
        return new ConnectionFactory().CreateConnection("127.0.0.1:9000");
    }
    
    private Publisher CreateDefaultPublisher()
    {
        var loggerMock = new Mock<ILogger<Publisher>>();
        return new Publisher(loggerMock.Object, _provider);
    }
    
    public List<Msg> GetAllJetStreamMessages(string subject)
    {
        using var connection = _provider.GetRequiredService<IConnection>();
        var js = connection.CreateJetStreamContext();
        var pullOptions = PullSubscribeOptions.Builder().Build();
        var sub = js.PullSubscribe(subject, pullOptions);
        
        return sub.Fetch(100, 1000).ToList();
    }
    
    //[Fact]
    // public void Send_Message_ReturnsSameMessage()
    // {
    //     var publisher = CreateDefaultPublisher();
    //
    //     var expectedMessage = new MessageDataDto()
    //     {
    //         Headers = new List<MessageHeaderDto>()
    //         {
    //             new()
    //             {
    //                 Name = "header1",
    //                 Value = "value1"
    //             }
    //         },
    //         Payload = new MessagePayloadDto() { Data = "Payload" },
    //         Subject = "TestSubject"
    //     }; 
    //     publisher.SendNewMessage(expectedMessage);
    //     var actualMessage = GetAllJetStreamMessages("TestSubject")[0];
    //
    //     Assert.Equal(expectedMessage.Payload.Data, Encoding.UTF8.GetString(actualMessage.Data));
    //     Assert.Equal(expectedMessage.Subject, actualMessage.Subject);
    // }
    
}