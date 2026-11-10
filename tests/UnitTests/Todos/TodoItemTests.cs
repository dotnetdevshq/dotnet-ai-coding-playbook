using Playbook.Domain.Todos;

namespace Playbook.UnitTests.Todos;

public sealed class TodoItemTests
{
    private static readonly DateTimeOffset Now = new(2026, 1, 15, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public void CreateTrimsTheTitle()
    {
        TodoItem todo = TodoItem.Create("  Write the playbook  ", Now);

        Assert.Equal("Write the playbook", todo.Title);
        Assert.Equal(Now, todo.CreatedAt);
        Assert.False(todo.IsCompleted);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateRejectsAnEmptyTitle(string title)
    {
        Assert.Throws<ArgumentException>(() => TodoItem.Create(title, Now));
    }

    [Fact]
    public void CreateRejectsATitleThatIsTooLong()
    {
        string title = new('a', TodoItem.MaxTitleLength + 1);

        Assert.Throws<ArgumentException>(() => TodoItem.Create(title, Now));
    }

    [Fact]
    public void CreateAcceptsATitleOfExactlyTheMaximumLength()
    {
        string title = new('a', TodoItem.MaxTitleLength);

        TodoItem todo = TodoItem.Create(title, Now);

        Assert.Equal(TodoItem.MaxTitleLength, todo.Title.Length);
    }

    [Fact]
    public void CompleteRecordsTheCompletionTime()
    {
        TodoItem todo = TodoItem.Create("Write the playbook", Now);
        DateTimeOffset completedAt = Now.AddHours(2);

        todo.Complete(completedAt);

        Assert.True(todo.IsCompleted);
        Assert.Equal(completedAt, todo.CompletedAt);
    }

    [Fact]
    public void CompletingTwiceKeepsTheFirstCompletionTime()
    {
        TodoItem todo = TodoItem.Create("Write the playbook", Now);
        DateTimeOffset firstCompletion = Now.AddHours(2);

        todo.Complete(firstCompletion);
        todo.Complete(Now.AddHours(5));

        Assert.Equal(firstCompletion, todo.CompletedAt);
    }
}
