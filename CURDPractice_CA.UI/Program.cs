using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using Services;

using RepositoryContracts;
using Repositories;

using Serilog;
using Serilog.AspNetCore;
using CURD_Practice.Filters.ActionFilters;

using CURD_Practice;
using CURD_Practice.Middleware;

var builder = WebApplication.CreateBuilder(args);

//Serilog
builder.Host.UseSerilog( (HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfig) =>
{
    loggerConfig
    .ReadFrom.Configuration(context.Configuration) //Read config settings from built-in IConfiguration
    .ReadFrom.Services(services);// Read current app services and make them avilable to serilog
});

builder.Services.ConfigureServices( builder.Configuration , builder.Environment );

var app = builder.Build();

if(builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseExceptionHandelingMiddleware();
}
   
app.UseSerilogRequestLogging();

if (builder.Environment.IsEnvironment("Test") == false)
{
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
}

app.UseHttpLogging();
app.UseStaticFiles();

//Identity and Security
app.UseRouting(); // Identifying actions medthd based on routed

app.UseAuthentication(); //Reading and Identify cookie

app.UseAuthorization(); // Validate access permissions 

app.MapControllers(); // Execute filter pipelines (action + filter)

app.Run();

public partial class Program { }