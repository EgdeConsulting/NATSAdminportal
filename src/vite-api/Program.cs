using System.Text.Json.Nodes;
using Backend.Logic;

//////////////////////
// Building the app //
//////////////////////

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSpaStaticFiles(config =>
{
    config.RootPath = "dist";
});

var app = builder.Build();

///////////////////////////////
// Loading data from secrets //
///////////////////////////////

string? natsServerURL;

if (app.Environment.IsDevelopment())
{
    var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

    natsServerURL = config["LOCAL_NATS_SERVER_URL"];
}
else
{
    natsServerURL = Environment.GetEnvironmentVariable("AZURE_NATS_SERVER_URL");
}

/////////////////////////
// Configuring the app //
/////////////////////////

app.UseRouting();
app.UseEndpoints(_ => { });
app.UseSpaStaticFiles();
app.UseSpa(builder =>
{
    if (app.Environment.IsDevelopment())
        builder.UseProxyToSpaDevelopmentServer("http://localhost:5173/");
});

//////////////////////////////////
// Initializing crucial objects //
//////////////////////////////////

SubjectManager subjectManager = new SubjectManager(natsServerURL);
StreamManager streamManager = new StreamManager(natsServerURL);

Subscriber sub = new Subscriber(natsServerURL, subjectManager);
Thread thread = new Thread(sub.Run);
thread.Start();

Publisher pub = new Publisher("EgdeTest", natsServerURL);
Publisher pub2 = new Publisher(natsServerURL);

///////////////////////////////////////////////////
// Adding API-endpoints for data retrieval (GET) //
///////////////////////////////////////////////////

app.MapGet("/Subjects", () => subjectManager.GetSubjectHierarchy());
app.MapGet("/SubjectNames", () => subjectManager.GetSubjectNames());
app.MapGet("/LastMessages", () => sub.GetLatestMessages());

///////////////////////////////////////////////////
// Adding API-endpoints for data delivery (POST) //
///////////////////////////////////////////////////

app.MapPost("/NewSubject", async (HttpRequest request) =>
{
    string content = "";
    using (StreamReader stream = new StreamReader(request.Body))
    {
        content = await stream.ReadToEndAsync();
    }

    var jsonObject = JsonNode.Parse(content);

    if (jsonObject != null && jsonObject["subject"] != null)
    {
        var subject = jsonObject["subject"];

        if (subject != null && !string.IsNullOrWhiteSpace(subject.ToString()))
            sub.MessageSubject = subject.ToString();
    }
});

app.MapPost("/PublishFullMessage", async (HttpRequest request) =>
{
    string content = "";
    using (StreamReader stream = new StreamReader(request.Body))
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
            pub.SendNewMessage(payload.ToString(), headers!.ToString(), subject!.ToString());
    }
});

app.MapPost("/PublishMessage", async (HttpRequest request) =>
{
    string content = "";
    using (StreamReader stream = new StreamReader(request.Body))
    {
        content = await stream.ReadToEndAsync();
    }

    var jsonObject = JsonNode.Parse(content);

    if (jsonObject != null && jsonObject["payload"] != null)
    {
        var payload = jsonObject["payload"];

        if (payload != null && !string.IsNullOrWhiteSpace(payload.ToString()))
            pub.SendNewMessage(payload.ToString());
    }
});

app.MapPost("/DeleteMessage", async (HttpRequest request) =>
{
    string content = "";
    using (StreamReader stream = new StreamReader(request.Body))
    {
        content = await stream.ReadToEndAsync();
    }

    var jsonObject = JsonNode.Parse(content);

    if (jsonObject != null && jsonObject["name"] != null && jsonObject["sequenceNumber"] != null)
    {
        string streamName = jsonObject["name"]!.ToString();
        ulong sequenceNumber = ulong.Parse(jsonObject["number"]!.ToString());
        bool erase = jsonObject["erase"]!.ToString() == "true";

        streamManager.DeleteMessage(streamName, sequenceNumber, erase);
    }
});

//////////////////////
// Starting the app //
//////////////////////

app.Run();