// using vite_api.Classes;
// using System.Text.Json;
//
// namespace vite_api.Tests;
//
// [UsesVerify, Collection("MockServer collection")]
// public class VerifySubjectManagerTests
// {
//     private readonly MockServerFixture _fixture;
//     
//     public VerifySubjectManagerTests(MockServerFixture fixture)
//     {
//         _fixture = fixture;
//     }
//     
//     private SubjectManager CreateDefaultSubjectManager()
//     {
//         return new SubjectManager(_fixture.Provider);
//     }
//     
//     [Fact]
//     public void GetAllSubjects_InSortedOrder_ReturnsSameSubjects()
//     {
//         var manager = CreateDefaultSubjectManager();
//         var validSubjects = _fixture.ValidSubjects.ToList();
//         validSubjects.Sort();
//         
//         var expectedSubjects = validSubjects;
//         var actualSubjects = manager.GetAllSubjects();
//
//         Assert.Equal(expectedSubjects, actualSubjects);
//     }
//     
//     [Fact]
//     public void GetAllSubjects_InUnsortedOrder_ReturnsNotEqual()
//     {
//         var manager = CreateDefaultSubjectManager();
//
//         var expectedSubjects = _fixture.ValidSubjects.ToList();
//         var actualSubjects = manager.GetAllSubjects();
//         
//         Assert.NotEqual(expectedSubjects, actualSubjects);
//     }
//     
//     [Fact]
//     public Task GetSubjectHierarchy_ShouldSerializeAsExpected()
//     {
//         var manager = CreateDefaultSubjectManager();
//
//         var actualHierarchy = manager.GetSubjectHierarchy();
//         var json = JsonSerializer.Serialize(actualHierarchy);
//         
//         return VerifyJson(json);
//     }
// }