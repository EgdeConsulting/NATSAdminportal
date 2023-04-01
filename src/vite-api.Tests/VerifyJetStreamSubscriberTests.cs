
using vite_api.Classes;

namespace vite_api.Tests;

[UsesVerify, Collection("JetStream collection")]
public class VerifyJetStreamSubscriberTests
{
    private readonly JetStreamFixture _fixture;

    public VerifyJetStreamSubscriberTests(JetStreamFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public void AllMessagesReturned()
    {
        var subscriber = new JetStreamSubscriber(_fixture.Provider, _fixture.StreamName, new List<string>() { _fixture.Subject });
        Assert.Equal(_fixture.MsgDataDtos.Count, subscriber.GetMessages().Count);
    }
    
    [Fact]
    public void ExpectedPayload()
    {
        ulong sequenceNumber = 1;
        var subscriber = new JetStreamSubscriber(_fixture.Provider, _fixture.StreamName, new List<string>() { _fixture.Subject });
        Assert.Equal(_fixture.MsgDataDtos[(int) sequenceNumber - 1].Payload.Data, subscriber.GetPayload(sequenceNumber).Data);
    }

    [Fact]
    public void PayloadDoesNotExist()
    {
        ulong sequenceNumber = 100;
        var subscriber = new JetStreamSubscriber(_fixture.Provider, _fixture.StreamName, new List<string>() { _fixture.Subject });
        Assert.Throws<InvalidOperationException>(() => subscriber.GetPayload(sequenceNumber));
    }
    
    [Fact]
    public void ExpectedMessageData()
    {
        ulong sequenceNumber = 2;
        var subscriber = new JetStreamSubscriber(_fixture.Provider, _fixture.StreamName, new List<string>() { _fixture.Subject });
        Assert.Equivalent(_fixture.MsgDataDtos[(int) sequenceNumber - 1], subscriber.GetMessageData(sequenceNumber)); 
    }
    
    [Fact]
    public void MessageDataDoesNotExist()
    {
        ulong sequenceNumber = 100;
        var subscriber = new JetStreamSubscriber(_fixture.Provider, _fixture.StreamName, new List<string>() { _fixture.Subject });
        Assert.Throws<InvalidOperationException>(() => subscriber.GetMessageData(sequenceNumber));
    }
}
