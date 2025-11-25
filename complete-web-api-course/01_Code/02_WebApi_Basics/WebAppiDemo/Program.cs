var builder = WebApplication.CreateBuilder(args);

// add controllers to DI container
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.Run();
