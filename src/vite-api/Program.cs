using System.Text.Json.Nodes;
using Backend.Logic;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSpaStaticFiles(config =>
{
    config.RootPath = "dist";
});

var app = builder.Build();

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

//var policyName = "enableCORS";

// builder.Services.AddCors(options =>
// {
//     options.AddPolicy(policyName,
//                           policy =>
//                           {
//                               policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
//                           });
// });

// app.UseCors(policyName);

app.UseRouting();

app.UseEndpoints(_ => { });

app.UseSpaStaticFiles();

app.UseSpa(builder =>
{
    if (app.Environment.IsDevelopment())
        builder.UseProxyToSpaDevelopmentServer("http://localhost:5173/");
});

Subscriber sub = new Subscriber(natsServerURL);
Thread thread = new Thread(sub.Run);
thread.Start();

Publisher pub = new Publisher("EgdeTest", natsServerURL);
Publisher pub2 = new Publisher(natsServerURL);

app.MapGet("/StreamInfo", () => Streams.GetStreamNames(natsServerURL));
app.MapGet("/Subjects", () => Streams.GetStreamSubjects(natsServerURL));
app.MapGet("/ConsumerInfo", () => Consumers.GetConsumerNamesForAStream(natsServerURL, "stream1"));
app.MapGet("/LastMessages", () => sub.GetLatestMessages());

app.MapPost("/NewSubject", async (HttpRequest request) =>
{
    string content = "";
    using (StreamReader stream = new StreamReader(request.Body))
    {
        content = await stream.ReadToEndAsync();
    }

    var jsonObject = JsonNode.Parse(content);

    if (jsonObject != null && jsonObject["Subject"] != null)
    {
        var subject = jsonObject["Subject"];

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

    if (jsonObject != null && jsonObject["Payload"] != null)
    {
        var payload = jsonObject["Payload"];
        var subject = jsonObject["Subject"];
        var headers = jsonObject["Headers"];

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

    if (jsonObject != null && jsonObject["Payload"] != null)
    {
        var payload = jsonObject["Payload"];

        if (payload != null && !string.IsNullOrWhiteSpace(payload.ToString()))
            pub.SendNewMessage(payload.ToString());
    }
});

app.Run();

