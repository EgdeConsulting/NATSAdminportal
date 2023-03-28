using System.Text.Json;
using Microsoft.Extensions.Options;
using NATS.Client;
using NATS.Client.JetStream;
using vite_api.Config;
using vite_api.Internal;

namespace vite_api.Classes
{
    public class SubjectManager
    {
        private readonly IOptions<AppConfig> appConfig;
        private List<Subject> allSubjects;

        private string Url => appConfig.Value.NatsServerUrl ?? Defaults.Url;

        public SubjectManager(IOptions<AppConfig> appConfig)
        {
            this.appConfig = appConfig;
            allSubjects = new List<Subject>();
            InitializeSubjects(Url);
        }



        private void InitializeSubjects(string? url)
        {
            List<string> rawSubjects = new List<string>();
            List<string[]> refinedSubjects = new List<string[]>();

            using (IConnection c = new ConnectionFactory().CreateConnection(url))
            {
                List<StreamInfo> streamInfo = c.CreateJetStreamManagementContext().GetStreams().ToList<StreamInfo>();

                // Gets all subjects in form ["Subject.A.1", "Subject.A.2", ....]
                streamInfo.ForEach(a => rawSubjects.AddRange(a.Config.Subjects));
            }

            rawSubjects.Sort();
            rawSubjects.ForEach(a => refinedSubjects.Add(a.Split(".")));


            for (int i = 0; i < refinedSubjects.Count; i++)
            {
                // Adding all unique subjects to list.
                for (int k = 0; k < refinedSubjects[i].Length; k++)
                {
                    string subjectName = refinedSubjects[i][k];
                    if (allSubjects.FirstOrDefault(x => x.SubjectName.Equals(subjectName)) is null)
                        allSubjects.Add(new Subject(subjectName));
                }

                // Initializing the hierarchy links between all the subjects. 
                for (int k = 0; k < refinedSubjects[i].Length; k++)
                {
                    string? parentName = k - 1 < 0 ? null : refinedSubjects[i][k - 1];
                    string? childName = k + 1 > refinedSubjects[i].Length - 1 ? null : refinedSubjects[i][k + 1];
                    AddSubjectLinks(refinedSubjects[i][k], parentName, childName, i);
                }
            }
        }

        private void AddSubjectLinks(string subjectName, string? parentName, string? childName, int branchID)
        {
            Subject? obj = SubjectObjectExists(subjectName);
            if (parentName is Object)
            {
                Subject? parent = SubjectObjectExists(parentName);
                if (parent is Object && obj is Object && !obj.ParentLinkExists(parent))
                    obj.ParentLinks.Add(new Dictionary<int, Subject>() { { branchID, parent } });
            }

            if (childName is Object)
            {
                Subject? child = SubjectObjectExists(childName);
                if (child is Object && obj is Object && !obj.ChildrenLinkExists(child))
                    obj.ChildrenLinks.Add(new Dictionary<int, Subject>() { { branchID, child } });
            }
        }

        private Subject? SubjectObjectExists(string subjectName)
        {
            return allSubjects.FirstOrDefault(x => x.SubjectName.Equals(subjectName));
        }

        /// <summary>
        /// Gets all unique subjects on all streams on the NATS-server.
        /// </summary>
        /// <returns>A collection containing the subject names.</returns>
        public List<string> GetAllSubjects()
        {
            var url = appConfig.Value.NatsServerUrl ?? Defaults.Url;
            using var connection = new ConnectionFactory().CreateConnection(url);
            var subjects = connection
                .CreateJetStreamManagementContext()
                .GetStreams()
                .SelectMany(x => x.Config.Subjects)
                .Distinct()
                .ToList();
            subjects.Sort();
            return subjects;
        }

        public bool SubjectExists(string subjectName)
        {
            List<string> matches = new List<string>();
            allSubjects.Where(a => a.ParentLinks.Count == 0).ToList().
                ForEach(b =>
                {
                    List<string> subjects = b.ToString(-1, b.ChildrenLinks.Count).Split(",").ToList();
                    matches = Enumerable.Concat(matches, subjects.Where(c => c.Equals(subjectName)).ToList()).ToList();
                });
            return matches.Count > 0;
        }

        /// <summary>
        /// Creates a hierarchy trees based on all of the subjects on the NATS-server.
        /// The hierarchy tree shows all of the subjects and their respective parent
        /// and child subjects independent of which stream the subjects may belong to.   
        /// </summary>
        /// <returns>A NodeMember object containing the hierarchy structure.</returns>
        public NodeMember<object?> GetSubjectHierarchy()
        {
            var url = appConfig.Value.NatsServerUrl ?? Defaults.Url;
            using var connection = new ConnectionFactory().CreateConnection(url);
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
