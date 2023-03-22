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
        //private readonly IOptions<AppConfig> _appConfig;
        private readonly IServiceProvider _provider;
        //private List<Subject> _allSubjects;

        //private string Url => _appConfig.Value.NatsServerUrl ?? Defaults.Url;

        public SubjectManager(IServiceProvider provider)
        {
            //_appConfig = appConfig;
            _provider = provider;
            //_allSubjects = new List<Subject>();
            //InitializeSubjects(Url);
        }
        
        // private void InitializeSubjects(string? url)
        // {
        //     List<string> rawSubjects = new List<string>();
        //     List<string[]> refinedSubjects = new List<string[]>();
        //
        //     using (IConnection c = new ConnectionFactory().CreateConnection(url))
        //     {
        //         List<StreamInfo> streamInfo = c.CreateJetStreamManagementContext().GetStreams().ToList<StreamInfo>();
        //
        //         // Gets all subjects in form ["Subject.A.1", "Subject.A.2", ....]
        //         streamInfo.ForEach(a => rawSubjects.AddRange(a.Config.Subjects));
        //     }
        //
        //     rawSubjects.Sort();
        //     rawSubjects.ForEach(a => refinedSubjects.Add(a.Split(".")));
        //
        //
        //     for (int i = 0; i < refinedSubjects.Count; i++)
        //     {
        //         // Adding all unique subjects to list.
        //         for (int k = 0; k < refinedSubjects[i].Length; k++)
        //         {
        //             string subjectName = refinedSubjects[i][k];
        //             if (_allSubjects.FirstOrDefault(x => x.SubjectName.Equals(subjectName)) is null)
        //                 _allSubjects.Add(new Subject(subjectName));
        //         }
        //
        //         // Initializing the hierarchy links between all the subjects. 
        //         for (int k = 0; k < refinedSubjects[i].Length; k++)
        //         {
        //             string? parentName = k - 1 < 0 ? null : refinedSubjects[i][k - 1];
        //             string? childName = k + 1 > refinedSubjects[i].Length - 1 ? null : refinedSubjects[i][k + 1];
        //             AddSubjectLinks(refinedSubjects[i][k], parentName, childName, i);
        //         }
        //     }
        // }

        // private void AddSubjectLinks(string subjectName, string? parentName, string? childName, int branchId)
        // {
        //     Subject? obj = SubjectObjectExists(subjectName);
        //     if (parentName is Object)
        //     {
        //         Subject? parent = SubjectObjectExists(parentName);
        //         if (parent is Object && obj is Object && !obj.ParentLinkExists(parent))
        //             obj.ParentLinks.Add(new Dictionary<int, Subject>() { { branchId, parent } });
        //     }
        //
        //     if (childName is Object)
        //     {
        //         Subject? child = SubjectObjectExists(childName);
        //         if (child is Object && obj is Object && !obj.ChildrenLinkExists(child))
        //             obj.ChildrenLinks.Add(new Dictionary<int, Subject>() { { branchId, child } });
        //     }
        // }
        //
        // private Subject? SubjectObjectExists(string subjectName)
        // {
        //     return _allSubjects.FirstOrDefault(x => x.SubjectName.Equals(subjectName));
        // }
        
        /// <summary>
        /// Gets all unique subjects on all streams on the NATS-server.
        /// </summary>
        /// <returns>A collection containing the subject names.</returns>
        public List<string> GetAllSubjects()
        {
            // string json = "[";
            // allSubjects.Where(a => a.ParentLinks.Count == 0).ToList().
            //     ForEach(b =>
            //     {
            //         List<string> subjects = b.ToString(-1, b.ChildrenLinks.Count).Split(",").ToList();
            //         for (int i = 0; i < subjects.Count; i++)
            //         {
            //             json += JsonSerializer.Serialize(
            //                 new
            //                 {
            //                     name = subjects[i]
            //                 }
            //             );
            //             json = i < subjects.Count - 1 ? json + "," : json;
            //         }
            //         json += ",";   
            //     });
            // return json.Substring(0, json.Length - 1) + "]";
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

        // public bool SubjectExists(string subjectName)
        // {
        //     List<string> matches = new List<string>();
        //     _allSubjects.Where(a => a.ParentLinks.Count == 0).ToList().
        //         ForEach(b =>
        //         {
        //             List<string> subjects = b.ToString(-1, b.ChildrenLinks.Count).Split(",").ToList();
        //             matches = Enumerable.Concat(matches, subjects.Where(c => c.Equals(subjectName)).ToList()).ToList();
        //         });
        //     return matches.Count > 0;
        // }

        // public string GetSubjectHierarchy2()
        // {
        //     string hierarchyJSON = "[";
        //     allSubjects.Where(a => a.ParentLinks.Count == 0).ToList().
        //         ForEach(b =>
        //         {
        //             hierarchyJSON += b.ToJSON(-1, b.ChildrenLinks.Count) + ", ";
        //         });
        //     return hierarchyJSON.Substring(0, hierarchyJSON.Length - 2) + "]";
        // }
        
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
