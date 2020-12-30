using MediatR;

namespace ProjectManager.Commands
{
    public interface ICommand<out T> : IRequest<T>
    {
    }
}