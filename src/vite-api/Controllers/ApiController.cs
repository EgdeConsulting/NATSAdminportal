using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using NATS.Client;
using vite_api.Classes;
using vite_api.Dto;

namespace vite_api.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase
{
    private readonly StreamManager _streamManager;
    private readonly Publisher _publisher;
    private readonly SubjectManager _subjectManager;
    private readonly SubscriberManager _subscriberManager;

    public ApiController(StreamManager streamManager, Publisher publisher, SubjectManager subjectManager, SubscriberManager subscriberManager)
    {
        _streamManager = streamManager;
        _publisher = publisher;
        _subjectManager = subjectManager;
        _subscriberManager = subscriberManager;
    }

    [HttpGet("allMessages")]
    public IActionResult AllMessages()
    {
        var res = _subscriberManager.GetAllMessages();
        return Ok(res);
    }

    [HttpGet("specificMessage")]
    public IActionResult SpecificMessage([FromQuery] string streamName, [FromQuery] ulong sequenceNumber)
    {
        try
        {
            var res = _subscriberManager.GetSpecificMessage(streamName, sequenceNumber);
            return Ok(res);
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpPost("newMessage")]
    public IActionResult NewMessage([FromBody] MessageDataDto msg)
    {
        try
        {
            _publisher.SendNewMessage(msg);
            return Ok();
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpPost("copyMessage")]
    public IActionResult CopyMessage([FromQuery] string streamName, [FromQuery] ulong sequenceNumber, [FromQuery] string newSubject)
    {
        try
        {
            var msg = _subscriberManager.GetSpecificMessage(streamName, sequenceNumber);
            _publisher.CopyMessage(msg!, newSubject);
            return Ok();
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpDelete("deleteMessage")]
    public IActionResult DeleteMessage([FromQuery] string streamName, [FromQuery] ulong sequenceNumber, [FromQuery] bool erase)
    {
        try
        {
            var res = _streamManager.DeleteMessage(streamName, sequenceNumber, erase);
            return Ok(res);
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpGet("allStreams")]
    public IActionResult AllStreams()
    {
        try
        {
            var res = _streamManager.GetAllStreams();
            return Ok(res);
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpGet("specificStream")]
    public IActionResult SpecificStream([FromQuery] string streamName)
    {
        try
        {
            var res = _streamManager.GetSpecificStream(streamName);
            return Ok(res);
        }
        catch
        {
            return BadRequest();
        }
    }
    
    [HttpGet("subjectHierarchy")]
    public IActionResult GetSubjectHierarchy()
    {
        try
        {
            var res = _subjectManager.GetSubjectHierarchy();
            return Ok(res);
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpGet("allSubjects")]
    public IActionResult AllSubjects()
    {
        try
        {
            var res = _subjectManager.GetAllSubjects();
            return Ok(res);
        }
        catch
        {
            return BadRequest();
        }
    }

#warning This endpoint exists solely to allow for swapping between change dummy user accounts
    [HttpPost("updateUserAccount")]
    public IActionResult UpdateUserAccount([FromQuery] string username)
    {
        try
        {
            UserAccount.Name = username;
            return Ok();
        }
        catch
        {
            return BadRequest();
        }
    }
}
