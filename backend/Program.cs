using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Backend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<ITareasService, TareasService>();
builder.Services.AddScoped<IPlantillasTareaService, PlantillasTareaService>();

var app = builder.Build();

app.MapControllers();

app.Run();
