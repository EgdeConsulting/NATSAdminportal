
namespace Backend.Logic
{
    public class Subject
    {
        public string SubjectName { get; }

        public string? ParentSubject { get; }

        public string? ChildSubject { get; }

        public Subject(string subjectName, string parentName, string childSubject)
        {
            SubjectName = subjectName;
            ParentSubject = parentName;
            ChildSubject = childSubject;
        }
    }
}