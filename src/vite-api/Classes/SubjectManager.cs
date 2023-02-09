using System.Collections.Generic;
using System.Linq;
using System;

namespace Backend.Logic
{
    public static class SubjectManager
    {
        private static List<Subject> allSubjects = new List<Subject>();

        public static void AddSubject(string subjectName, string? parentName, string? childName)
        {
            Subject? obj = SubjectExists(subjectName);

            if (obj is null) 
            {
                allSubjects.Add(new Subject(subjectName));
            }
            else 
            {
                if (parentName is Object)
                {
                    Subject? parent = SubjectExists(parentName);
                    if (parent is Object && !obj.ParentLinkExists(parent))
                    {
                        obj.ParentLinks.Add(parent);
                    }
                }

                if (childName is Object)
                {
                    Subject? child = SubjectExists(childName);
                    if (child is Object && !obj.ChildrenLinkExists(child))
                    {
                        obj.ChildrenLinks.Add(child);
                    }
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

        // // Desired outcome? [[subject, [[A, [1, 2]], [B, [1, 2]]]], [subject2, [A, C], [1, 2]]]]
        public static void GetHierarchy()
        {
            Console.WriteLine(allSubjects.Where(x => x.SubjectName.Equals("subject")).FirstOrDefault().ToJSON());
            // List<Subject> firstLevelSubjects = new List<Subject>();
            // allSubjects.Where(a => a.ParentSubject == null).ToList().ForEach(b =>
            // {
            //     //THIS IS ALL DONE AFTER 09.02 SESSION -- DANIEL

            //     if (!SubjectExists(b.SubjectName, firstLevelSubjects))
            //     {
            //         Console.WriteLine(b.SubjectName);
            //         firstLevelSubjects.Add(b);
            //     }
            // });
            // foreach (Subject item in firstLevelSubjects)
            // {
            //     //Console.WriteLine(item.ParentSubject + " " + item.SubjectName + " " + item.ChildSubject);
            //     // firstLevelSubjects can be filtered after the fact. In our example this would then reduce firstLevelSubjects...
            // }
            // // Consider changing subject hierarchy to non-static as to create spesific objects based on first order subject....
            // foreach (Subject item in allSubjects) //All subject paths are here, cant filter out the orders instead?
            // {
            //     //Console.WriteLine(item.ParentSubject + " " + item.SubjectName + " " + item.ChildSubject);
            //     // firstLevelSubjects can be filtered after the fact. In our example this would then reduce firstLevelSubjects...
            // }
            // //////////////////////////////////////////////

            // List<Subject> secondLevelSubjects = new List<Subject>();
            // allSubjects.Where(a => a.ParentSubject != null).ToList().
            //     ForEach(a =>
            //     {
            //         firstLevelSubjects.Where(b => b.SubjectName == a.ParentSubject).ToList().ForEach(c => secondLevelSubjects.Add(a));
            //     });

            // List<Subject> thirdLevelSubjects = new List<Subject>();
            // allSubjects.Where(a => a.ParentSubject != null).ToList().
            //     ForEach(a =>
            //     {
            //         secondLevelSubjects.Where(b => b.SubjectName == a.ParentSubject).ToList().ForEach(c => thirdLevelSubjects.Add(a));
            //     });

            // List<string[]> linkBetweenFirstAndThird = new List<string[]>();
            // thirdLevelSubjects.ForEach(a =>
            //     {
            //         secondLevelSubjects.Where(b => b.SubjectName == a.ParentSubject).ToList().ForEach(c =>
            //     {
            //         firstLevelSubjects.Where(d => d.ChildSubject == c.SubjectName).ToList().ForEach(e => linkBetweenFirstAndThird.Add(new string[] { e.SubjectName, c.SubjectName, a.SubjectName }));
            //     });
            //     });

            // foreach (string[] el in linkBetweenFirstAndThird)
            // {
            //     //Console.WriteLine("[" + el[0] + ", " + el[1] + ", " + el[2] + "]");
            // }
            // //Console.WriteLine(secondLevelSubjects.Count);

            // // foreach (Subject el in linkBetweenFirstAndThird)
            // // {
            // //     Console.WriteLine(el.ParentSubject + " " + el.SubjectName + " " + el.ChildSubject);
            // // }
        }
    }
}