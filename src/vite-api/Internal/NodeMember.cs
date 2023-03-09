using System.Text.Json.Serialization;

namespace vite_api.Internal;

[JsonConverter(typeof(NodeMemberJsonConverter))]
public sealed class NodeMember<T>
{
    private readonly Dictionary<string, NodeMember<T>> _children = new();

    public NodeMember()
    {
        // This is the root node
        Name = string.Empty;
        Value = default;
    }

    private NodeMember(string name, T? value)
    {
        if (string.IsNullOrWhiteSpace(name))
            ArgumentException.ThrowIfNullOrEmpty(name);

        Name = name;
        Value = value;
    }

    public string Name { get; }
    public T? Value { get; }
    public IEnumerable<NodeMember<T>> Children => _children.Values;

    public NodeMember<T> AddChild(string name, T? value)
    {
        if (!_children.TryGetValue(name, out var node))
        {
            node = new NodeMember<T>(name, value);
            _children[name] = node;
        }

        return node;
    }
}
