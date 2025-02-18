using Microsoft.EntityFrameworkCore;
using SimpleTodo.Api;

var builder = WebApplication.CreateBuilder(args);
// Remove Azure
//var credential = new DefaultAzureCredential();
//builder.Configuration.AddAzureKeyVault(new Uri(builder.Configuration["AZURE_KEY_VAULT_ENDPOINT"]), credential);

builder.Configuration
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.Services.AddScoped<ListsRepository>();
builder.Services.AddDbContext<TodoDb>(options =>
{
    var connectionString = builder.Configuration[builder.Configuration["SQL_CONNECTION_STRING_KEY"]!];
    options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration);
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>(nameof(DatabaseHealthCheck));

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoDb>();
    await db.Database.EnsureCreatedAsync();
}

app.UseCors(policy =>
{
    policy.AllowAnyOrigin();
    policy.AllowAnyHeader();
    policy.AllowAnyMethod();
});

// Swagger UI
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("./openapi.yaml", "v1");
    options.RoutePrefix = "api";
});

app.UseStaticFiles(new StaticFileOptions
{
    // Serve openapi.yaml file
    ServeUnknownFileTypes = true,
});


app.MapGroup("/lists")
    .MapTodoApi()
    .WithOpenApi();

app.MapGroup("/version")
    .MapVersionApi();

// Healtch check
app.MapHealthChecks("/hc");

app.Run();