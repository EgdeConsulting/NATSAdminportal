using Moq;
using NATS.Client;
using NATS.Client.JetStream;
using vite_api.Classes;

namespace vite_api.Tests;

[UsesVerify, Collection("MockServer collection")]
public class VerifyJetStreamSubscriberTests
{
    private readonly MockServerFixture _fixture;
    
    public VerifyJetStreamSubscriberTests(MockServerFixture fixture)
    {
        _fixture = fixture;
    }

    private JetStreamSubscriber CreateDefaultJetStreamSubscriber()
    {
        return new JetStreamSubscriber(_fixture.Provider.Object, _fixture.StreamName, new List<string>() { _fixture.PrimarySubject });
    }

    [Fact]
    public void GetMessages_ReturnsSameCount()
    {
        var subscriber = CreateDefaultJetStreamSubscriber();

        var expectedMessageCount = 3;
        var actualMessageCount = subscriber.GetMessages().Count;

        Assert.Equal(expectedMessageCount, actualMessageCount);
    }

    // Both MetaData and JetStreamMsg are sealed classes which can't be mocked.
    // This in return makes it difficult to get the following unit tests to work.
    // https://github.com/nats-io/nats.net/blob/1da3602602b2e590f22c0be4a83c9b36ce053238/src/NATS.Client/JetStream/JetStreamMsg.cs#L34

    // [Fact]
    // public void GetPayload_ReturnsSamePayload()
    // {
    //     const ulong sequenceNumber = 1;
    //     var subscriber = CreateDefaultJetStreamSubscriber();
    //
    //     var expectedPayload = _fixture.MsgDataDtos[(int)sequenceNumber - 1].Payload.Data;
    //     var actualPayload = subscriber.GetPayload(sequenceNumber).Data;
    //     
    //     Assert.Equal(expectedPayload, actualPayload);
    // }
    
    // [Fact]
    // public void GetPayload_ThrowsInvalidOperationException()
    // {
    //     const ulong sequenceNumber = 100;
    //     var subscriber = CreateDefaultJetStreamSubscriber();
    //
    //     void ActualAction() => subscriber.GetPayload(sequenceNumber);
    //
    //     Assert.Throws<InvalidOperationException>(ActualAction);
    // }
    
    // [Fact]
    // public void GetMessageData_ReturnsSameData()
    // {
    //     const ulong sequenceNumber = 2;
    //     var subscriber = CreateDefaultJetStreamSubscriber();
    //
    //     var expectedData = _fixture.MsgDataDtos[(int)sequenceNumber - 1];
    //     var actualData = subscriber.GetMessageData(sequenceNumber);
    //     
    //     Assert.Equivalent(expectedData, actualData); 
    // }

    // [Fact]
    // public void GetMessageData_ThrowsInvalidOperationException()
    // {
    //     const ulong sequenceNumber = 100;
    //     var subscriber = CreateDefaultJetStreamSubscriber();
    //
    //     void ActualAction() => subscriber.GetMessageData(sequenceNumber);
    //     
    //     Assert.Throws<InvalidOperationException>(ActualAction);
    // }
}