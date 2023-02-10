using System.Collections.Generic;
using System.Linq;
using System;

namespace Backend.Logic
{
    public static class SubjectManager
    {
        private static List<Subject> allSubjects = new List<Subject>();

        public static void AddSubject(string subjectName)
        {
            if (SubjectExists(subjectName) is null) 
            {
                allSubjects.Add(new Subject(subjectName));
            }
        }

        public static void AddSubjectLinks(string subjectName, string? parentName, string? childName, int branchID)
        {
            Subject? obj = SubjectExists(subjectName);

            if (parentName is Object)
            {
                Subject? parent = SubjectExists(parentName);
                if (parent is Object && !obj.ParentLinkExists(parent))
                {
                    obj.ParentLinks.Add(new Dictionary<int, Subject>() {{ branchID, parent }});
                }
            }

            if (childName is Object)
            {
                Subject? child = SubjectExists(childName);
                if (child is Object && !obj.ChildrenLinkExists(child))
                {
                    obj.ChildrenLinks.Add(new Dictionary<int, Subject>() {{ branchID, child }});
                }
            }
        }


        public static void ClearSubjects()
        {
            allSubjects.Clear();
        }

        public static Subject? SubjectExists(string subjectName)
        {
            return allSubjects.FirstOrDefault(x => x.SubjectName.Equals(subjectName));
        }

        public static string GetHierarchy()
        {
            string hierarchyJSON = "[";

            allSubjects.Where(a => a.ParentLinks.Count == 0).ToList().
                ForEach(b =>
                {
                    hierarchyJSON += b.ToJSON(-1, b.ChildrenLinks.Count) + ", ";
                });

            return hierarchyJSON.Substring(0, hierarchyJSON.Length - 2) + "]";
        }
    }
}