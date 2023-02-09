
namespace Backend.Logic
{
    public static class SubjectHierarchy
    {
        private static List<Subject> allSubjects = new List<Subject>();

        public static void AddSubject(Subject newSubject)
        {
            allSubjects.Add(newSubject);
        }

        public static void ClearSubjects()
        {
            allSubjects.Clear();
        }

        public static bool SubjectExists(string subject, List<Subject> list)
        {

            return list.Any(x => x.SubjectName == subject);
        }

        // Desired outcome? [[subject, [[A, [1, 2]], [B, [1, 2]]]], [subject2, [A, C], [1, 2]]]]
        public static void GetHierarchy()
        {
            List<Subject> firstLevelSubjects = new List<Subject>();
            allSubjects.Where(a => a.ParentSubject == null).ToList().ForEach(b =>
            {
                //THIS IS ALL DONE AFTER 09.02 SESSION -- DANIEL

                if (!SubjectExists(b.SubjectName, firstLevelSubjects))
                {
                    Console.WriteLine(b.SubjectName);
                    firstLevelSubjects.Add(b);
                }
            });
            foreach (Subject item in firstLevelSubjects)
            {
                //Console.WriteLine(item.ParentSubject + " " + item.SubjectName + " " + item.ChildSubject);
                // firstLevelSubjects can be filtered after the fact. In our example this would then reduce firstLevelSubjects...
            }
            // Consider changing subject hierarchy to non-static as to create spesific objects based on first order subject....
            foreach (Subject item in allSubjects) //All subject paths are here, cant filter out the orders instead?
            {
                //Console.WriteLine(item.ParentSubject + " " + item.SubjectName + " " + item.ChildSubject);
                // firstLevelSubjects can be filtered after the fact. In our example this would then reduce firstLevelSubjects...
            }
            //////////////////////////////////////////////

            List<Subject> secondLevelSubjects = new List<Subject>();
            allSubjects.Where(a => a.ParentSubject != null).ToList().
                ForEach(a =>
                {
                    firstLevelSubjects.Where(b => b.SubjectName == a.ParentSubject).ToList().ForEach(c => secondLevelSubjects.Add(a));
                });

            List<Subject> thirdLevelSubjects = new List<Subject>();
            allSubjects.Where(a => a.ParentSubject != null).ToList().
                ForEach(a =>
                {
                    secondLevelSubjects.Where(b => b.SubjectName == a.ParentSubject).ToList().ForEach(c => thirdLevelSubjects.Add(a));
                });

            List<string[]> linkBetweenFirstAndThird = new List<string[]>();
            thirdLevelSubjects.ForEach(a =>
                {
                    secondLevelSubjects.Where(b => b.SubjectName == a.ParentSubject).ToList().ForEach(c =>
                {
                    firstLevelSubjects.Where(d => d.ChildSubject == c.SubjectName).ToList().ForEach(e => linkBetweenFirstAndThird.Add(new string[] { e.SubjectName, c.SubjectName, a.SubjectName }));
                });
                });

            foreach (string[] el in linkBetweenFirstAndThird)
            {
                //Console.WriteLine("[" + el[0] + ", " + el[1] + ", " + el[2] + "]");
            }
            //Console.WriteLine(secondLevelSubjects.Count);

            // foreach (Subject el in linkBetweenFirstAndThird)
            // {
            //     Console.WriteLine(el.ParentSubject + " " + el.SubjectName + " " + el.ChildSubject);
            // }
        }
    }
}