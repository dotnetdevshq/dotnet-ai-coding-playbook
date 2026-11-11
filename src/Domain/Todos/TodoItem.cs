namespace Playbook.Domain.Todos;

/// <summary>
/// A task to be done. The entity guards its own rules, so it can never exist in an invalid state.
/// </summary>
public sealed class TodoItem
{
    public const int MaxTitleLength = 200;

    private TodoItem(Guid id, string title, DateTimeOffset createdAt)
    {
        Id = id;
        Title = title;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }

    public string Title { get; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public bool IsCompleted => CompletedAt is not null;

    /// <summary>
    /// Creates a new item. The current time is passed in so the domain never reads the system clock.
    /// </summary>
    public static TodoItem Create(string title, DateTimeOffset createdAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        string trimmedTitle = title.Trim();
        if (trimmedTitle.Length > MaxTitleLength)
        {
            throw new ArgumentException($"A title can have at most {MaxTitleLength} characters.", nameof(title));
        }

        return new TodoItem(Guid.NewGuid(), trimmedTitle, createdAt);
    }

    /// <summary>
    /// Marks the item as completed. Completing an item twice keeps the first completion time.
    /// </summary>
    public void Complete(DateTimeOffset completedAt)
    {
        if (IsCompleted)
        {
            return;
        }

        CompletedAt = completedAt;
    }
}
