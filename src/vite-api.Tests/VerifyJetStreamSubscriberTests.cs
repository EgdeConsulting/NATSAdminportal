using Moq;
using NATS.Client;
using NATS.Client.JetStream;
using vite_api.Classes;

namespace vite_api.Tests;

// [UsesVerify, Collection("MockServer collection")]
public class VerifyJetStreamSubscriberTests
{
    // private readonly MockServerFixture _fixture;
    //
    // public VerifyJetStreamSubscriberTests(MockServerFixture fixture)
    // {
    //     _fixture = fixture;
    // }

    private JetStreamSubscriber CreateDefaultJetStreamSubscriber()
    {
        // var mockProvider = new Mock<IServiceProvider>();
        var mockConnection = new Mock<IConnection>();
        var mockJetstream = new Mock<IJetStream>();
        var mockPullSubscribe = new Mock<IJetStreamPullSubscription>();
        // var mockMessages = new Mock<IList<Msg>>();

        // mockMessages.Setup(x=>x.)
        // .Object;
        var mockMessages = new List<Msg> {new("Hei")};
        // SequenceNumber = x.MetaData.StreamSequence,
        // Timestamp = x.MetaData.Timestamp,
        // Stream = StreamName,
        // Subject = x.Subject,
        mockPullSubscribe.Setup(x => x.Fetch(It.IsAny<int>(), It.IsAny<int>()))
            .Returns<int, int>((batchSize, maxWaitMillis) => mockMessages);

        var primarySubject = "Primary";
        mockJetstream.Setup(x => x.PullSubscribe(It.IsAny<string>(), It.IsAny<PullSubscribeOptions>())).Returns(mockPullSubscribe.Object);
        mockConnection.Setup(x => x.CreateJetStreamContext(It.IsAny<JetStreamOptions>())).Returns(mockJetstream.Object);
        var streamName = "Name";

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider
            .Setup(x => x.GetService(typeof(IConnection)))
            .Returns(mockConnection.Object);

        return new JetStreamSubscriber(serviceProvider.Object, streamName, new List<string> {primarySubject});
    }

    [Fact]
    public void GetMessages_ReturnsSameCount()
    {
        var subscriber = CreateDefaultJetStreamSubscriber();

        var expectedMessageCount = 1;
        var actualMessageCount = subscriber.GetMessages().Count;

        Assert.Equal(expectedMessageCount, actualMessageCount);
    }

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
    //
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
    //
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