using NATS.Client;
using vite_api.Internal;

namespace vite_api.Classes
{
    public class SubjectManager
    {
        private readonly IServiceProvider _provider;

        public SubjectManager(IServiceProvider provider)
        {
            _provider = provider;
        }
        
        /// <summary>
        /// Gets all unique subjects on all streams on the NATS-server.
        /// </summary>
        /// <returns>A collection containing the subject names.</returns>
        public List<string> GetAllSubjects()
        {
            using var connection = _provider.GetRequiredService<IConnection>();
            var subjects = connection
                .CreateJetStreamManagementContext()
                .GetStreams()
                .SelectMany(x => x.Config.Subjects)
                .Distinct()
                .ToList();
            subjects.Sort();
            return subjects;
        }

        /// <summary>
        /// Creates a hierarchy trees based on all of the subjects on the NATS-server.
        /// The hierarchy tree shows all of the subjects and their respective parent
        /// and child subjects independent of which stream the subjects may belong to.   
        /// </summary>
        /// <returns>A NodeMember object containing the hierarchy structure.</returns>
        public NodeMember<object?> GetSubjectHierarchy()
        {
            using var connection = _provider.GetRequiredService<IConnection>();
            var subjects = connection
               .CreateJetStreamManagementContext()
               .GetStreams()
               .SelectMany(x => x.Config.Subjects)
               .Distinct()
               .ToList();

            var root = new NodeMember<object?>();
            foreach (var subject in subjects)
            {
                var node = root;
                foreach (var segment in subject.Split('.'))
                    node = node.AddChild(segment, null);
            }

            return root;
        }
    }
}
