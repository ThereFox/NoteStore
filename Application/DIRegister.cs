using Application.Commands;
using Application.Commands.Abstraction;
using Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DIRegister
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<NoteQueryService>();
        services.AddScoped<ICommandHandler<CreateNoteCommand>, AddNoteCommandHandler>();
        
        return services;
    }
}