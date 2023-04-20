using System.Text;
using Microsoft.Extensions.Logging;
using Moq;
using vite_api.Classes;

namespace vite_api.Tests;

[UsesVerify, Collection("MockServer collection")]

public class VerifyStreamManagerTests
{
    private readonly MockServerFixture _fixture;

    public VerifyStreamManagerTests(MockServerFixture fixture)
    {
        _fixture = fixture;
    }

    private StreamManager CreateDefaultStreamManager()
    {
        var loggerMock = new Mock<ILogger<StreamManager>>();
        return new StreamManager(loggerMock.Object, _fixture.Provider);
    }

    [Fact]
    public void DeleteMessage_ReturnsSameCount()
    {
        var streamName = _fixture.StreamName;
        var sequenceNumber = _fixture.GetAllJetStreamMessages().Count;
        var streamManager = CreateDefaultStreamManager();

        var expectedCount = sequenceNumber - 1;
        streamManager.DeleteMessage(streamName, (ulong)sequenceNumber, true);
        var actualCount = _fixture.GetAllJetStreamMessages().Count;

        Assert.Equal(expectedCount, actualCount);
    }

    [Fact]
    public void DeleteMessage_ThrowsArgumentException()
    {
        var streamName = _fixture.StreamName;
        var sequenceNumber = 69;
        var streamManager = CreateDefaultStreamManager();

        void ActualAction() => streamManager.DeleteMessage(streamName, (ulong)sequenceNumber, true);

        Assert.Throws<ArgumentException>(ActualAction);
    }

    [Fact]
    public void GetAllStreams_ReturnsSameName()
    {
        var streamManager = CreateDefaultStreamManager();

        var expectedValue = _fixture.StreamName;
        var actualValue = streamManager.GetAllStreams()[0].Name;

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GetAllStreams_ReturnsSameCount()
    {
        var streamManager = CreateDefaultStreamManager();

        var expectedValue = 1;
        var actualValue = streamManager.GetAllStreams().Count;

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GetSpecificStream_ReturnsSameObject()
    {
        var streamName = _fixture.StreamName;
        var streamManager = CreateDefaultStreamManager();

        var expectedValue = _fixture.ValidSubjects;
        var actualValue = streamManager.GetSpecificStream(streamName).Subjects;

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GetSpecificStream_ThrowsArgumentException()
    {
        var streamName = _fixture.InvalidStreamName;
        var streamManager = CreateDefaultStreamManager();

        void ActualAction() => streamManager.GetSpecificStream(streamName);

        Assert.Throws<ArgumentException>(ActualAction);
    }
}