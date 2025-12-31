using Infrastructure;
using WebApi;
using Application;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);


// To allow client apps to connect with the api
builder.Services.AddCors(o =>
    o.AddPolicy("ABC Admin", builder =>
        {
            builder
            .AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    ));

builder.Services.AddControllers();
builder.Services.AddDatabase(builder.Configuration); // add Db service
builder.Services.AddIdentitySettings();
builder.Services.AddApplicationServices();
builder.Services.AddJwtAuthentication(builder.Services.GetApplicationSettings(builder.Configuration));
builder.Services.AddIdentityServices();
builder.Services.AddEmployeeService();
builder.Services.AddInfrastructureDependencies();

builder.Services.AddEndpointsApiExplorer();
builder.Services.RegisterSwagger();

var app = builder.Build();

app.SeedDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("ABC Admin");

app.UseAuthorization();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

app.Run();
