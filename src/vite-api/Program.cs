using System.Text.Json.Nodes;
using Backend.Logic;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

string? natsServerIp = args.Length != 0 ? args[0] : config["natsServerIp"];
string? natsServerPort = args.Length != 0 ? args[1] : config["natsServerPort"];

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSpaStaticFiles(config =>
{
    config.RootPath = "dist";
});

var app = builder.Build();

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

Subscriber sub = new Subscriber(natsServerIp + ":" + natsServerPort);
Thread thread = new Thread(sub.Run);
thread.Start();

Publisher pub = new Publisher("EgdeTest");

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

