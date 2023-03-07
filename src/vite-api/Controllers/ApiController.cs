using System.Text.Json.Nodes;
using Backend.Logic;
using Microsoft.AspNetCore.Mvc;

namespace vite_api.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase
{
    private readonly StreamManager _streamManager;
    private readonly Subscriber _sub;
    private readonly Publisher _pub;
    private readonly SubjectManager _subjectManager;

    public ApiController(StreamManager streamManager, Subscriber sub, Publisher pub, SubjectManager subjectManager)
    {
        _streamManager = streamManager;
        _sub = sub;
        _pub = pub;
        _subjectManager = subjectManager;
    }

    #warning Why post and not get?
    [HttpPost("streamName")]
    public async Task<IActionResult> GetExtendedStreamInfo()
    {

        string streamName = "";

        using (StreamReader stream = new StreamReader(Request.Body))
        {
            streamName = await stream.ReadToEndAsync();
        }
        var result = Results.Json(_streamManager.GetExtendedStreamInfo(streamName));
        return Ok(result);
    }

    [HttpGet("streamName/{streamName}")]
    public IActionResult GetExtendedStreamInfo2([FromRoute] string streamName)
    {
        var result = _streamManager.GetExtendedStreamInfo2(streamName);
        return Ok(result);
    }

    [HttpPut("subject")]
    public async Task<IActionResult> CreateSubject()
    {
        string content = "";
        using (StreamReader stream = new StreamReader(Request.Body))
        {
            content = await stream.ReadToEndAsync();
        }

        var jsonObject = JsonNode.Parse(content);

        if (jsonObject != null && jsonObject["subject"] != null)
        {
            var subject = jsonObject["subject"];

            if (subject != null && !string.IsNullOrWhiteSpace(subject.ToString()))
                _sub.MessageSubject = subject.ToString();
        }

        #warning Post but no return info about created resource?
        return Ok();
    }


    [HttpPost("publishFullMessage")]
    public async Task PublishFullMessage()
    {
        string content = "";
        using (StreamReader stream = new StreamReader(Request.Body))
        {
            content = await stream.ReadToEndAsync();
        }

        var jsonObject = JsonNode.Parse(content);

        if (jsonObject != null && jsonObject["payload"] != null)
        {
            var payload = jsonObject["payload"];
            var subject = jsonObject["subject"];
            var headers = jsonObject["headers"];

            if (payload != null && !string.IsNullOrWhiteSpace(payload.ToString()))
                _pub.SendNewMessage(payload.ToString(), headers!.ToString(), subject!.ToString());
        }


#warning Post but no return info about created resource?
    }

    #warning Why post and not delete?
    [HttpPost("deleteMessage")]
    public async Task<IActionResult> DeleteMessage()
    {
        string content = "";
        using (StreamReader stream = new StreamReader(Request.Body))
        {
            content = await stream.ReadToEndAsync();
        }

        var jsonObject = JsonNode.Parse(content);

        if (jsonObject != null && jsonObject["name"] != null && jsonObject["sequenceNumber"] != null)
        {
            string streamName = jsonObject["name"]!.ToString();
            ulong sequenceNumber = ulong.Parse(jsonObject["number"]!.ToString());
            bool erase = jsonObject["erase"]!.ToString() == "true";

            _streamManager.DeleteMessage(streamName, sequenceNumber, erase);
        }

        return Ok();
    }

    [HttpPost("newUserAccount")]
    public async Task<IActionResult> CreateUserAccount()
    {
        string content = "";
        using (StreamReader stream = new StreamReader(Request.Body))
        {
            content = await stream.ReadToEndAsync();
        }
        UserAccount.Name = JsonNode.Parse(content)!["name"]!.ToString();

        #warning No info about created resource returned
        return Ok();
    }


    #warning Which stream do we get it for? There's absolutely no parameters here?
    [HttpGet("streamBasicInfo")]
    public IActionResult GetBasicStreamInfo()
    {
        var res = _streamManager.GetBasicStreamInfo2();
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
        var res = _sub.GetMessages2();
        return Ok(res);
    }
}
