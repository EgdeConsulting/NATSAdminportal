using System.Text;
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
        publisher.SendMessage(expectedMessage);
        var actualMessage = _fixture.GetAllJetStreamMessages(_fixture.PrimarySubject)[index + 1];

        Assert.Equal(expectedMessage.Payload.Data, Encoding.UTF8.GetString(actualMessage.Data));
        Assert.Equal(expectedMessage.Subject, actualMessage.Subject);
    }
    
    [Fact]
    public void Send_Message_ThrowsArgumentException()
    {
        var index = _fixture.MsgDataDtos.Count - 1;
        var publisher = CreateDefaultPublisher();
        
        var expectedMessage = _fixture.MsgDataDtos[index];
        expectedMessage.Subject = _fixture.InvalidSubject;
        void ActualAction() => publisher.SendMessage(expectedMessage);

        Assert.Throws<ArgumentException>(ActualAction);
    }
    
    [Fact]
    public void Copy_Message_ReturnsSameMessageOnNewSubject()
    {
        var index = 0;
        var sequenceNumber = (ulong) index + 1;
        var publisher = CreateDefaultPublisher();
        
        var expectedMessage = _fixture.MsgDataDtos[index]; 
        publisher.CopyMessage(expectedMessage, sequenceNumber, _fixture.SecondarySubject);
        var actualMessage = _fixture.GetAllJetStreamMessages(_fixture.SecondarySubject)[index];


        Assert.Equal(expectedMessage.Payload.Data, Encoding.UTF8.GetString(actualMessage.Data));
        Assert.Equal(_fixture.SecondarySubject, actualMessage.Subject);
    }
    
    [Fact]
    public void Copy_Message_ThrowsArgumentException()
    {
        var index = 1;
        var sequenceNumber = (ulong) index + 1;
        var publisher = CreateDefaultPublisher();
        
        var expectedMessage = _fixture.MsgDataDtos[index];
        void ActualAction() => publisher.CopyMessage(expectedMessage, sequenceNumber, _fixture.InvalidSubject);

        Assert.Throws<ArgumentException>(ActualAction);
    }
}