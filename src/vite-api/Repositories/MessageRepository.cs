using NATS.Client;

namespace vite_api.Repositories;

public sealed class MessageRepository
{
    private readonly object _lockObj = new object();
    private readonly List<(DateTime Timestamp, Msg Message)> _messages = new();

    public void AddMessage(DateTime timestamp, Msg message)
    {
        lock (_lockObj)
            _messages.Add((timestamp, message));
    }

    public List<(DateTime Timestamp, Msg Message)> GetAll()
    {
        lock (_lockObj)
            return _messages.ToList();
    }

    public List<Msg> GetMessages()
    {
        lock (_lockObj)
            return _messages.Select(x => x.Message).ToList();
    }

    public List<DateTime> GetTimestamps()
    {
        lock (_lockObj)
            return _messages.Select(x => x.Timestamp).ToList();
    }
}
