using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Backend.Logic
{
    public class Subject
    {
        public List<Dictionary<int, Subject>> ParentLinks { get; set; }
        public List<Dictionary<int, Subject>> ChildrenLinks { get; set; }

        public string SubjectName { get; }

        public Subject(string subjectName)
        {
            SubjectName = subjectName;
            ParentLinks = new List<Dictionary<int, Subject>>();
            ChildrenLinks = new List<Dictionary<int, Subject>>();
        }

        public bool ParentLinkExists(Subject parent)
        {
            foreach (Dictionary<int, Subject> dict in ParentLinks)
            {
                foreach (KeyValuePair<int, Subject> entry in dict)
                {
                    if (entry.Value.SubjectName.Equals(parent.SubjectName))
                        return true;
                }
            }
            return false;
        }

        public bool ChildrenLinkExists(Subject child)
        {
            foreach (Dictionary<int, Subject> dict in ChildrenLinks)
            {
                foreach (KeyValuePair<int, Subject> entry in dict)
                {
                    if (entry.Value.SubjectName.Equals(child.SubjectName))
                        return true;
                }
            }
            return false;
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
                            childrenJSON += entry.Value.ToJSON(entry.Key, entry.Value.ChildrenLinks.Count) + ", ";
                    }
                }
                childrenJSON = childrenJSON.Substring(0, childrenJSON.Length - 2) + "]";
                return json + @"""subSubjects"": " + childrenJSON + "}";
            }
        }

        public string ToString(int branchID, int childrenCount, string parentName = "")
        {
            string subjects = branchID == -1 ? SubjectName : parentName + "." + SubjectName;
            if (ChildrenLinks.Count == 0)
            {
                return subjects;
            }
            else
            {
                string childrenSubjects = "";
                for (int i = 0; i < ChildrenLinks.Count; i++)
                {
                    foreach (KeyValuePair<int, Subject> entry in ChildrenLinks[i])
                    {
                        if (branchID == -1 || Math.Abs(branchID - entry.Key) < childrenCount)
                            childrenSubjects += entry.Value.ToString(entry.Key, entry.Value.ChildrenLinks.Count, branchID == -1 ? SubjectName : parentName + "." + SubjectName) + ",";
                    }
                    childrenSubjects = i == ChildrenLinks.Count - 1 ? childrenSubjects.Substring(0, childrenSubjects.Length - 1) : childrenSubjects;
                }
                return subjects + "," + childrenSubjects;
            }
        }
    }
}