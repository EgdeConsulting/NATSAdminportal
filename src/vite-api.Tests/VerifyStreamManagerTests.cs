using System.Text;
using Microsoft.Extensions.Logging;
using Moq;
using vite_api.Classes;

namespace vite_api.Tests;

[UsesVerify, Collection("JetStream collection")]

public class VerifyStreamManagerTests
{
    private readonly JetStreamFixture _fixture;

    public VerifyStreamManagerTests(JetStreamFixture fixture)
    {
        _fixture = fixture;
    }

    private StreamManager CreateDefaultStreamManager()
    {
        var loggerMock = new Mock<ILogger<StreamManager>>();
        return new StreamManager(loggerMock.Object, _fixture.Provider);
    }

    public void Delete_Message_ReturnsSameCount()
    {
        var streamName = _fixture.StreamName;
        var sequenceNumber = _fixture.MsgDataDtos.Count;
        var streamManager = CreateDefaultStreamManager();

        var expectedCount = sequenceNumber - 1;
        streamManager.DeleteMessage(streamName, (ulong)sequenceNumber, true);
    }

    public void Deleted_Message_Cannot_Be_Found()
    {
        //Check that the deleted message cannot be found(by checking for seq.num)
    }
}