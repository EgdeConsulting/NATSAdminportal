using NATS.Client;

namespace vite_api.Internal;

public sealed class SubjectValidation
{
    public static bool SubjectExists(IConnection c, string subject)
    {
        var subjects = c
            .CreateJetStreamManagementContext()
            .GetStreams()
            .SelectMany(x => x.Config.Subjects)
            .Distinct()
            .ToList();
        return subjects.Contains(subject);
    }
}