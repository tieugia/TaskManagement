using TaskManagement.Application;
using TaskManagement.Infrastructure;
using TaskManagement.Presentation;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddPresentation(builder.Configuration);
services.AddApplication();
services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.Configure();

app.Run();

public partial class Program;
