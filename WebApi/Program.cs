using Abstractions.Persistence;
using Core.DependencyInjection;
using Infrastructure.DependencyInjection;
using WebApi.Exceptions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddCoreFusion();
builder.Services.AddInfrastructure(builder.Configuration);

#endregion Services

WebApplication app = builder.Build();

#region Database

using (IServiceScope scope = app.Services.CreateScope())
{
    IApplicationDatabaseInitializer databaseInitializer = scope.ServiceProvider.GetRequiredService<IApplicationDatabaseInitializer>();

    await databaseInitializer.InitializeAsync();
}

#endregion Database

#region Middleware

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();

#endregion Middleware

#region Endpoints

app.MapControllers();

#endregion Endpoints

await app.RunAsync();
