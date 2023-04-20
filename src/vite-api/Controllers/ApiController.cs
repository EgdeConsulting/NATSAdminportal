using Microsoft.AspNetCore.Mvc;
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
        try
        {
            var res = _subscriberManager.GetAllMessages();
            return Ok(res);
        }
        catch
        {
            return StatusCode(429);
        }
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

    [HttpGet("specificPayload")]
    public IActionResult SpecificPayload([FromQuery] string streamName, [FromQuery] ulong sequenceNumber)
    {
        try
        {
            var res = _subscriberManager.GetSpecificPayload(streamName, sequenceNumber);
            return Ok(res);
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpPost("newMessage")]
    public IActionResult NewMessage([FromBody] MessageDataDto msgDto)
    {
        try
        {
            _publisher.SendMessage(msgDto);
            return Ok();
        }
        catch (ArgumentException e)
        {
            var response = new { error = e.Message };
            return StatusCode(406, response);
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpPost("copyMessage")]
    public IActionResult CopyMessage([FromBody] MessageDto msgDto)
    {
        try
        {
            if (msgDto.Stream == null || msgDto.Subject == null) return BadRequest();
            var msg = _subscriberManager.GetSpecificMessage(msgDto.Stream, msgDto.SequenceNumber, false);
            var payload = _subscriberManager.GetSpecificPayload(msgDto.Stream, msgDto.SequenceNumber, false);
            msg!.Payload = payload!;
            _publisher.CopyMessage(msg, msgDto.SequenceNumber, msgDto.Subject);
            return Ok();
        }
        catch (ArgumentException e)
        {
            var response = new { error = e.Message };
            return StatusCode(406, response);
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpDelete("deleteMessage")]
    public IActionResult DeleteMessage([FromBody] MessageDto msgDto)
    {
        try
        {
            var res = msgDto.Stream != null &&
                      _streamManager.DeleteMessage(msgDto.Stream, msgDto.SequenceNumber, msgDto.Erase);
            return Ok(res);
        }
  
        catch (ArgumentException e)
        {
            var response = new { error = e.Message };
            return StatusCode(406, response);
        }
        catch (Exception)
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
