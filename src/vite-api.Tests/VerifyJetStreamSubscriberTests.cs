﻿
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

    private JetStreamSubscriber CreateDefaultJetStreamSubscriber()
    {
        return new JetStreamSubscriber(_fixture.Provider, _fixture.StreamName, new List<string>() { _fixture.Subject });
    }
    
    [Fact]
    public void Get_AllMessages_ReturnsSameCount()
    {
        var subscriber = CreateDefaultJetStreamSubscriber();

        var expectedMessageCount = _fixture.MsgDataDtos.Count;
        var actualMessageCount = subscriber.GetMessages().Count;

        Assert.Equal(expectedMessageCount, actualMessageCount);
    }
    
    [Fact]
    public void Get_SpecificPayload_ReturnsSamePayload()
    {
        const ulong sequenceNumber = 1;
        var subscriber = CreateDefaultJetStreamSubscriber();

        var expectedPayload = _fixture.MsgDataDtos[(int)sequenceNumber - 1].Payload.Data;
        var actualPayload = subscriber.GetPayload(sequenceNumber).Data;
        
        Assert.Equal(expectedPayload, actualPayload);
    }

    [Fact]
    public void Get_SpecificPayload_ThrowsInvalidOperationException()
    {
        const ulong sequenceNumber = 100;
        var subscriber = CreateDefaultJetStreamSubscriber();

        void ActualAction() => subscriber.GetPayload(sequenceNumber);

        Assert.Throws<InvalidOperationException>(ActualAction);
    }
    
    [Fact]
    public void Get_SpecificMessage_ReturnsSameMessage()
    {
        const ulong sequenceNumber = 2;
        var subscriber = CreateDefaultJetStreamSubscriber();

        var expectedData = _fixture.MsgDataDtos[(int)sequenceNumber - 1];
        var actualData = subscriber.GetMessageData(sequenceNumber);
        
        Assert.Equivalent(expectedData, actualData); 
    }
    
    [Fact]
    public void Get_SpecificMessage_ThrowsInvalidOperationException()
    {
        const ulong sequenceNumber = 100;
        var subscriber = CreateDefaultJetStreamSubscriber();

        void ActualAction() => subscriber.GetMessageData(sequenceNumber);
        
        Assert.Throws<InvalidOperationException>(ActualAction);
    }
}
