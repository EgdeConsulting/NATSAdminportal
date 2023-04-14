using Microsoft.Extensions.Logging;
using Moq;
using vite_api.Classes;

namespace vite_api.Tests;

[UsesVerify, Collection("JetStream collection")]
public class VerifySubscriberManagerTests
{
    private readonly JetStreamFixture _fixture;

    public VerifySubscriberManagerTests(JetStreamFixture fixture)
    {
        _fixture = fixture;
    }

    private SubscriberManager CreateDefaultSubscriberManager()
    {
        var loggerMock = new Mock<ILogger<SubscriberManager>>();
        return new SubscriberManager(loggerMock.Object, _fixture.Provider);
    }
    [Fact]
    public void GetSpecificPayload_ReturnsSameObject()
    {
        var index = _fixture.MsgDataDtos.Count;
        var manager = CreateDefaultSubscriberManager();
        
        var expectedPayload = _fixture.MsgDataDtos[index - 1].Payload;
        var actualPayload = manager.GetSpecificPayload(_fixture.StreamName, (ulong) index);
        
        Assert.Equivalent(expectedPayload, actualPayload);
    }
    
    [Fact]
    public void GetSpecificMessage_ReturnsSameObject()
    {
        var index = _fixture.MsgDataDtos.Count;
        var manager = CreateDefaultSubscriberManager();
        
        var expectedMessage = _fixture.MsgDataDtos[index - 1];
        var actualMessage = manager.GetSpecificMessage(_fixture.StreamName, (ulong) index);
        
        Assert.Equivalent(expectedMessage, actualMessage);
    }

    [Fact]
    public void GetSpecificMessage_ThrowsNullReferenceException()
    {
        var index = _fixture.MsgDataDtos.Count;
        var manager = CreateDefaultSubscriberManager();

        const string streamName = "faultyStream";
        const ulong sequenceNum = 112;
        //Tests faulty stream name
        void ActualAction() => manager.GetSpecificMessage(streamName, sequenceNum);
        Assert.Throws<NullReferenceException>(ActualAction);
    }
    [Fact]
    public void GetSpecificMessage_ThrowsInvalidOperationException()
    {
        var index = _fixture.MsgDataDtos.Count;
        var manager = CreateDefaultSubscriberManager();
        
        const ulong sequenceNum = 112;
        //Tests faulty sequence number
        void ActualAction() => manager.GetSpecificMessage(_fixture.StreamName, sequenceNum);
        Assert.Throws<InvalidOperationException>(ActualAction);
    }
}