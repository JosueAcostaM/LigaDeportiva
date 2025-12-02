using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Api_LigaDeportiva.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Api_LigaDeportivaContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Api_LigaDeportivaContext") ?? throw new InvalidOperationException("Connection string 'Api_LigaDeportivaContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
