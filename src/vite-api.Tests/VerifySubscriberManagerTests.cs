using Microsoft.Extensions.Logging;
using Moq;
using vite_api.Classes;

namespace vite_api.Tests;

[UsesVerify, Collection("MockServer collection")]
public class VerifySubscriberManagerTests
{
    private readonly MockServerFixture _fixture;

    public VerifySubscriberManagerTests(MockServerFixture fixture)
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
        var index = 0;
        var manager = CreateDefaultSubscriberManager();
        
        var expectedPayload = _fixture.MsgDataDtos[index].Payload;
        var actualPayload = manager.GetSpecificPayload(_fixture.StreamName, (ulong) index + 1);
        
        Assert.Equivalent(expectedPayload, actualPayload);
    }
    
    [Fact]
    public void GetSpecificMessage_ShortPayload_ReturnsSameObject()
    {
        var index = 1;
        var manager = CreateDefaultSubscriberManager();
        
        var expectedMessage = _fixture.MsgDataDtos[index];
        var actualMessage = manager.GetSpecificMessage(_fixture.StreamName, (ulong) index + 1);
        
        Assert.Equivalent(expectedMessage, actualMessage);
    }

    [Fact]
    public void GetSpecificMessage_LongPayload_ReturnsSameObject()
    {
        var index = 0;
        var manager = CreateDefaultSubscriberManager();
        
        var expectedMessage = _fixture.MsgDataDtos[index];
        var actualMessage = manager.GetSpecificMessage(_fixture.StreamName, (ulong) index + 1);
        actualMessage!.Payload.Data = manager.GetSpecificPayload(_fixture.StreamName, (ulong) index + 1)!.Data;
        
        Assert.Equivalent(expectedMessage, actualMessage);
    }

    [Fact]
    public void GetSpecificMessage_ThrowsNullReferenceException()
    {
        var index = _fixture.MsgDataDtos.Count;
        var manager = CreateDefaultSubscriberManager();

        void ActualAction() => manager.GetSpecificMessage("invalidStream", 100);

        Assert.Throws<NullReferenceException>(ActualAction);
    }

    [Fact]
    public void GetSpecificMessage_ThrowsInvalidOperationException()
    {
        var index = _fixture.MsgDataDtos.Count;
        var manager = CreateDefaultSubscriberManager();

        void ActualAction() => manager.GetSpecificMessage(_fixture.StreamName, 100);

        Assert.Throws<InvalidOperationException>(ActualAction);
    }
}