using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Transactions;
using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Enums;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Tests.Controllers;

public class TaskControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _client;
    private readonly TransactionScope _transaction;

    private const string TaskApi = "/api/Task";
    private const string AuthApi = "/api/Auth";
    private const string DefaultDescription = "Test Description";

    public TaskControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    }

    public void Dispose()
    {
        _transaction.Dispose();
        _client.Dispose();
    }

    [Fact]
    public async Task Create_And_Get_Task()
    {
        // Arrange
        var user = await RegisterTestUser();
        SetBearer(user.Token);

        var dto = await CreateSampleTask("Basic Task", user.Id);

        // Act
        var result = await _client.GetFromJsonAsync<GetTaskDto>($"{TaskApi}/{dto.Id}");

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Basic Task");
    }

    [Fact]
    public async Task Update_Task_Success()
    {
        // Arrange
        var user = await RegisterTestUser();
        SetBearer(user.Token);

        var dto = await CreateSampleTask("Update Me", user.Id);
        var update = new UpdateTaskDto
        {
            Id = dto.Id,
            Title = "Updated Title",
            Description = "Updated Desc",
            Status = TaskStatus.Completed,
            UpdatedAt = DateTime.UtcNow,
            RowVersion = dto.RowVersion
        };

        // Act
        var response = await _client.PutAsJsonAsync($"{TaskApi}/{dto.Id}", update);
        var updated = await _client.GetFromJsonAsync<GetTaskDto>($"{TaskApi}/{dto.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        updated.Should().NotBeNull();
        updated.Title.Should().Be("Updated Title");
        updated.Description.Should().Be("Updated Desc");
    }

    [Fact]
    public async Task Delete_Task_Should_NotFound_Afterward()
    {
        // Arrange
        var user = await RegisterTestUser();
        SetBearer(user.Token);

        var dto = await CreateSampleTask("Delete Me", user.Id);

        // Act
        var delete = await _client.DeleteAsync($"{TaskApi}/{dto.Id}");
        var check = await _client.GetAsync($"{TaskApi}/{dto.Id}");

        // Assert
        delete.StatusCode.Should().Be(HttpStatusCode.NoContent);
        check.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_Task_With_Conflict_Should_Return_409()
    {
        // Arrange
        var user = await RegisterTestUser();
        SetBearer(user.Token);

        var dto = await CreateSampleTask("Concurrency Task", user.Id);
        var update = new UpdateTaskDto
        {
            Id = dto.Id,
            Title = "First Update",
            Description = "Updated",
            Status = TaskStatus.InProgress,
            UpdatedAt = DateTime.UtcNow,
            RowVersion = dto.RowVersion
        };

        // Act
        var first = await _client.PutAsJsonAsync($"{TaskApi}/{dto.Id}", update);
        update.Title = "Second Update";
        var second = await _client.PutAsJsonAsync($"{TaskApi}/{dto.Id}", update);

        // Assert
        first.StatusCode.Should().Be(HttpStatusCode.NoContent);
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Filter_Tasks_By_Status()
    {
        // Arrange
        var user = await RegisterTestUser();
        SetBearer(user.Token);

        await CreateSampleTask("Todo", user.Id, TaskStatus.Backlog);
        await CreateSampleTask("Done", user.Id, TaskStatus.InProgress);

        // Act
        var response = await _client.GetAsync($"{TaskApi}/user/{user.Id}?status={(int)TaskStatus.InProgress}");
        var list = await response.Content.ReadFromJsonAsync<List<GetTaskDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        list.Should().NotBeNull();
        list.Should().OnlyContain(t => t.Status == TaskStatus.InProgress);
    }

    [Fact]
    public async Task Paging_Task_Result()
    {
        // Arrange
        var user = await RegisterTestUser();
        SetBearer(user.Token);

        for (int i = 0; i < 12; i++)
            await CreateSampleTask($"Paged Task {i}", user.Id);

        // Act
        var response = await _client.GetAsync($"{TaskApi}/user/{user.Id}?page=2&pageSize=5");
        var list = await response.Content.ReadFromJsonAsync<IEnumerable<GetTaskDto>?>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        list.Should().NotBeNull();
        list.Count().Should().Be(5);
    }

    private async Task<GetTaskDto> CreateSampleTask(string title, Guid userId, TaskStatus status = TaskStatus.Backlog)
    {
        var dto = new CreateTaskDto
        {
            Title = title,
            Description = DefaultDescription,
            Status = status,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        var res = await _client.PostAsJsonAsync(TaskApi, dto);
        res.EnsureSuccessStatusCode();
        var task = await res.Content.ReadFromJsonAsync<GetTaskDto>() ?? throw new Exception("Null task");

        // Assign user to task (simulate UserTasks table)
        var assignRes = await _client.PostAsync($"{TaskApi}/{task.Id}/assign/{userId}", null);
        assignRes.EnsureSuccessStatusCode();

        return task;
    }

    private async Task<UserWithToken> RegisterTestUser()
    {
        var dto = new RegisterRequestDto
        {
            Username = $"user_{Guid.NewGuid():N}".Substring(0, 10),
            Password = "123456",
            Role = UserRole.User
        };

        var res = await _client.PostAsJsonAsync($"{AuthApi}/register", dto);
        res.EnsureSuccessStatusCode();

        var auth = await res.Content.ReadFromJsonAsync<AuthResponseDto>();
        auth.Should().NotBeNull();

        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(auth!.Token);
        var sub = token.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        sub.Should().NotBeNull();

        return new UserWithToken
        {
            Id = Guid.Parse(sub!),
            Username = dto.Username,
            Role = dto.Role,
            Token = auth.Token
        };
    }

    private void SetBearer(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private class UserWithToken
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public UserRole Role { get; set; }
        public string Token { get; set; } = null!;
    }
}
