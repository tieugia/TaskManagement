using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Application.Interfaces.Services;
using TaskManagement.Application.Services;

namespace TaskManagement.Application;

public static class DependencyInjection
{ 
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IQueryTaskService, QueryTaskService>(); 
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
