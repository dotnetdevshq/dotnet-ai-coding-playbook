using Playbook.Api.Endpoints;
using Playbook.Application;
using Playbook.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("Playbook")
    ?? throw new InvalidOperationException("The connection string 'Playbook' is not configured.");

builder.Services.AddProblemDetails();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

WebApplication app = builder.Build();

// Applying migrations at startup is convenient on a developer machine. In production,
// apply them as a separate deployment step, so a failed migration cannot take the app down
// and several instances do not migrate at the same time.
if (app.Environment.IsDevelopment())
{
    await app.Services.ApplyMigrationsAsync();
}

app.UseExceptionHandler();
app.MapTodoEndpoints();

await app.RunAsync();
