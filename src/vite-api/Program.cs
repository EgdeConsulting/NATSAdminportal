using System.Text.Json;
using vite_api.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NATS.Client;
using vite_api.Classes;
using Options = NATS.Client.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddOptions<AppConfig>().BindConfiguration("");
builder.Services.AddControllers().AddJsonOptions(ConfigureJsonOptions);
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddSingleton<Publisher>()
   .AddSingleton<StreamManager>()
   .AddSingleton<SubjectManager>()
   .AddSingleton<SubscriberManager>();

builder.Services.AddTransient(NatsConnectionFactory);
//builder.Services.AddHostedService<SyncSubscriberService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

#pragma warning disable ASP0014
app.UseEndpoints(_ => { /* Needed for routing to work with SPA proxy */ });
#pragma warning restore ASP0014

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSpa(configure =>
    {
        configure.UseProxyToSpaDevelopmentServer("http://localhost:5173/");
    });
}

app.Run();

static void ConfigureJsonOptions(JsonOptions options)
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
}

static IConnection NatsConnectionFactory(IServiceProvider provider)
{
    var config = provider.GetRequiredService<IOptions<AppConfig>>();
    Options opts = ConnectionFactory.GetDefaultOptions();
    opts.Url = config.Value.NatsServerUrl;
    
    // The default handlers write a newline for each event, pretty annoying.
    opts.ClosedEventHandler += (sender, args) => { };
    opts.DisconnectedEventHandler += (sender, args) => { };
    
    return new ConnectionFactory().CreateConnection(opts);
}
