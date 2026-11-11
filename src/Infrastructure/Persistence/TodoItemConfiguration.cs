using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Playbook.Domain.Todos;

namespace Playbook.Infrastructure.Persistence;

/// <summary>
/// Maps <see cref="TodoItem"/> to its table. The mapping lives here so the domain entity
/// needs no EF Core attributes and no public setters.
/// </summary>
internal sealed class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.ToTable("Todos");

        builder.HasKey(todo => todo.Id);

        // The entity creates its own identifier, so the database must not generate one.
        builder.Property(todo => todo.Id).ValueGeneratedNever();

        // Read-only properties are not mapped by convention. Naming them here maps them.
        builder.Property(todo => todo.Title).HasMaxLength(TodoItem.MaxTitleLength).IsRequired();
        builder.Property(todo => todo.CreatedAt);
        builder.Property(todo => todo.CompletedAt);

        // Calculated from CompletedAt, so it is not stored.
        builder.Ignore(todo => todo.IsCompleted);
    }
}
