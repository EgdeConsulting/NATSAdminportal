using vite_api.Classes;

namespace vite_api.Internal;

public sealed class InputValidation
{
    private readonly SubjectManager _subjectManager;
    
    public InputValidation(SubjectManager subjectManager)
    {
        _subjectManager = subjectManager;
    }
    
    public void DeleteValidation(string streamName, ulong  sequenceNumber)
    {
        
    }

    public void AsciiChars(string str)
    {
        
    }


}