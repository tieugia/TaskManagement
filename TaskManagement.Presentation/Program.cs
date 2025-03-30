using TaskManagement.Application;
using TaskManagement.Infrastructure;
using TaskManagement.Presentation;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddPresentation();
services.AddApplication();
services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.Configure();
