using System.Text.Json;
using vite_api.Internal;

namespace vite_api.Tests;

[UsesVerify]
public class VerifyNodeMemberTests
{
    [Fact]
    public Task Should_serialize_as_expected()
    {
        var root = new NodeMember<object?>();
        var subjects = new[] {"a.a.a", "a.a.b", "a.b", "a.c", "b.b.a"};
        PopulateNodeTree(root, subjects);

        var json = JsonSerializer.Serialize(root);
        return VerifyJson(json);
    }

    private static void PopulateNodeTree(NodeMember<object?> root, string[] subjects)
    {
        foreach (var subject in subjects)
        {
            var node = root;
            foreach (var segment in subject.Split('.'))
                node = node.AddChild(segment, null);
        }
    }
}
