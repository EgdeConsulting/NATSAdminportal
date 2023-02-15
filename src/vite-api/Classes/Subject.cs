using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Backend.Logic
{
    public class Subject
    {
        public string SubjectName { get; }

        public List<Dictionary<int, Subject>> ParentLinks { get; set; }

        public List<Dictionary<int, Subject>> ChildrenLinks { get; set; }

        public Subject(string subjectName)
        {
            SubjectName = subjectName;
            ParentLinks = new List<Dictionary<int, Subject>>();
            ChildrenLinks = new List<Dictionary<int, Subject>>();
        }

        public bool ParentLinkExists(Subject parent)
        {
            bool exists = false;
            foreach (Dictionary<int, Subject> dict in ParentLinks)
            {
                foreach (KeyValuePair<int, Subject> entry in dict)
                {
                    if (entry.Value.SubjectName.Equals(parent.SubjectName))
                    {
                        exists = true;
                        break;
                    }
                }
            }
            return exists;
        }

        public bool ChildrenLinkExists(Subject child)
        {
            bool exists = false;
            foreach (Dictionary<int, Subject> dict in ChildrenLinks)
            {
                foreach (KeyValuePair<int, Subject> entry in dict)
                {
                    if (entry.Value.SubjectName.Equals(child.SubjectName))
                    {
                        exists = true;
                        break;
                    }
                }
            }
            return exists;
        }

        public string ToJSON(int branchID, int childrenCount)
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
                json = json.Substring(0, json.Length - 1) + ",";

                string childrenJSON = "[";

                foreach (Dictionary<int, Subject> dict in ChildrenLinks)
                {
                    foreach (KeyValuePair<int, Subject> entry in dict)
                    {
                        if (branchID == -1 || Math.Abs(branchID - entry.Key) < childrenCount)
                        {
                            childrenJSON += entry.Value.ToJSON(entry.Key, entry.Value.ChildrenLinks.Count) + ", ";
                        }
                    }
                }

                childrenJSON = childrenJSON.Substring(0, childrenJSON.Length - 2) + "]";

                return json + @"""subSubjects"": " + childrenJSON + "}";
            }
        }
    }
}