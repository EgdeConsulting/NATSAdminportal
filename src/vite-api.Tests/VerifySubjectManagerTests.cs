using vite_api.Classes;
using System.Text.Json;
using Xunit.Abstractions;

namespace vite_api.Tests;

[UsesVerify, Collection("MockServer collection")]
public class VerifySubjectManagerTests
{
    private readonly MockServerFixture _fixture;

    private readonly ITestOutputHelper _testOutputHelper;
    public VerifySubjectManagerTests(MockServerFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _fixture = fixture;
        _testOutputHelper = testOutputHelper;
    }
    
    private SubjectManager CreateDefaultSubjectManager()
    {
        return new SubjectManager(_fixture.Provider);
    }
    
    [Fact]
    public void GetAllSubjects_ReturnsSameSubjects()
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
    public Task GetSubjectHierarchy_ShouldSerializeAsExpected()
    {
        var manager = CreateDefaultSubjectManager();

        var expectedHierarchy = _fixture.ValidSubjects;
        var actualHierarchy = manager.GetSubjectHierarchy();

        var json = JsonSerializer.Serialize(actualHierarchy);

        return VerifyJson(json);
    }
}