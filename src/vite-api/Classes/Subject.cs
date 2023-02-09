using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Backend.Logic
{
    public class Subject
    {
        public string SubjectName { get; }

        public List<Subject> ParentLinks { get; set; }

        public List<Subject> ChildrenLinks { get; set; }

        public Subject(string subjectName)
        {
            SubjectName = subjectName;
            ParentLinks = new List<Subject>();
            ChildrenLinks = new List<Subject>();
        }

        public bool ParentLinkExists(Subject parent)
        {
            return ParentLinks.Any(x => x.SubjectName.Equals(parent.SubjectName));
        } 

        public bool ChildrenLinkExists(Subject child)
        {
            return ChildrenLinks.Any(x => x.SubjectName.Equals(child.SubjectName));
        } 

        public string ToJSON() 
        {
            string json = JsonSerializer.Serialize(
                new
                {
                    name = SubjectName,
                }
            );

            if (ChildrenLinks.Count == 0)
            {
                return json;
            }
            else 
            {
                // Include parent check here to prevent duplicates!
                json = json.Substring(0, json.Length - 1) + ",";

                string childrenJSON = "[";

                foreach (Subject child in ChildrenLinks)
                {
                    childrenJSON += child.ToJSON() + ", ";
                }
                childrenJSON = childrenJSON.Substring(0, childrenJSON.Length - 2) + "]";

                return json + " subSubjects: " + childrenJSON + "}";
            }
        }
    }
}