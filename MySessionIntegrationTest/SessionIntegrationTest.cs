using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MySessionIntegrationTest;

public class SessionIntegrationTest : IClassFixture<WebApplicationFactory<Session_Clone.Program>>
{
    private readonly HttpClient _client_factory;

    public SessionIntegrationTest(WebApplicationFactory<Session_Clone.Program> factory)
    {
        _client_factory = factory.CreateClient();
    }

    [Fact]
    public async Task Call_TestGetSession_Returns_Ok_Async()
    {
        var response = await _client_factory.GetAsync($"/TestSession/TestGetSession");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Call_Set_And_Get_SessionValue_Returns_Ok_Async()
    {
        string KeyTest = "KeyTest";
        string ValueTest = Guid.NewGuid().ToString();

        await _client_factory.GetAsync($"/TestSession/SetSessionValue?key={KeyTest}&value={ValueTest}");

        var response = await _client_factory.GetAsync($"/TestSession/GetSessionValue?key={KeyTest}");
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(ValueTest, responseString);
    }
}