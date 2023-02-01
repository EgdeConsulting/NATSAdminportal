using System.Text.Json.Nodes;
using Backend.Logic;
using NATS.Client;
using NATS.Client.JetStream;
using System.Text.Json;

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
// app.UseStaticFiles(config => 
// {
//     config.RootPath = "dist";
// });

app.UseRouting();

app.UseEndpoints(_ => { });

// app.Use((context, next) => 
// {
//     if (context.Request.Path.StartsWithSegments("/LastMessages"))
//     {
//         context.Response.StatusCode = 404;
//         return Task.CompletedTask;
//     }

//     return next();
// });

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

// NEW CODE PER BRANCH 11
///////////////////////////////////////////////////////////
///
app.MapPost("/NewStream", async (HttpRequest request) =>
{
    string content = "";

    using (StreamReader stream = new StreamReader(request.Body))
    {
        content = await stream.ReadToEndAsync();
    }

    var jsonObject = JsonNode.Parse(content);

    if (jsonObject != null && jsonObject["StreamName"] != null)
    {
        var streamName = jsonObject["StreamName"];
        //var subject = jsonObject["Subject"]!;

        if (streamName != null && !string.IsNullOrWhiteSpace(streamName.ToString()))
        {
            using (IConnection c = new ConnectionFactory().CreateConnection(natsServerURL))
            {
                IJetStreamManagement jsm = c.CreateJetStreamManagementContext();
                JsUtils.CreateStreamWhenDoesNotExist(jsm, StorageType.File, streamName.ToString(), "test12345");
            }
        }
    }
});

app.MapGet("/StreamNames", () =>
{
    List<string> streamNames;
    string json = "[";

    using (IConnection c = new ConnectionFactory().CreateConnection(natsServerURL))
    {
        IJetStreamManagement jsm = c.CreateJetStreamManagementContext();
        streamNames = JsUtils.GetStreamNamesArray(jsm).ToList<string>();

        for (int i = 0; i < streamNames.Count; i++)
        {
            json += JsonSerializer.Serialize(
                new
                {
                    StreamNames = streamNames[i]
                }
            );
            json = i < streamNames.Count - 1 ? json + "," : json;
        }
    }

    return json + "]";
});

app.MapGet("/StreamInfo", () =>
{
    List<StreamInfo> streamInfo;
    string json = "[";

    using (IConnection c = new ConnectionFactory().CreateConnection(natsServerURL))
    {
        IJetStreamManagement jsm = c.CreateJetStreamManagementContext();
        streamInfo = JsUtils.GetStreamInfoArray(jsm).ToList<StreamInfo>();

        for (int i = 0; i < streamInfo.Count; i++)
        {
            json += JsonSerializer.Serialize(
                new
                {
                    StreamInfo = streamInfo[i]
                }
            );
            json = i < streamInfo.Count - 1 ? json + "," : json;
        }
    }

    return json + "]";
});
/////////////////////////////////////////////////////////////

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

// app.MapPost("/AddStream", async (HttpRequest request) =>
// {
//     pub.CreateStream();
// });

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

