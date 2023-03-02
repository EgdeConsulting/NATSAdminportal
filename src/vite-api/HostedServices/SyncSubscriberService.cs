using System.Runtime.ExceptionServices;
using NATS.Client;
using vite_api.Repositories;

namespace vite_api.HostedServices;

public sealed class SyncSubscriberService : BackgroundService
{
    private const string Subject = ">";
    private const int Timeout = 1000;

    private readonly MessageRepository _messages;
    private readonly IConnection _connection;

    public SyncSubscriberService(
        MessageRepository messages,
        IConnection connection)
    {
        _messages = messages;
        _connection = connection;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ExceptionDispatchInfo? runThreadException = null;
        var thread = new Thread(() => Run(stoppingToken, out runThreadException));
        thread.Start();

        try
        {
            // Check on the thread every 500ms so we can react if something went wrong.
            while (!stoppingToken.IsCancellationRequested && runThreadException == null && thread.IsAlive)
                await Task.Delay(500, stoppingToken);

            // Throw the captured exception if we have one.
            // Because we're using ExceptionDispatchInfo, the original stack trace
            // is preserved, as well as the one to this location.
            runThreadException?.Throw();
        }
        catch (TaskCanceledException)
        {
            // Ignoring this as it's expected
        }
        finally
        {
            thread.Join();
        }
    }

    private void Run(CancellationToken stoppingTokenObj, out ExceptionDispatchInfo? exception)
    {
        exception = null;

        try
        {
            using var sub = _connection.SubscribeSync(Subject);

            while (!stoppingTokenObj.IsCancellationRequested)
            {
                try
                {
                    var msg = sub.NextMessage(Timeout);
                    _messages.AddMessage(DateTime.UtcNow, msg);
                }
                catch (NATSTimeoutException)
                {
                    // It's so dumb NATS is using an exception to indicate
                    // a timeout we set occured. Oh well, ignoring it :/
                }
            }

            sub.Drain();
        }
        catch (Exception ex)
        {
            // Background threads are nasty places for exceptions as they're hard to debug.
            // Return the exception to our caller, which will throw it.
            exception = ExceptionDispatchInfo.Capture(ex);
        }
    }
}
