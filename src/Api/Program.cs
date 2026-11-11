using Playbook.Api.Endpoints;
using Playbook.Application;
using Playbook.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

WebApplication app = builder.Build();

app.UseExceptionHandler();
app.MapTodoEndpoints();

app.Run();
