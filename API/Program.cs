using API.Extensions;
using API.Middlewares;
using API.Seed;
using API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Azure;
//using API.Services.NotificationsHub;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container.
builder.Services.AddApplicationService(builder.Configuration);
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
});


//middleware
var app = builder.Build();
app.UseRouting();
app.UseCors(options =>
options.AllowAnyHeader().AllowCredentials().AllowAnyMethod().WithOrigins("https://job-point.azurewebsites.net", "http://localhost:4200", "http://localhost:3000", "https://jobshubui.azurewebsites.net", "https://jobifyui.azurewebsites.net"));

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseDefaultFiles();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
});


app.MapControllers();

//Seed
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var seed = services.GetService<SeedData>();
    await seed.SeedDatabase();
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}

//Run application
app.Run();