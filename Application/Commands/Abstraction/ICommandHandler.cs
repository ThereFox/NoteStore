using CSharpFunctionalExtensions;

namespace Application.Commands.Abstraction;

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    public Task<Result> Handle(TCommand command);
}