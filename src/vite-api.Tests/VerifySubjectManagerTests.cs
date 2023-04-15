using vite_api.Classes;

namespace vite_api.Tests;

[UsesVerify, Collection("JetStream collection")]
public class VerifySubjectManagerTests
{
    private readonly JetStreamFixture _fixture;

    public VerifySubjectManagerTests(JetStreamFixture fixture)
    {
        _fixture = fixture;
    }
    
    private SubjectManager CreateDefaultSubjectManager()
    {
        return new SubjectManager(_fixture.Provider);
    }
    
    [Fact]
    public void Get_AllSubjects_ReturnsSameSubjects()
    {
        var manager = CreateDefaultSubjectManager();

        var expectedSubjects = _fixture.ValidSubjects.ToList();
        var actualSubjects = manager.GetAllSubjects();
        
        // Can't check if expected and actual subjects are equal, because actual subjects
        // contains a lot of irrelevant subjects. This is due to the use of a public NATS-server
        // in the JetStream fixture. 
        //Assert.All(expectedSubjects, item => Assert.Contains(item, actualSubjects));
        Assert.Equal(expectedSubjects, actualSubjects);
    }
    
    [Fact]
    public void Get_SubjectHierarchy_ReturnsSameObject()
    {
        var manager = CreateDefaultSubjectManager();

        var expectedSubjects = _fixture.ValidSubjects;
        var actualSubjects = manager.GetSubjectHierarchy();
        
        //Assert.All(expectedSubjects, item => Assert.Contains(item, actualSubjects));
    }
}