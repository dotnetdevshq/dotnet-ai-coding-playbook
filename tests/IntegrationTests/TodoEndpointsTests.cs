using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Playbook.IntegrationTests;

/// <summary>
/// Runs the real API in memory and calls it over HTTP.
/// </summary>
public sealed class TodoEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TodoEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostThenGetReturnsTheCreatedTodo()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage createResponse = await client.PostAsJsonAsync("/todos", new { title = "Write the playbook" });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        TodoResponse? created = await createResponse.Content.ReadFromJsonAsync<TodoResponse>();
        Assert.NotNull(created);
        Assert.Equal("Write the playbook", created.Title);
        Assert.False(created.IsCompleted);

        using HttpResponseMessage getResponse = await client.GetAsync($"/todos/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        TodoResponse? fetched = await getResponse.Content.ReadFromJsonAsync<TodoResponse>();
        Assert.Equal(created, fetched);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task PostWithAnEmptyTitleReturnsBadRequest(string title)
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsJsonAsync("/todos", new { title });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetWithAnUnknownIdReturnsNotFound()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync($"/todos/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private sealed record TodoResponse(Guid Id, string Title, bool IsCompleted);
}
