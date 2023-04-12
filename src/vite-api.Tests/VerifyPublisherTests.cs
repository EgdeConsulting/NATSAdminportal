using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using vite_api.Classes;

namespace vite_api.Tests;

[UsesVerify, Collection("JetStream collection")]
public class VerifyPublisherTests
{
    private readonly JetStreamFixture _fixture;

    public VerifyPublisherTests(JetStreamFixture fixture)
    {
        _fixture = fixture;
    }

    private Publisher CreateDefaultPublisher()
    {
        var loggerMock = new Mock<ILogger<Publisher>>();
        return new Publisher(loggerMock.Object, _fixture.Provider);
    }
    
    [Fact]
    public void Send_Message_ReturnsSameMessage()
    {
        var index = _fixture.MsgDataDtos.Count - 1;
        var publisher = CreateDefaultPublisher();
        
        var expectedMessage = _fixture.MsgDataDtos[index]; 
        publisher.SendNewMessage(expectedMessage);
        var actualMessage = _fixture.GetAllJetStreamMessages(_fixture.Subject)[index + 1];

        Assert.Equal(expectedMessage.Payload.Data, Encoding.UTF8.GetString(actualMessage.Data));
        Assert.Equal(expectedMessage.Subject, actualMessage.Subject);
    }
    
    [Fact]
    public void Send_Message_ThrowsArgumentException()
    {
        var index = _fixture.MsgDataDtos.Count - 1;
        var publisher = CreateDefaultPublisher();
        
        var expectedMessage = _fixture.MsgDataDtos[index];
        expectedMessage.Subject = _fixture.FaultySubject;
        void ActualAction() => publisher.SendNewMessage(expectedMessage);

        Assert.Throws<ArgumentException>(ActualAction);
    }
    
    [Fact]
    public void Copy_Message_ReturnsSameMessageOnNewSubject()
    {
        var index = 0;
        var publisher = CreateDefaultPublisher();
        
        var expectedMessage = _fixture.MsgDataDtos[index]; 
        publisher.CopyMessage(expectedMessage, _fixture.CopySubject);
        var actualMessage = _fixture.GetAllJetStreamMessages(_fixture.CopySubject)[index];

        Assert.Equal(expectedMessage.Payload.Data, Encoding.UTF8.GetString(actualMessage.Data));
        Assert.Equal(_fixture.CopySubject, actualMessage.Subject);
    }
    
    [Fact]
    public void Copy_Message_ThrowsArgumentException()
    {
        var index = 0;
        var publisher = CreateDefaultPublisher();
        
        var expectedMessage = _fixture.MsgDataDtos[index];
        void ActualAction() => publisher.CopyMessage(expectedMessage, _fixture.FaultySubject);

        Assert.Throws<ArgumentException>(ActualAction);
    }
}