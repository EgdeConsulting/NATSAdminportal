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

    [HttpGet("streamName")]
    public IActionResult GetStreamName([FromQuery] string streamName)
    {
        var res = _streamManager.GetExtendedStreamInfo(streamName);
        return Ok(res);
    }

    [HttpGet("messageData")]
    public IActionResult GetMessageData([FromQuery] string streamName, [FromQuery] ulong sequenceNumber)
    {
        var res = _subscriberManager.GetSpecificMessage(streamName, sequenceNumber);
        return Ok(res);
    }

    [HttpPost("publishFullMessage")]
    public IActionResult PublishFullMessage([FromBody] PublishMessageDto msg)
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
    [HttpDelete("deleteMessage")]
    public async Task<IActionResult> DeleteMessage([FromQuery] string streamName, [FromQuery] ulong sequenceNumber, [FromQuery] bool erase)
    {
        var res = _streamManager.DeleteMessage(streamName, sequenceNumber, erase);
        return Ok(res);
    }

    // #warning Why post and not delete?
    // [HttpPost("deleteMessage")]
    // public async Task<IActionResult> DeleteMessage()
    // {
    //     string content = "";
    //     using (StreamReader stream = new StreamReader(Request.Body))
    //     {
    //         content = await stream.ReadToEndAsync();
    //     }
    //
    //     var jsonObject = JsonNode.Parse(content);
    //
    //     if (jsonObject != null && jsonObject["name"] != null && jsonObject["sequenceNumber"] != null)
    //     {
    //         string streamName = jsonObject["name"]!.ToString();
    //         ulong sequenceNumber = ulong.Parse(jsonObject["number"]!.ToString());
    //         bool erase = jsonObject["erase"]!.ToString() == "true";
    //
    //         _streamManager.DeleteMessage(streamName, sequenceNumber, erase);
    //     }
    //
    //     return Ok();
    // }

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

#warning Which stream do we get it for? There's absolutely no parameters here?
    [HttpGet("streamBasicInfo")]
    public IActionResult GetBasicStreamInfo()
    {
        var res = _streamManager.GetBasicStreamInfo();
        return Ok(res);
    }

#warning Get which subject hierarchy? On what stream? No parameters.
    [HttpGet("subjectHierarchy")]
    public IActionResult GetSubjectHierarch()
    {
        var res = _subjectManager.GetSubjectHierarchy();
        return Ok(res);
    }

    [HttpGet("subjectHierarchy2")]
    public IActionResult GetSubjectHierarch2()
    {
        var res = _subjectManager.GetSubjectHierarch2();
        return Ok(res);
    }


    [HttpGet("subjectNames")]
    public IActionResult GetSubjectNames()
    {
#warning Get subject names for what stream?
        var res = _subjectManager.GetSubjectNames();
        return Ok(res);
    }

    [HttpGet("messages")]
    public IActionResult GetMessages()
    {
#warning Get what messages? No parameters so no stream defined
        var res = _subscriberManager.GetAllMessages();
        return Ok(res);
    }
}
