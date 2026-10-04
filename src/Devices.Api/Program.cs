using Devices.Application.Services;
using Devices.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DevicesDb") ?? throw new InvalidOperationException("Connection string 'DevicesDb' not found.");

// Add services to the container.
builder.Services.AddInfrastructure(connectionString);
builder.Services.AddScoped<DeviceService>();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
